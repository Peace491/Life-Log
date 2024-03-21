namespace Peace.Lifelog.DataAccess;

using System.Diagnostics;
using DomainModels;
using Microsoft.Extensions.FileSystemGlobbing.Internal.PathSegments;
using Org.BouncyCastle.Asn1.Cmp;
using Peace.Lifelog.RE;

public class RecomendationEngineRepository
{
    public Task<Response> GetNumRecs(string userHash, int numRecs, CancellationToken cancellationToken = default)
    {
        try
        {
            ReadDataOnlyDAO readDataOnlyDAO = new ReadDataOnlyDAO();

            // if we cant get the users datamart, use the system datamart to get their recomendations

            REDataMart userDatamart = new REDataMart
            {
                UserHash = "System",
                MostCommonUserCategory = "Outdoor",
                MostCommonUserSubCategory = "Art",
                MostCommonPublicCategory = "Mental Health"
            };

            // SELECT * FROM LLI WHERE userHash != '0Yg6cgh/M4+ImmL0GozWqhgcDCqTZEhzm9angvVAC30=' ORDER BY RAND() LIMIT 5;
            string query = DynamicallyConstructQuery(userDatamart, numRecs);
            query = $"SELECT * FROM LLI WHERE userHash != '{userHash}' ORDER BY RAND() LIMIT {numRecs};";
            var getRecsResponse = readDataOnlyDAO.ReadData(query); 

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
                    query += "SELECT * FROM ( SELECT * FROM LLIWithCategory WHERE Category = @UsersMostCommon ORDER BY RAND() LIMIT 2 ) AS Cat1 UNION ALL SELECT * FROM ( SELECT * FROM LLIWithCategory WHERE Category = @UserSubCategory ORDER BY RAND() LIMIT 1 ) AS Cat2 UNION ALL SELECT * FROM ( SELECT * FROM LLIWithCategory WHERE Category = @MostCommonPublic ORDER BY RAND() LIMIT 1 ) AS Cat3 UNION ALL SELECT * FROM ( SELECT * FROM LLIWithCategory WHERE Category NOT IN (@UsersMostCommon, @UserSubCategory, @MostCommonPublic) ORDER BY RAND() LIMIT 1 ) AS OtherCats;";
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
            string actual = $"SELECT * FROM LLI WHERE userHash != '{userDatamart.UserHash}' ORDER BY RAND() LIMIT {numRecs};";      
            return actual;
    }

    private string StartQuery()
    {
        // establish temp table, and clear it
        return "SELECT * FROM Recomendations WHERE userHash = ";
    }

    private string SelectLLIWithCategory(string cateogry, int num)
    {
        // select items from LLIWithCategory
        return "SELECT * FROM LLIWithCategory WHERE Category = ";
    }

    private string SelectLLIWithoutCategories(string[] categories, int num)
    {
        // select items from LLIWithCategory
        return "SELECT * FROM LLIWithCategory WHERE Category NOT IN (";
    }

    private string EndQuery()
    {
        // select resulting items from temp table and drop temp table
        return " ORDER BY RAND() LIMIT 1;";
    }

}

