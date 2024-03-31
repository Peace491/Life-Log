namespace Peace.Lifelog.DataAccess;

using System.Diagnostics;
using System.Linq.Expressions;
using DomainModels;
using Microsoft.Extensions.FileSystemGlobbing.Internal.PathSegments;
using Org.BouncyCastle.Asn1.Cmp;
using Peace.Lifelog.RE;

public class RecomendationEngineRepository : IRecomendationEngineRepository
{
    public async Task<Response> GetNumRecs(string userHash, int numRecs, CancellationToken cancellationToken = default)
    {
        var response = new Response();
        try
        {
            
            // TODO : Refactor to use a stored procedure instead of dynamically construcing the query

            ReadDataOnlyDAO readDataOnlyDAO = new ReadDataOnlyDAO();

            var userDataMartQuery = GetUserDataMartQuery(userHash);

            var dataMartResponse = await readDataOnlyDAO.ReadData(userDataMartQuery, null);

            // need to check the first element of output, beacuse if we pass invalid user hash we can still get the system info
            // if (dataMartResponse.Output[0] == null)
            // {
            //     return dataMartResponse;
            // }

            var userDatamart = PopulateUserDataMart(userHash, dataMartResponse);

            string recommendationQuery = DynamicallyConstructQuery(userDatamart, numRecs);

            response = await readDataOnlyDAO.ReadData(recommendationQuery, null); 
        }
        catch (Exception ex)
        {
            // Log or handle the exception as needed
            throw;
        }
        return response;
    }


    // Helper methods

    private REDataMart PopulateUserDataMart(string userHash, Response dataMartResponse)
    {
        List<string> categories = new List<string>();
        if (dataMartResponse.Output == null) return null;
        foreach (List<object> row in dataMartResponse.Output)
        {
            foreach (var key in row)
            {
                categories.Add(key.ToString());
            }
        }
        
        REDataMart userDatamart = new REDataMart
        {
            UserHash = userHash,
            Categories = categories
        };
        return userDatamart;
    }

    private string DynamicallyConstructQuery(REDataMart userDatamart, int numRecs)
    {
        string tableName = $"{userDatamart.UserHash}Recs";
        string query = StartQuery(tableName);

        int current;
        while (numRecs > 0)
            {
                current = numRecs % 5;
                if (current == 0)
                {
                    query += SelectNewLLIWithCategory(tableName, userDatamart.UserHash, userDatamart.Categories[0], 2);
                    query += SelectNewLLIWithCategory(tableName, userDatamart.UserHash, userDatamart.Categories[1], 1);
                    query += SelectNewLLIWithCategory(tableName, userDatamart.UserHash, userDatamart.Categories[2], 1);
                    query += SelectNewLLINotOfCategories(tableName, userDatamart.UserHash, userDatamart.Categories, 1);
                    numRecs -= 5;
                }
                if (current == 4)
                {
                    query += SelectNewLLIWithCategory(tableName, userDatamart.UserHash, userDatamart.Categories[0], 2);
                    query += SelectNewLLIWithCategory(tableName, userDatamart.UserHash, userDatamart.Categories[1], 1);
                    query += SelectNewLLIWithCategory(tableName, userDatamart.UserHash, userDatamart.Categories[2], 1);
                    numRecs -= 4;
                }
                if (current == 3)
                {
                    query += SelectNewLLIWithCategory(tableName, userDatamart.UserHash, userDatamart.Categories[0], 1);
                    query += SelectNewLLIWithCategory(tableName, userDatamart.UserHash, userDatamart.Categories[1], 1);
                    query += SelectNewLLIWithCategory(tableName, userDatamart.UserHash, userDatamart.Categories[2], 1);
                    numRecs -= 3;
                }
                if (current == 2)
                {
                    query += SelectNewLLIWithCategory(tableName, userDatamart.UserHash, userDatamart.Categories[0], 1);
                    query += SelectNewLLIWithCategory(tableName, userDatamart.UserHash, userDatamart.Categories[1], 1);
                    numRecs -= 2;
                }
                if (current == 1)
                {
                    query += SelectNewLLIWithCategory(tableName, userDatamart.UserHash, userDatamart.Categories[2], 1);
                    numRecs--;
                }
            }     
            query += EndQuery(tableName); 
            return query;
    }


 
    private string StartQuery(string tableName)
    {
        return 
        $"CREATE TABLE IF NOT EXISTS `{tableName}` (" +
        "LLIId INT," +
        "UNIQUE KEY (LLIId)" +
        ");" +
        $"DELETE FROM `{tableName}`;";
    }

    private string EndQuery(string tableName)
    {
        return 
        $"SELECT LLI.* " +
        $"FROM LLI " +
        $"INNER JOIN `{tableName}` ON LLI.LLIId = `{tableName}`.LLIId " +
        "ORDER BY RAND();";
    }


    private string SelectNewLLIWithCategory(string tableName, string userHash, string category, int limit)
    {
        // randomly select a variable number  from LLIWithCategory
        return 
        $"INSERT INTO `{tableName}` (LLIId) " +
        "SELECT LLIId FROM LLI " +
        $"WHERE ((UserHash != '{userHash}' " +
        "AND Visibility = 'Public') " +
        $"OR (LLI.UserHash = '{userHash}' " +
        $"AND LLI.Status = 'Completed' AND LLI.CompletionDate < DATE_SUB(CURDATE(), INTERVAL 1 YEAR))) " +
        $"AND LLIId NOT IN (SELECT LLIId FROM `{tableName}`) " +
        $"AND (Category1 = '{category}' OR Category2 = '{category}' OR Category3 = '{category}') " +
        "ORDER BY RAND() " +
        $"LIMIT {limit};";
    }

    private string SelectNewLLINotOfCategories(string tableName, string userHash, List<string> categories, int limit)
    {
        return 
            $"INSERT INTO `{tableName}` (LLIId) " +
            "SELECT LLIId FROM LLI " +
            $"WHERE ((UserHash != '{userHash}' " +
            "AND Visibility = 'Public') " +
            $"AND LLIId NOT IN (SELECT LLIId FROM `{tableName}`) " +
            $"AND (Category1 != '{categories[0]}' AND Category2 != '{categories[0]}' AND Category3 != '{categories[0]}') " +
            $"AND (Category1 != '{categories[1]}' AND Category2 != '{categories[1]}' AND Category3 != '{categories[1]}') " +
            $"AND (Category1 != '{categories[2]}' AND Category2 != '{categories[2]}' AND Category3 != '{categories[2]}') " +
            ") " +
            "ORDER BY RAND() " +
            $"LIMIT {limit};";
    }
    private string GetUserDataMartQuery(string userHash)
    {
        return
            $"SELECT " + 
            $"(SELECT Category1 FROM RecommendationDataMart WHERE UserHash = '{userHash}') AS UserCategory1, " +
            $"(SELECT Category2 FROM RecommendationDataMart WHERE UserHash = '{userHash}') AS UserCategory2, " +
            $"(SELECT Category1 FROM RecommendationDataMart WHERE UserHash = 'System') AS MostPopularSystemCategory ";
    }
}

