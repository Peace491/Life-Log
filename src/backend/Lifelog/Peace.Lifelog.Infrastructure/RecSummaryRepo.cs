namespace Peace.Lifelog.Infrastructure;

using DomainModels;
using Peace.Lifelog.DataAccess;

public class RecSummaryRepo : IRecSummaryRepo
{
    private string selectAllUserHash = "SELECT UserHash FROM LifelogDB.LifelogUserHash;";
    private string tableName = "RecSummary";

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
            var query = $"UPDATE {tableName} SET Category1 = '{category1}', Category2 = '{category2}', SystemMostPopular = (SELECT Category1 FROM (SELECT Category1 FROM RecSummary WHERE UserHash = 'system') AS derivedTable) WHERE UserHash = '{userHash}';";
            var response = await updateDataOnlyDAO.UpdateData(query);
            return response;
        }
        catch (Exception ex)
        {
            // Log or handle the exception as needed
            throw;
        }
    }

    public async Task<Response> UpdateSystemDataMart(CancellationToken cancellationToken = default)
    {
        try
        {
            UpdateDataOnlyDAO updateDataOnlyDAO = new UpdateDataOnlyDAO();
            var res = await GetMostPopularCategory();
            // get the category from the response
            //var category = res.Output[0][0].ToString();
            string category = "Art";
            var query = $"UPDATE {tableName} SET Category1 = '{category}', Category2 = '{category}', SystemMostPopular = '{category}  WHERE UserHash = 'system';";
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

}
