namespace Peace.Lifelog.Logging;

using DataAccess;
using DomainModels;

public class LogRepo : ILogRepo
{
    public Response CreateLog(string level, string category, string? message)
    {

        var createDataOnlyDAO = new CreateDataOnlyDAO();

        string createLogSql = $"INSERT INTO Logs VALUES (NOW(), '{level}', '{category}', '{message}')"; // Need to change date format

        var createLogResponse = createDataOnlyDAO.CreateData(createLogSql); // insert sql statement to insert into log table.
         // above statement is taking the passed info + timestamp 

        return createLogResponse;
    }

    // Might wanna write seperate ReadLog functions based on your search criteria
    public Response ReadLog(string level = "", string category = "", string? message = "")
    {
        var readDataOnlyDAO = new ReadDataOnlyDAO();

        // If an input is left blank, disregard it in the WHERE Clause of the sql statement by setting it to != ''
        string levelInput = level == "" ? "LogLevel != ''" : $"LogLevel = '{level}'";
        string categoryInput = category == "" ? "LogCategory != ''" : $"LogCategory = '{category}'";
        string messageInput = message == "" ? "LogMessage != ''" : $"LogMessage = '{message}'";

        string readLogSql = $"SELECT * FROM Logs WHERE {levelInput} AND {categoryInput} AND {messageInput}";

        var readLogResponse = readDataOnlyDAO.ReadData(readLogSql);

        return readLogResponse;
    }

     public Response DeleteLog(string level = "", string category = "", string? message = "")
    {
        var deleteDataOnlyDAO = new DeleteDataOnlyDAO();

        // If an input is left blank, disregard it in the WHERE Clause of the sql statement by setting it to != ''
        string levelInput = level == "" ? "LogLevel != ''" : $"LogLevel = '{level}'";
        string categoryInput = category == "" ? "LogCategory != ''" : $"LogCategory = '{category}'";
        string messageInput = message == "" ? "LogMessage != ''" : $"LogMessage = '{message}'";

        string deleteLogSql = $"DELETE FROM Logs WHERE {levelInput} AND {categoryInput} AND {messageInput}";

        var deleteLogResponse = deleteDataOnlyDAO.DeleteData(deleteLogSql);

        return deleteLogResponse;
    }
}