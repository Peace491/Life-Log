namespace Peace.Lifelog.DataAccess;

using System.Diagnostics;
using DomainModels;
using Microsoft.Extensions.FileSystemGlobbing.Internal.PathSegments;
using Org.BouncyCastle.Asn1.Cmp;
using Peace.Lifelog.RE;

public class RecomendationEngineRepository
{
    private string test = "";
    public Task<Response> GetNumRecs(string userHash, int numRecs, CancellationToken cancellationToken = default)
    {
        try
        {
            ReadDataOnlyDAO readDataOnlyDAO = new ReadDataOnlyDAO();

            // if we cant get the users datamart, use the system datamart to get their recomendations

            REDataMart userDatamart = new REDataMart
            {
                UserHash = "0Yg6cgh/M4+ImmL0GozWqhgcDCqTZEhzm9angvVAC30=",
                MostCommonUserCategory = "Outdoor",
                MostCommonUserSubCategory = "Art",
                MostCommonPublicCategory = "Mental Health"
            };

            string whereNotUserHash = WhereNotUserHash(userDatamart.UserHash);

            // SELECT * FROM LLI WHERE userHash != '0Yg6cgh/M4+ImmL0GozWqhgcDCqTZEhzm9angvVAC30=' ORDER BY RAND() LIMIT 5;
            string query = DynamicallyConstructQuery(userDatamart, numRecs);
            query += EndQuery();
            query = $"SELECT * FROM LLI WHERE userHash != '{userHash}' ORDER BY RAND() LIMIT {numRecs};";
            var getRecsResponse = readDataOnlyDAO.ReadData(query, null); 

            return getRecsResponse;
        }
        catch (Exception ex)
        {
            // Log or handle the exception as needed
            throw;
        }
    }

    // Helper methods
    private string DynamicallyConstructQuery(REDataMart userDatamart, int numRecs)
    {
        string query = StartQuery();

        int current;
        while (numRecs > 0)
            {
                current = numRecs % 5;
                if (current == 0)
                {
                    // Add logic to pull 5 recomendations
                    query += SelectNewLLIWithCategory(WhereNotUserHash(userDatamart.UserHash), userDatamart.MostCommonUserCategory, 2);
                    query += SelectNewLLIWithCategory(WhereNotUserHash(userDatamart.UserHash), userDatamart.MostCommonUserSubCategory, 2);
                    query += SelectNewLLIWithCategory(WhereNotUserHash(userDatamart.UserHash), userDatamart.MostCommonPublicCategory, 1);
                    numRecs -= 5;
                }
                if (current == 3)
                {
                    // Add logic to pull 3 recomendations
                    numRecs -= 3;
                }
                if (current == 2)
                {
                    // Add logic to pull 2 recomendations
                    numRecs -= 2;
                }
                if (current == 1)
                {
                    
                    numRecs--;
                }
            }     
            query += EndQuery(); 
            
            return query;
    }

    private string StartQuery()
    {
        // establish temp table, and clear it
        // should be priv string
        return 
        "CREATE TEMPORARY TABLE IF NOT EXISTS SelectedLLIIds (LLIId INT); " +
        "TRUNCATE TABLE SelectedLLIIds;";
    }

    private string EndQuery()
    {   
        // Select items from temp table, and drop it
        // should be priv string
        return 
        "SELECT LLI.* " +
        "FROM LLI " +
        "JOIN SelectedLLIIds ON LLI.LLIId = SelectedLLIIds.LLIId; " +
        "DROP TEMPORARY TABLE IF EXISTS SelectedLLIIds; ";
    }


    private string SelectNewLLIWithCategory(string whereUserHash, string category, int num)
    {
        // randomly select a variable number  from LLIWithCategory
        return 
        "INSERT INTO SelectedLLIIds (LLIId) " +
        "SELECT LLIId FROM LLI " +
        whereUserHash +
        $"AND (Category1 = '{category}' OR Category2 = '{category}' OR Category3 = '{category}') " +
        "AND LLIId NOT IN (SELECT LLIId FROM SelectedLLIIds) " +
        "ORDER BY RAND() " +
        $"LIMIT {num};";
    }



    private string WhereNotUserHash(string userHash)
    {
        return $"WHERE UserHash != '{userHash}' ";
    }
}

