namespace Peace.Lifelog.Logging;

using DataAccess;
using DomainModels;

public class LogRepo : ILogRepo
{
    public Response CreateLog(CreateDataOnlyDAO createOnlyDAO, string level, string category, string? message)
    {
        //var createDataOnlyDAO = new CreateDataOnlyDAO();

        DateTime timestamp = DateTime.UtcNow;

        string createLogSql = $"INSERT INTO Logs (LogTimestamp, LogLevel, LogCategory, LogMessage) VALUES (NOW(), '{level}', '{category}', '{message}')"; // Need to change date format

        var createLogResponse = createOnlyDAO.CreateData(createLogSql); // insert sql statement to insert into log table.

        return createLogResponse;
    }

    // Might wanna write seperate ReadLog functions based on your search criteria
    // Agreed, separating could be addressed once its a more significant concern (while doing usage dashboard.)
    public Response ReadLog(ReadDataOnlyDAO readOnlyDAO, string level = "", string category = "", string? message = "")
    {

        // If an input is left blank, disregard it in the WHERE Clause of the sql statement by setting it to != ''
        string levelInput = level == "" ? "LogLevel != ''" : $"LogLevel = '{level}'";
        string categoryInput = category == "" ? "LogCategory != ''" : $"LogCategory = '{category}'";
        string messageInput = message == "" ? "LogMessage != ''" : $"LogMessage = '{message}'";

        string readLogSql = $"SELECT * FROM Logs WHERE {levelInput} AND {categoryInput} AND {messageInput}";

        var readLogResponse = readOnlyDAO.ReadData(readLogSql);

        return readLogResponse;
    }

     public Response DeleteLog(DeleteDataOnlyDAO deleteOnlyDAO, string level = "", string category = "", string? message = "")
    {
        // var deleteDataOnlyDAO = new DeleteDataOnlyDAO();

        // If an input is left blank, disregard it in the WHERE Clause of the sql statement by setting it to != ''
        string levelInput = level == "" ? "LogLevel != ''" : $"LogLevel = '{level}'";
        string categoryInput = category == "" ? "LogCategory != ''" : $"LogCategory = '{category}'";
        string messageInput = message == "" ? "LogMessage != ''" : $"LogMessage = '{message}'";

        string deleteLogSql = $"DELETE FROM Logs WHERE {levelInput} AND {categoryInput} AND {messageInput}";

        var deleteLogResponse = deleteOnlyDAO.DeleteData(deleteLogSql);

        return deleteLogResponse;
    }
}