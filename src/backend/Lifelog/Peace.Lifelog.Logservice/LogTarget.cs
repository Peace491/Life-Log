namespace Peace.Lifelog.Logging;

using DataAccess;
using DomainModels;

public class LogTarget : ILogTarget
{
    private readonly ICreateDataOnlyDAO createOnlyDAO;

    public LogTarget(ICreateDataOnlyDAO createOnlyDAO)
    {
        this.createOnlyDAO = createOnlyDAO;
    }
    public async Task<Response> WriteLog(string table, string userHash, string level, string category, string? message)
    {
        string createLogSql = $"INSERT INTO {table} (Timestamp, UserHash, Level, Category, Message) VALUES (NOW(), '{userHash}', '{level}', '{category}', '{message}')";

        var createLogResponse = await createOnlyDAO.CreateData(createLogSql); // insert sql statement to insert into log table.

        return createLogResponse;
    }
}