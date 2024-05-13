namespace Peace.Lifelog.Logging;

using DataAccess;
using DomainModels;

public class LogTarget : ILogTarget
{
    private readonly ICreateDataOnlyDAO createOnlyDAO;
    private readonly IReadDataOnlyDAO readDataOnlyDAO;

    public LogTarget(ICreateDataOnlyDAO createOnlyDAO, IReadDataOnlyDAO readDataOnlyDAO)
    {
        this.createOnlyDAO = createOnlyDAO;
        this.readDataOnlyDAO = readDataOnlyDAO;
    }

    public async Task<Response> ReadLoginLogsCount(string table, string type, int period)
    {
        var logMessage = "";
        if (type == "Success")
        {
            logMessage = "successfully";
        }
        else
        {
            logMessage = "failed to";
        }

        string sql = $"SELECT Count(*) FROM {table} WHERE Message LIKE '%{logMessage} log in%' AND Timestamp >= DATE_SUB(NOW(), INTERVAL {period} MONTH)";

        var response = await readDataOnlyDAO.ReadData(sql);

        return response;
    }

    public async Task<Response> ReadRegLogsCount(string table, string type, int period)
    {
        var logMessage = "";
        if (type == "Success")
        {
            logMessage = "User registration successful";
        }
        else
        {
            logMessage = "registration failed";
        }

        string sql = $"SELECT Count(*) FROM {table} WHERE Message LIKE '%{logMessage}%' AND Timestamp >= DATE_SUB(NOW(), INTERVAL {period} MONTH)";

        var response = await readDataOnlyDAO.ReadData(sql);

        return response;
    }

    public async Task<Response> ReadTopNLongestPageVisit(string table, int numOfPage, int period)
    {
        string sql = "SELECT "
        + "SUBSTRING_INDEX(SUBSTRING_INDEX(Message, ' was on ', -1), ' for ', 1) AS Page, "
            + "MAX(CAST(SUBSTRING_INDEX(SUBSTRING_INDEX(Message, ' for ', -1), ' seconds', 1) AS UNSIGNED)) AS Longest_Visit_Seconds "
        + "FROM "
            + $"{table} "
        + "WHERE "
            + "Message LIKE '% was on % for % seconds%' "
            + $"AND Timestamp >= DATE_SUB(NOW(), INTERVAL {period} MONTH) "
        + "GROUP BY "
            + "Page "
        + "ORDER BY "
            + "Longest_Visit_Seconds DESC ";

        var response = await readDataOnlyDAO.ReadData(sql, count: numOfPage);

        return response;
    }

    public Task<Response> ReadTopNMostVisitedPage(string table, int numOfPage, int period = 6)
    {
        string sql = "SELECT "
        + "REPLACE(SUBSTRING(Message, POSITION('accessed' IN Message) + 9), '\"', '') AS Page, "
        + "COUNT(*) AS Visit_Count "
        + "FROM "
            + $"{table} "
        + "WHERE "
            + "Category = 'View' "
            + $"AND Timestamp >= DATE_SUB(NOW(), INTERVAL {period} MONTH) "
        + "GROUP BY "
            + "Page "
        + "ORDER BY "
            + "Visit_Count DESC ";

        var response = readDataOnlyDAO.ReadData(sql, count: numOfPage);

        return response;
    }

    public async Task<Response> WriteLog(string table, string userHash, string level, string category, string? message)
    {
        string createLogSql = $"INSERT INTO {table} (Timestamp, UserHash, Level, Category, Message) VALUES (NOW(), '{userHash}', '{level}', '{category}', '{message}')";

        var createLogResponse = await createOnlyDAO.CreateData(createLogSql); // insert sql statement to insert into log table.

        return createLogResponse;
    }
}