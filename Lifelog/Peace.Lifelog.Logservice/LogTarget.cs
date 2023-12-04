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
    public async Task<Response> WriteLog(string level, string category, string? message)
    {
        string createLogSql = $"INSERT INTO Logs (LogTimestamp, LogLevel, LogCategory, LogMessage) VALUES (NOW(), '{level}', '{category}', '{message}')"; // Need to change date format

        var createLogResponse = await createOnlyDAO.CreateData(createLogSql); // insert sql statement to insert into log table.

        return createLogResponse;
    }
}