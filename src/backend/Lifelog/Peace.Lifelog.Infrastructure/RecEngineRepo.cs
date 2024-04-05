namespace Peace.Lifelog.Infrastructure;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DomainModels;
using Peace.Lifelog.DataAccess;
using Peace.Lifelog.Logging;

public class RecEngineRepo : IRecEngineRepo
{
    #region Fields

    private readonly IReadDataOnlyDAO readDataOnlyDAO;
    private readonly ILogging logger;

    #endregion

    #region Constructor

    /// <summary>
    /// Initializes a new instance of the RecEngineRepo class, injecting dependencies for data access and logging.
    /// </summary>
    /// <param name="readDataOnlyDAO">Data access object for read-only database operations.</param>
    /// <param name="logger">Service for logging application events and errors.</param>
    public RecEngineRepo(IReadDataOnlyDAO readDataOnlyDAO, ILogging logger)
    {
        this.readDataOnlyDAO = readDataOnlyDAO;
        this.logger = logger;
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Asynchronously retrieves a list of personalized recommendations for a user.
    /// </summary>
    /// <param name="userHash">The user's unique identifier.</param>
    /// <param name="numRecs">The number of recommendations to retrieve.</param>
    /// <param name="cancellationToken">Optional cancellation token to cancel the request.</param>
    /// <returns>A Response object containing the requested number of recommendations or an error message.</returns>
    public async Task<Response> GetNumRecs(string userHash, int numRecs, CancellationToken cancellationToken = default)
    {
        var response = new Response();
        try
        {
            var userSummaryQuery = GetuserSummaryQuery(userHash);

            response = await readDataOnlyDAO.ReadData(userSummaryQuery, null);

            var userSummary = PopulateUserSummary(userHash, response);

            string recommendationQuery = DynamicallyConstructQuery(userSummary, numRecs);

            response = await readDataOnlyDAO.ReadData(recommendationQuery, null);

            if (response.Output == null || response.Output.Count != numRecs)
            {
                response.HasError = true;
                response.ErrorMessage = $"Unable to find {numRecs} recommendations for user {userHash}.";
            }
        }
        catch (Exception ex)
        {
            await logger.CreateLog("Logs", userHash, "ERROR", "Data Access", ex.Message);
            response.ErrorMessage = "An error occurred while processing your request.";
        }
        return response;
    }

    #endregion

    #region Private Helper Methods

    /// <summary>
    /// Dynamically constructs the SQL query for fetching the specified number of recommendations.
    /// </summary>
    /// <param name="userSummary">User data mart containing information for tailoring recommendations.</param>
    /// <param name="numRecs">Number of recommendations to retrieve.</param>
    /// <returns>The SQL query string.</returns>
    private string DynamicallyConstructQuery(RecSummary? userSummary, int numRecs)
    {
        if (userSummary == null)
        {
            return $"SELECT * FROM LLI ORDER BY RAND() LIMIT {numRecs};";
        }
        string tableName = $"{userSummary.UserHash}Recs";
        string query = StartQuery(tableName);


        int current;
        while (numRecs > 0)
        {
            current = numRecs % 5;
            if (current == 0)
            {
                query += SelectNewLLIWithCategory(tableName, userSummary.UserHash, userSummary.Categories[0], 2);
                query += SelectNewLLIWithCategory(tableName, userSummary.UserHash, userSummary.Categories[1], 2);
                query += SelectNewLLIWithCategory(tableName, userSummary.UserHash, userSummary.Categories[2], 1);
                numRecs -= 5;
            }
            if (current == 4)
            {
                query += SelectNewLLIWithCategory(tableName, userSummary.UserHash, userSummary.Categories[0], 2);
                query += SelectNewLLIWithCategory(tableName, userSummary.UserHash, userSummary.Categories[1], 1);
                query += SelectNewLLIWithCategory(tableName, userSummary.UserHash, userSummary.Categories[2], 1);
                numRecs -= 4;
            }
            if (current == 3)
            {
                query += SelectNewLLIWithCategory(tableName, userSummary.UserHash, userSummary.Categories[0], 1);
                query += SelectNewLLIWithCategory(tableName, userSummary.UserHash, userSummary.Categories[1], 1);
                query += SelectNewLLIWithCategory(tableName, userSummary.UserHash, userSummary.Categories[2], 1);
                numRecs -= 3;
            }
            if (current == 2)
            {
                query += SelectNewLLIWithCategory(tableName, userSummary.UserHash, userSummary.Categories[0], 1);
                query += SelectNewLLIWithCategory(tableName, userSummary.UserHash, userSummary.Categories[1], 1);
                numRecs -= 2;
            }
            if (current == 1)
            {
                query += SelectNewLLIWithCategory(tableName, userSummary.UserHash, userSummary.Categories[2], 1);
                numRecs--;
            }
        }
        query += EndQuery(tableName);
        return query;
    }

    /// <summary>
    /// Starts the SQL query by creating and cleaning a temporary table for the user's recommendations.
    /// </summary>
    /// <param name="tableName">Name of the temporary table for storing recommendations.</param>
    /// <returns>The starting portion of the SQL query.</returns>
    private string StartQuery(string tableName)
    {
        return
        $"CREATE TABLE IF NOT EXISTS `{tableName}` (" +
        "LLIId INT," +
        "UNIQUE KEY (LLIId)" +
        ");" +
        $"DELETE FROM `{tableName}`;";
    }

    /// <summary>
    /// Completes the SQL query with a SELECT statement to fetch recommendations from the temporary table.
    /// </summary>
    /// <param name="tableName">Name of the temporary table with stored recommendations.</param>
    /// <returns>The concluding portion of the SQL query.</returns>
    private string EndQuery(string tableName)
    {
        return
        $"SELECT LLI.* " +
        $"FROM LLI " +
        $"INNER JOIN `{tableName}` ON LLI.LLIId = `{tableName}`.LLIId " +
        "ORDER BY RAND();";
    }

    /// <summary>
    /// Selects new learning life items (LLIs) within a specified category, up to a specified limit.
    /// </summary>
    /// <param name="tableName">Temporary table name for the query.</param>
    /// <param name="userHash">User's unique identifier.</param>
    /// <param name="category">Category of LLIs to include.</param>
    /// <param name="limit">Maximum number of LLIs to select.</param>
    /// <returns>A portion of the SQL query for selecting LLIs within a category.</returns>
    private string SelectNewLLIWithCategory(string tableName, string userHash, string category, int limit)
    {
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

    /// <summary>
    /// Selects new LLIs not matching specified categories, up to a specified limit.
    /// </summary>
    /// <param name="tableName">Temporary table name for the query.</param>
    /// <param name="userHash">User's unique identifier.</param>
    /// <param name="categories">Categories of LLIs to exclude.</param>
    /// <param name="limit">Maximum number of LLIs to select.</param>
    /// <returns>A portion of the SQL query for selecting LLIs not in specified categories.</returns>
    private string SelectNewLLINotOfCategories(string tableName, string userHash, List<string> categories, int limit)
    {
        // Detailed implementation skipped for brevity.
        string category1 = categories[0];
        string category2 = categories[1];
        string category3 = categories[2];
        return
            $"INSERT INTO `{tableName}` (LLIId) " +
            "SELECT LLIId FROM LLI " +
            $"WHERE ((UserHash != '{userHash}' " +
            "AND Visibility = 'Public') " +
            $"OR (LLI.UserHash = '{userHash}' " +
            $"AND LLI.Status = 'Completed' AND LLI.CompletionDate < DATE_SUB(CURDATE(), INTERVAL 1 YEAR))) " +
            $"AND LLIId NOT IN (SELECT LLIId FROM `{tableName}`) " +
            $"AND NOT (Category1 = '{category1}' OR Category2 = '{category1}' OR Category3 = '{category1}') " +
            $"AND NOT (Category1 = '{category2}' OR Category2 = '{category2}' OR Category3 = '{category2}') " +
            $"AND NOT (Category1 = '{category3}' OR Category2 = '{category3}' OR Category3 = '{category3}') " +
            "ORDER BY RAND() " +
            $"LIMIT {limit};";
    }

    /// <summary>
    /// Generates the initial user data mart query based on the user's hash.
    /// </summary>
    /// <param name="userHash">User's unique identifier.</param>
    /// <returns>The SQL query for retrieving user data mart information.</returns>
    private string GetuserSummaryQuery(string userHash)
    {
        return
           $"SELECT " +
           $"(SELECT Category1 FROM RecSummary WHERE UserHash = '{userHash}') AS UserCategory1, " +
           $"(SELECT Category2 FROM RecSummary WHERE UserHash = '{userHash}') AS UserCategory2, " +
           $"(SELECT SystemMostPopular FROM RecSummary WHERE UserHash = '{userHash}') AS MostPopularSystemCategory ";
    }

    /// <summary>
    /// Populates the user data mart with information retrieved from the database.
    /// </summary>
    /// <param name="userHash">User's unique identifier.</param>
    /// <param name="dataMartResponse">Response containing the initial data mart query results.</param>
    /// <returns>An instance of REDataMart populated with the user's data mart information.</returns>
    /// <summary>
    /// Populates a user data mart model with category information from a database response.
    /// </summary>
    /// <param name="userHash">The unique identifier for the user.</param>
    /// <param name="dataMartResponse">The response object containing the user's data mart information from the database.</param>
    /// <returns>An instance of REDataMart populated with the user's category preferences. Returns null if the response contains no data.</returns>
    private RecSummary? PopulateUserSummary(string userHash, Response dataMartResponse)
    {
        // Check if the response from the database is valid and contains output
        if (dataMartResponse.Output == null || dataMartResponse.Output.Count == 0)
        {
            // Consider logging the lack of data for debugging or audit purposes
            return null;
        }

        // Initialize a list to hold the categories extracted from the response
        List<string> categories = new List<string>();

        // Iterate through each row of the response output
        foreach (List<object> row in dataMartResponse.Output)
        {
            // Each row is expected to contain category information; extract and add it to the categories list
            foreach (var item in row)
            {
                // Ensure the item is not null before calling ToString to avoid a potential NullReferenceException
                string i = item.ToString()!;
                if (i != null)
                {
                    categories.Add(i);
                }
            }
        }

        // Create and return the populated REDataMart object
        RecSummary userSummary = new RecSummary
        {
            UserHash = userHash,
            Categories = categories
        };

        return userSummary;
    }
    #endregion
}
