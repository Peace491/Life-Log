namespace Peace.Lifelog.DataAccess;

using System.Diagnostics;
using System.Linq.Expressions;
using DomainModels;
using Microsoft.Extensions.FileSystemGlobbing.Internal.PathSegments;
using Org.BouncyCastle.Asn1.Cmp;
using Peace.Lifelog.RE;

public class RecomendationEngineRepository : IRecomendationEngineRepository
{
    
    private string selectAllUserHash = "SELECT UserHash FROM LifelogDB.LifelogUserHash;";
    public async Task<Response> GetNumRecs(string userHash, int numRecs, CancellationToken cancellationToken = default)
    {
        try
        {
            ReadDataOnlyDAO readDataOnlyDAO = new ReadDataOnlyDAO();

            var userDataMartQuery = GetUserDataMartQuery(userHash);

            var dataMartResponse = await readDataOnlyDAO.ReadData(userDataMartQuery, null);

            var userDatamart = PopulateUserDataMart(userHash, dataMartResponse);

            string recommendationQuery = DynamicallyConstructQuery(userDatamart, numRecs);

            var getRecsResponse = await readDataOnlyDAO.ReadData(recommendationQuery, null); 

            return getRecsResponse;
        }
        catch (Exception ex)
        {
            // Log or handle the exception as needed
            throw;
        }
    }

    public async Task<Response> GetAllUserHash(CancellationToken cancellationToken = default)
    {
        try
        {
            ReadDataOnlyDAO readDataOnlyDAO = new ReadDataOnlyDAO();
            var response = await readDataOnlyDAO.ReadData(selectAllUserHash, null);
            return response;
        }
        catch (Exception ex)
        {
            // Log or handle the exception as needed
            throw;
        }
    }

    public async Task<Response> GetUserForm(string userHash, CancellationToken cancellationToken = default)
    {
        try
        {
            ReadDataOnlyDAO readDataOnlyDAO = new ReadDataOnlyDAO();
            var query = $"SELECT Category, Rating FROM UserForm WHERE UserHash = '{userHash}' ORDER BY Rating ASC;;";
            var response = await readDataOnlyDAO.ReadData(query, null);
            return response;
        }
        catch (Exception ex)
        {
            // Log or handle the exception as needed
            throw;
        }
    }

    public async Task<Response> GetNumUserLLI(string userHash, int? limit, CancellationToken cancellationToken = default)
    {
        try
        {
            ReadDataOnlyDAO readDataOnlyDAO = new ReadDataOnlyDAO();
            var query = $"SELECT Status, Category1, Category2, Category3  FROM LLI WHERE UserHash = '{userHash}';";
            var response = await readDataOnlyDAO.ReadData(query, limit);
            return response;
        }
        catch (Exception ex)
        {
            // Log or handle the exception as needed
            throw;
        }
    }

    public async Task<Response> UpdateUserDataMart(string userHash, string category1, string category2, CancellationToken cancellationToken = default)
    {
        try
        {
            UpdateDataOnlyDAO updateDataOnlyDAO = new UpdateDataOnlyDAO();
            var query = $"UPDATE RecommendationDataMart SET CategoryOne = '{category1}', CategoryTwo = '{category2}' WHERE UserHash = '{userHash}';";
            var response = await updateDataOnlyDAO.UpdateData(query);
            return response;
        }
        catch (Exception ex)
        {
            // Log or handle the exception as needed
            throw;
        }
    }

    public async Task<Response> GetMostPopularCategory(CancellationToken cancellationToken = default)
    {
        try
        {
            ReadDataOnlyDAO readDataOnlyDAO = new ReadDataOnlyDAO();
            string lliTable = "LLI";
            string query = 
            $"SELECT category, COUNT(*) AS occurrence_count " +
            "FROM ( " +
            $"SELECT category1 AS category FROM {lliTable} WHERE category1 <> 'None' " +
            "UNION ALL " +
            $"SELECT category2 FROM {lliTable} WHERE category2 <> 'None' " +
            "UNION ALL " +
            $"SELECT category3 FROM {lliTable} WHERE category3 <> 'None' " +
            ") AS combined_categories " +
            "GROUP BY category " +
            "ORDER BY occurrence_count DESC " +
            "LIMIT 1;";
            var response = await readDataOnlyDAO.ReadData(query, null);
            return response;
        }
        catch (Exception ex)
        {
            // Log or handle the exception as needed
            throw;
        }
    }

    // Helper methods

    private string GetUserDataMartQuery(string userHash)
    {
        return
            $"SELECT " + 
            $"(SELECT CategoryOne FROM RecommendationDataMart WHERE UserHash = '{userHash}') AS UserCategoryOne, " +
            $"(SELECT CategoryTwo FROM RecommendationDataMart WHERE UserHash = '{userHash}') AS UserCategoryTwo, " +
            $"(SELECT CategoryOne FROM RecommendationDataMart WHERE UserHash = 'System') AS MostPopularSystemCategory ";
    }

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
                    query += SelectNewLLIWithCategory(tableName, userDatamart.UserHash, userDatamart.Categories[0], 1);
                    query += SelectNewLLIWithCategory(tableName, userDatamart.UserHash, userDatamart.Categories[1], 1);
                    query += SelectNewLLIWithCategory(tableName, userDatamart.UserHash, userDatamart.Categories[2], 1);
                    query += SelectNewLLINotOfCategories(tableName, userDatamart.UserHash, userDatamart.Categories, 1);
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
        $"AND LLI.Status = 'complete' AND LLI.CompletionDate < DATE_SUB(CURDATE(), INTERVAL 1 YEAR))) " +
        $"AND LLIId NOT IN (SELECT LLIId FROM `{tableName}`) " +
        $"AND (Category1 = '{category}' OR Category2 = '{category}' OR Category3 = '{category}') " +
        "ORDER BY RAND() " +
        $"LIMIT {limit};";
    }

    private string SelectNewLLINotOfCategories(string tableName, string userHash, List<string> categories, int limit)
    {
        // do this way to handle the case where datamart takes more information in
        string categoriesString = string.Join("', '", categories);
        return 
            $"INSERT INTO `{tableName}` (LLIId) " +
            "SELECT LLIId FROM LLI " +
            $"WHERE UserHash != '{userHash}' " +
            "AND Visibility = 'public' " +
            $"AND LLIId NOT IN (SELECT LLIId FROM `{tableName}`) " +
            $"AND Category1 NOT IN ('{categoriesString}') " +
            $"AND Category2 NOT IN ('{categoriesString}') " +
            $"AND Category3 NOT IN ('{categoriesString}') " +
            "ORDER BY RAND() " +
            $"LIMIT {limit};";
    }
}

