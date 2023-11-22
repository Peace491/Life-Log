namespace Peace.Lifelog.Logging;

using DataAccess;
using DomainModels;

public class LogTarget : ILogTarget
{
    public async Task<Response> WriteLog(CreateDataOnlyDAO createOnlyDAO, string level, string category, string? message)
    {
        string createLogSql = $"INSERT INTO Logs (LogTimestamp, LogLevel, LogCategory, LogMessage) VALUES (NOW(), '{level}', '{category}', '{message}')"; // Need to change date format

        var createLogResponse = await createOnlyDAO.CreateData(createLogSql); // insert sql statement to insert into log table.

        return createLogResponse;
    }
}