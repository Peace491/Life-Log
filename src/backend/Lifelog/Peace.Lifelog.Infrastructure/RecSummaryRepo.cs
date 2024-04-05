namespace Peace.Lifelog.Infrastructure;

using DomainModels;
using Peace.Lifelog.DataAccess;
using Peace.Lifelog.Logging;
using System;
using System.Threading.Tasks;

public class RecSummaryRepo : IRecSummaryRepo
{
    private readonly string selectAllUserHashQuery = "SELECT UserHash FROM LifelogDB.LifelogUserHash;";
    private readonly string tableName = "RecSummary";
    private readonly IReadDataOnlyDAO readDataOnlyDAO;
    private readonly IUpdateDataOnlyDAO updateDataOnlyDAO;
    private readonly ILogging logger;

    public RecSummaryRepo(IReadDataOnlyDAO readDataOnlyDAO, IUpdateDataOnlyDAO updateDataOnlyDAO, ILogging logger)
    {
        this.readDataOnlyDAO = readDataOnlyDAO;
        this.updateDataOnlyDAO = updateDataOnlyDAO;
        this.logger = logger;
    }

    public async Task<Response> GetAllUserHash(CancellationToken cancellationToken = default)
    {
        try
        {
            return await readDataOnlyDAO.ReadData(selectAllUserHashQuery);
        }
        catch (Exception ex)
        {
            await logger.CreateLog("Logs", "RecSummaryRepo", "ERROR", "Data", ex.Message);
            return new Response { HasError = true, ErrorMessage = ex.Message };
        }
    }

    public async Task<Response> GetUserForm(string userHash, CancellationToken cancellationToken = default)
    {
        try
        {
            var query = $"SELECT MentalHealthRanking, PhysicalHealthRanking, OutdoorRanking, SportRanking, ArtRanking, HobbyRanking, ThrillRanking, TravelRanking, VolunteeringRanking, FoodRanking FROM UserForm WHERE UserHash = '{userHash}'";
            return await readDataOnlyDAO.ReadData(query);
        }
        catch (Exception ex)
        {
            await logger.CreateLog("Logs", "RecSummaryRepo", "ERROR", "Data", ex.Message);
            return new Response { HasError = true, ErrorMessage = ex.Message };
        }
    }

    public async Task<Response> GetNumUserLLI(string userHash, int? limit, CancellationToken cancellationToken = default)
    {
        try
        {
            var query = $"SELECT Status, Category1, Category2, Category3 FROM LLI WHERE UserHash = '{userHash}';";
            return await readDataOnlyDAO.ReadData(query, limit);
        }
        catch (Exception ex)
        {
            await logger.CreateLog("Logs", "RecSummaryRepo", "ERROR", "Data", ex.Message);
            return new Response { HasError = true, ErrorMessage = ex.Message };
        }
    }

    public async Task<Response> UpdateUserDataMart(string userHash, string category1, string category2, CancellationToken cancellationToken = default)
    {
        try
        {
            var query = $"UPDATE {tableName} SET Category1 = '{category1}', Category2 = '{category2}', SystemMostPopular = (SELECT Category1 FROM (SELECT Category1 FROM RecSummary WHERE UserHash = 'system') AS derivedTable) WHERE UserHash = '{userHash}';";
            return await updateDataOnlyDAO.UpdateData(query);
        }
        catch (Exception ex)
        {
            await logger.CreateLog("Logs", "RecSummaryRepo", "ERROR", "Data", ex.Message);
            return new Response { HasError = true, ErrorMessage = ex.Message };
        }
    }

    public async Task<Response> UpdateSystemDataMart(CancellationToken cancellationToken = default)
    {
        try
        {
            var res = await GetMostPopularCategory(cancellationToken);
            // Extract the most popular category from the response. Assuming response parsing is correct.
            string category = "Art"; // Placeholder for actual extraction logic
            var query = $"UPDATE {tableName} SET Category1 = '{category}', Category2 = '{category}', SystemMostPopular = '{category}' WHERE UserHash = 'system';";
            return await updateDataOnlyDAO.UpdateData(query);
        }
        catch (Exception ex)
        {
            await logger.CreateLog("Logs", "RecSummaryRepo", "ERROR", "Data", ex.Message);
            return new Response { HasError = true, ErrorMessage = ex.Message };
        }
    }

    public async Task<Response> GetMostPopularCategory(CancellationToken cancellationToken = default)
    {
        try
        {
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
            return await readDataOnlyDAO.ReadData(query, null);
        }
        catch (Exception ex)
        {
            await logger.CreateLog("Logs", "RecSummaryRepo", "ERROR", "Data", ex.Message);
            return new Response { HasError = true, ErrorMessage = ex.Message };
        }
    }
}
