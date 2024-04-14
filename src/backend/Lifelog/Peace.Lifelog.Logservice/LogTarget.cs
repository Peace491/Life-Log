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

    public async Task<Response> ReadLoginLogsCount(string table, string type)
    {
        var logMessage = "";
        if (type == "Success") {
            logMessage = "successfully";
        } else {
            logMessage = "failed to";
        }

        string sql = $"SELECT Count(*) FROM {table} WHERE Message LIKE '%{logMessage} log in%'";

        var response = await readDataOnlyDAO.ReadData(sql);

        return response;
    }

    public async Task<Response> ReadRegLogsCount(string table, string type)
    {
        var logMessage = "";
        if (type == "Success") {
            logMessage = "User registration successful";
        } else {
            logMessage = "User registration failed";
        }

        string sql = $"SELECT Count(*) FROM {table} WHERE Message LIKE '%{logMessage} log in%'";

        var response = await readDataOnlyDAO.ReadData(sql);

        return response;
    }

    public Task<Response> ReadTopNMostVisitedPage(string table, int numOfLog, int period = 6)
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

        var response = readDataOnlyDAO.ReadData(sql, count: numOfLog);

        return response;
    }

    public async Task<Response> WriteLog(string table, string userHash, string level, string category, string? message)
    {
        string createLogSql = $"INSERT INTO {table} (Timestamp, UserHash, Level, Category, Message) VALUES (NOW(), '{userHash}', '{level}', '{category}', '{message}')";

        var createLogResponse = await createOnlyDAO.CreateData(createLogSql); // insert sql statement to insert into log table.

        return createLogResponse;
    }
}