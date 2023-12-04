namespace Peace.Lifelog.LogServiceTest;

using System.Threading.Tasks;
using Peace.Lifelog.DataAccess;
using Peace.Lifelog.Logging;

using System.Diagnostics;
public class LoggingShould
{
    private const int MAX_EXECUTION_TIME_IN_SECONDS = 3001;

    private const int LOG_ID_INDEX = 0;

    [Fact]
    public async void LoggingShould_CreateAnInfoLog()
    {
        // Arrange
        string infoLogLevel = "Info";
        string testLogCategory = "View";
        string? testLogMessage = null;

        // TODO fix with updated response object

        Stopwatch timer = new Stopwatch();

        // Need to initlaize all types of DAO for checking accuracy and cleanup.
        var createOnlyDAO = new CreateDataOnlyDAO();
        var logTarget = new LogTarget(createOnlyDAO);
        var logger = new Logging(logTarget);
        var readOnlyDAO = new ReadDataOnlyDAO();
        var deleteDataDAO = new DeleteDataOnlyDAO(); 

        // Act
        timer.Start();
        var createLogResponse = await logger.CreateLog(createOnlyDAO, infoLogLevel, testLogCategory, testLogMessage);
        timer.Stop();
        
        var infoLogSql = $"SELECT * FROM Logs WHERE LogId={createLogResponse.Output.ElementAt(LOG_ID_INDEX)}"; 
        var readLogResponse = await readOnlyDAO.ReadData(infoLogSql, 1);

        var cleanupSql = $"DELETE FROM Logs WHERE LogId='{createLogResponse.Output.ElementAt(LOG_ID_INDEX)}'";

        // Assert
        Assert.True(timer.ElapsedMilliseconds < MAX_EXECUTION_TIME_IN_SECONDS);
        Assert.True(createLogResponse.HasError == false);
        Assert.True(readLogResponse.HasError == false);
        foreach (List<Object> readLogResponseData in readLogResponse.Output)
        {
            Assert.True(readLogResponseData[2].ToString() == infoLogLevel);
        }
        
        // Cleanup
        var deleteLogResponse = await deleteDataDAO.DeleteData(cleanupSql);

        var logTransaction = new LogTransaction();
        await logTransaction.DeleteDataAccessTransactionLog(createLogResponse.LogId);
        await logTransaction.DeleteDataAccessTransactionLog(readLogResponse.LogId);
        await logTransaction.DeleteDataAccessTransactionLog(deleteLogResponse.LogId);
    }
    [Fact]
    public async void LoggingShould_CreateADebugLog()
    {
        // Arrange
        string debugLogLevel = "Info";
        string testLogCategory = "View";
        string? testLogMessage = null;

        // TODO fix with updated response object

        Stopwatch timer = new Stopwatch();

        // Need to initlaize all types of DAO for checking accuracy and cleanup.
        var createOnlyDAO = new CreateDataOnlyDAO();
        var logTarget = new LogTarget(createOnlyDAO);
        var logger = new Logging(logTarget);
        var readOnlyDAO = new ReadDataOnlyDAO();
        var deleteDataDAO = new DeleteDataOnlyDAO(); 

        // Act
        timer.Start();
        var createLogResponse = await logger.CreateLog(createOnlyDAO, debugLogLevel, testLogCategory, testLogMessage);
        timer.Stop();
        
        var infoLogSql = $"SELECT * FROM Logs WHERE LogId={createLogResponse.Output.ElementAt(LOG_ID_INDEX)}"; 
        var readLogResponse = await readOnlyDAO.ReadData(infoLogSql, 1);

        var cleanupSql = $"DELETE FROM Logs WHERE LogId='{createLogResponse.Output.ElementAt(LOG_ID_INDEX)}'";

        // Assert
        Assert.True(timer.ElapsedMilliseconds < MAX_EXECUTION_TIME_IN_SECONDS);
        Assert.True(createLogResponse.HasError == false);
        Assert.True(readLogResponse.HasError == false);
        foreach (List<Object> readLogResponseData in readLogResponse.Output)
        {
            Assert.True(readLogResponseData[2].ToString() == debugLogLevel);
        }
        
        // Cleanup
        var deleteLogResponse = await deleteDataDAO.DeleteData(cleanupSql);

        var logTransaction = new LogTransaction();
        await logTransaction.DeleteDataAccessTransactionLog(createLogResponse.LogId);
        await logTransaction.DeleteDataAccessTransactionLog(readLogResponse.LogId);
        await logTransaction.DeleteDataAccessTransactionLog(deleteLogResponse.LogId);
    }

    [Fact]
    public async void LoggingShould_CreateAWarningLog()
    {
        // Arrange
        string warningLogLevel = "Warning";
        string testLogCategory = "View";
        string? testLogMessage = null;

        // TODO fix with updated response object

        Stopwatch timer = new Stopwatch();

        // Need to initlaize all types of DAO for checking accuracy and cleanup.
        var createOnlyDAO = new CreateDataOnlyDAO();
        var logTarget = new LogTarget(createOnlyDAO);
        var logger = new Logging(logTarget);
        var readOnlyDAO = new ReadDataOnlyDAO();
        var deleteDataDAO = new DeleteDataOnlyDAO(); 

        // Act
        timer.Start();
        var createLogResponse = await logger.CreateLog(createOnlyDAO, warningLogLevel, testLogCategory, testLogMessage);
        timer.Stop();

        var infoLogSql = $"SELECT * FROM Logs WHERE LogId={createLogResponse.Output.ElementAt(LOG_ID_INDEX)}"; 
        var readLogResponse = await readOnlyDAO.ReadData(infoLogSql, 1);

        var cleanupSql = $"DELETE FROM Logs WHERE LogId='{createLogResponse.Output.ElementAt(LOG_ID_INDEX)}'";

        // Assert
        Assert.True(timer.ElapsedMilliseconds < MAX_EXECUTION_TIME_IN_SECONDS);
        Assert.True(createLogResponse.HasError == false);
        Assert.True(readLogResponse.HasError == false);
        foreach (List<Object> readLogResponseData in readLogResponse.Output)
        {
            Assert.True(readLogResponseData[2].ToString() == warningLogLevel);
        }
        
        // Cleanup
        var deleteLogResponse = await deleteDataDAO.DeleteData(cleanupSql);

        var logTransaction = new LogTransaction();
        await logTransaction.DeleteDataAccessTransactionLog(createLogResponse.LogId);
        await logTransaction.DeleteDataAccessTransactionLog(readLogResponse.LogId);
        await logTransaction.DeleteDataAccessTransactionLog(deleteLogResponse.LogId);
    }

    [Fact]
    public async void LoggingShould_CreateAnErrorLog()
    {
        // Arrange
        string errorLogLevel = "ERROR";
        string testLogCategory = "View";
        string? testLogMessage = null;

        // TODO fix with updated response object

        Stopwatch timer = new Stopwatch();

        // Need to initlaize all types of DAO for checking accuracy and cleanup.
        var createOnlyDAO = new CreateDataOnlyDAO();
        var logTarget = new LogTarget(createOnlyDAO);
        var logger = new Logging(logTarget);
        var readOnlyDAO = new ReadDataOnlyDAO();
        var deleteDataDAO = new DeleteDataOnlyDAO(); 

        // Act
        timer.Start();
        var createLogResponse = await logger.CreateLog(createOnlyDAO, errorLogLevel, testLogCategory, testLogMessage);
        
        timer.Stop();
        
        var infoLogSql = $"SELECT * FROM Logs WHERE LogId={createLogResponse.Output.ElementAt(LOG_ID_INDEX)}"; 
        var readLogResponse = await readOnlyDAO.ReadData(infoLogSql, 1);

        var cleanupSql = $"DELETE FROM Logs WHERE LogId='{createLogResponse.Output.ElementAt(LOG_ID_INDEX)}'";

        // Assert
        Assert.True(timer.ElapsedMilliseconds < MAX_EXECUTION_TIME_IN_SECONDS);
        Assert.True(createLogResponse.HasError == false);
        Assert.True(readLogResponse.HasError == false);
        foreach (List<Object> readLogResponseData in readLogResponse.Output)
        {
            Assert.True(readLogResponseData[2].ToString() == errorLogLevel);
        }
        
        // Cleanup
        var deleteLogResponse = await deleteDataDAO.DeleteData(cleanupSql);

        var logTransaction = new LogTransaction();
        await logTransaction.DeleteDataAccessTransactionLog(createLogResponse.LogId);
        await logTransaction.DeleteDataAccessTransactionLog(readLogResponse.LogId);
        await logTransaction.DeleteDataAccessTransactionLog(deleteLogResponse.LogId);
    }
    [Fact]
    public async Task LoggingShouldNot_CreateALogWithInvalidLevel()
    {
        // Arrange
        string invalidLogLevel = "Apple";
        string testLogCategory = "View";
        string? testLogMessage = null;

        var createOnlyDAO = new CreateDataOnlyDAO();
        var logTarget = new LogTarget(createOnlyDAO);
        var logger = new Logging(logTarget);


        Stopwatch timer = new Stopwatch();

        // Act
        timer.Start();
        var invalidLevelResponse = await logger.CreateLog(createOnlyDAO, invalidLogLevel, testLogCategory, testLogMessage);
        timer.Stop();

        // Assert
        Assert.True(timer.ElapsedMilliseconds < MAX_EXECUTION_TIME_IN_SECONDS);
        Assert.True(invalidLevelResponse.HasError == true);
    }

    // Category Testing

    [Fact]
    public async void LoggingShould_CreateAViewLog()
    {
        // Arrange
        string testLogLevel = "Info";
        string viewLogCategory = "View";
        string? testLogMessage = null;

        
        // TODO fix with updated response object

        Stopwatch timer = new Stopwatch();

        // Need to initlaize all types of DAO for checking accuracy and cleanup.
        var createOnlyDAO = new CreateDataOnlyDAO();
        var logTarget = new LogTarget(createOnlyDAO);
        var logger = new Logging(logTarget);
        var readOnlyDAO = new ReadDataOnlyDAO();
        var deleteDataDAO = new DeleteDataOnlyDAO(); 

        // Act
        timer.Start();
        var createLogResponse = await logger.CreateLog(createOnlyDAO, testLogLevel, viewLogCategory, testLogMessage);
        timer.Stop();

        var infoLogSql = $"SELECT * FROM Logs WHERE LogId={createLogResponse.Output.ElementAt(LOG_ID_INDEX)}"; 
        var readLogResponse = await readOnlyDAO.ReadData(infoLogSql, 1);

        var cleanupSql = $"DELETE FROM Logs WHERE LogId='{createLogResponse.Output.ElementAt(LOG_ID_INDEX)}'";
        

        // Assert
        Assert.True(timer.ElapsedMilliseconds < MAX_EXECUTION_TIME_IN_SECONDS);
        Assert.True(createLogResponse.HasError == false);
        Assert.True(readLogResponse.HasError == false);
        foreach (List<Object> readLogResponseData in readLogResponse.Output)
        {
            Assert.True(readLogResponseData[3].ToString() == viewLogCategory);
        }
        
        // Cleanup
        var deleteLogResponse = await deleteDataDAO.DeleteData(cleanupSql);

        var logTransaction = new LogTransaction();
        await logTransaction.DeleteDataAccessTransactionLog(createLogResponse.LogId);
        await logTransaction.DeleteDataAccessTransactionLog(readLogResponse.LogId);
        await logTransaction.DeleteDataAccessTransactionLog(deleteLogResponse.LogId);
    }
    [Fact]
    public async void LoggingShould_CreateABuisnessLog()
    {
        // Arrange
        string testLogLevel = "Info";
        string businessLogCategory = "Business";
        string? testLogMessage = null;

        // TODO fix with updated response object

        Stopwatch timer = new Stopwatch();

        // Need to initlaize all types of DAO for checking accuracy and cleanup.
        var createOnlyDAO = new CreateDataOnlyDAO();
        var logTarget = new LogTarget(createOnlyDAO);
        var logger = new Logging(logTarget);
        var readOnlyDAO = new ReadDataOnlyDAO();
        var deleteDataDAO = new DeleteDataOnlyDAO(); 

        // Act
        timer.Start();
        var createLogResponse = await logger.CreateLog(createOnlyDAO, testLogLevel, businessLogCategory, testLogMessage);
        timer.Stop();
        
        var infoLogSql = $"SELECT * FROM Logs WHERE LogId={createLogResponse.Output.ElementAt(LOG_ID_INDEX)}"; 
        var readLogResponse = await readOnlyDAO.ReadData(infoLogSql, 1);

        var cleanupSql = $"DELETE FROM Logs WHERE LogId='{createLogResponse.Output.ElementAt(LOG_ID_INDEX)}'";

        // Assert
        Assert.True(timer.ElapsedMilliseconds < MAX_EXECUTION_TIME_IN_SECONDS);
        Assert.True(createLogResponse.HasError == false);
        Assert.True(readLogResponse.HasError == false);
        foreach (List<Object> readLogResponseData in readLogResponse.Output)
        {
            Assert.True(readLogResponseData[3].ToString() == businessLogCategory);
        }
        
        // Cleanup
        var deleteLogResponse = await deleteDataDAO.DeleteData(cleanupSql);

        var logTransaction = new LogTransaction();
        await logTransaction.DeleteDataAccessTransactionLog(createLogResponse.LogId);
        await logTransaction.DeleteDataAccessTransactionLog(readLogResponse.LogId);
        await logTransaction.DeleteDataAccessTransactionLog(deleteLogResponse.LogId);
    }

    [Fact]
    public async void LoggingShould_CreateAServerLog()
    {
        // Arrange
        string testLogLevel = "Info";
        string serverLogCategory = "Server";
        string? testLogMessage = null;
 
        // TODO fix with updated response object

        Stopwatch timer = new Stopwatch();

        // Need to initlaize all types of DAO for checking accuracy and cleanup.
        var createOnlyDAO = new CreateDataOnlyDAO();
        var logTarget = new LogTarget(createOnlyDAO);
        var logger = new Logging(logTarget);
        var readOnlyDAO = new ReadDataOnlyDAO();
        var deleteDataDAO = new DeleteDataOnlyDAO(); 

        // Act
        timer.Start();
        var createLogResponse = await logger.CreateLog(createOnlyDAO, testLogLevel, serverLogCategory, testLogMessage);
        timer.Stop();
        
        var infoLogSql = $"SELECT * FROM Logs WHERE LogId={createLogResponse.Output.ElementAt(LOG_ID_INDEX)}"; 
        var readLogResponse = await readOnlyDAO.ReadData(infoLogSql, 1);

        var cleanupSql = $"DELETE FROM Logs WHERE LogId='{createLogResponse.Output.ElementAt(LOG_ID_INDEX)}'";

        // Assert
        Assert.True(timer.ElapsedMilliseconds < MAX_EXECUTION_TIME_IN_SECONDS);
        Assert.True(createLogResponse.HasError == false);
        Assert.True(readLogResponse.HasError == false);
        foreach (List<Object> readLogResponseData in readLogResponse.Output)
        {
            Assert.True(readLogResponseData[3].ToString() == serverLogCategory);
        }
        
        // Cleanup
        var deleteLogResponse = await deleteDataDAO.DeleteData(cleanupSql);

        var logTransaction = new LogTransaction();
        await logTransaction.DeleteDataAccessTransactionLog(createLogResponse.LogId);
        await logTransaction.DeleteDataAccessTransactionLog(readLogResponse.LogId);
        await logTransaction.DeleteDataAccessTransactionLog(deleteLogResponse.LogId);

    }
    [Fact]
    public async void LoggingShould_CreateADataLog()
    {
        // Arrange
        string testLogLevel = "Info";
        string dataLogCategory = "Data";
        string? testLogMessage = null;

        // TODO fix with updated response object

        Stopwatch timer = new Stopwatch();

        // Need to initlaize all types of DAO for checking accuracy and cleanup.
        var createOnlyDAO = new CreateDataOnlyDAO();
        var logTarget = new LogTarget(createOnlyDAO);
        var logger = new Logging(logTarget);
        var readOnlyDAO = new ReadDataOnlyDAO();
        var deleteDataDAO = new DeleteDataOnlyDAO(); 

        // Act
        timer.Start();
        var createLogResponse = await logger.CreateLog(createOnlyDAO, testLogLevel, dataLogCategory, testLogMessage);
        timer.Stop();
        
        var infoLogSql = $"SELECT * FROM Logs WHERE LogId={createLogResponse.Output.ElementAt(LOG_ID_INDEX)}"; 
        var readLogResponse = await readOnlyDAO.ReadData(infoLogSql, 1);

        var cleanupSql = $"DELETE FROM Logs WHERE LogId='{createLogResponse.Output.ElementAt(LOG_ID_INDEX)}'";

        // Assert
        Assert.True(timer.ElapsedMilliseconds < MAX_EXECUTION_TIME_IN_SECONDS);
        Assert.True(createLogResponse.HasError == false);
        Assert.True(readLogResponse.HasError == false);
        foreach (List<Object> readLogResponseData in readLogResponse.Output)
        {
            Assert.True(readLogResponseData[3].ToString() == dataLogCategory);
        }
        
        // Cleanup
        var deleteLogResponse = await deleteDataDAO.DeleteData(cleanupSql);

        var logTransaction = new LogTransaction();
        await logTransaction.DeleteDataAccessTransactionLog(createLogResponse.LogId);
        await logTransaction.DeleteDataAccessTransactionLog(readLogResponse.LogId);
        await logTransaction.DeleteDataAccessTransactionLog(deleteLogResponse.LogId);
    }
    [Fact]
    public async void LoggingShould_CreateAPersistentDataStoreLog()
    {
        // Arrange
        string testLogLevel = "Info";
        string persistentDataStoreLogCategory = "Persistent Data Store";
        string? testLogMessage = null;

        var infoLogSql = $"SELECT * FROM Logs ORDER BY LogTimestamp DESC"; 
        // TODO fix with updated response object

        Stopwatch timer = new Stopwatch();

        // Need to initlaize all types of DAO for checking accuracy and cleanup.
        var createOnlyDAO = new CreateDataOnlyDAO();
        var logTarget = new LogTarget(createOnlyDAO);
        var logger = new Logging(logTarget);
        var readOnlyDAO = new ReadDataOnlyDAO();
        var deleteDataDAO = new DeleteDataOnlyDAO(); 

        // Act
        timer.Start();
        var createLogResponse = await logger.CreateLog(createOnlyDAO, testLogLevel, persistentDataStoreLogCategory, testLogMessage);
        var readSql = $"SELECT * from Logs WHERE LogId='{createLogResponse.Output.ElementAt(LOG_ID_INDEX)}'";
        var readLogResponse = await readOnlyDAO.ReadData(readSql, 1);
        timer.Stop();
        var cleanupSql = $"DELETE FROM Logs WHERE LogId='{createLogResponse.Output.ElementAt(LOG_ID_INDEX)}'";

        // Assert
        Assert.True(timer.ElapsedMilliseconds < MAX_EXECUTION_TIME_IN_SECONDS);
        Assert.True(createLogResponse.HasError == false);
        foreach (List<Object> readLogResponseData in readLogResponse.Output)
        {
            Assert.True(readLogResponseData[3].ToString() == persistentDataStoreLogCategory);
        }
        
        // Cleanup
        var deleteLogResponse = await deleteDataDAO.DeleteData(cleanupSql);

        var logTransaction = new LogTransaction();
        await logTransaction.DeleteDataAccessTransactionLog(createLogResponse.LogId);
        await logTransaction.DeleteDataAccessTransactionLog(readLogResponse.LogId);
        await logTransaction.DeleteDataAccessTransactionLog(deleteLogResponse.LogId);
    }
    [Fact]
    public async void LoggingShouldNot_CreateALogWithInvalidCategory()
    {
        // Arrange
        string testLogLevel = "Info";
        string invalidLogCategory = "Carrot";
        string? testLogMessage = null; 
        var createOnlyDAO = new CreateDataOnlyDAO();
        var logTarget = new LogTarget(createOnlyDAO);
        var logger = new Logging(logTarget);


        Stopwatch timer = new Stopwatch();

        // Act
        timer.Start();
        var invalidResponse = await logger.CreateLog(createOnlyDAO, testLogLevel, invalidLogCategory, testLogMessage);
        timer.Stop();

        // Assert
        Assert.True(timer.ElapsedMilliseconds < MAX_EXECUTION_TIME_IN_SECONDS);
        Assert.True(invalidResponse.HasError == true);
    }

    // Message Testing

    [Fact]
    public void LoggingShould_CreateALogWithAValidMessage()
    {
        // TODO implement
    }

    [Fact]
    public async void LoggingShouldNot_CreateALogWithAnInvalidMessage()
    {
        // Arrange
        string testLogLevel = "Info";
        string testLogCategory = "View";
        // For now, max is 2000 characters
        string? invalidLogMessage = "Lorem ipsum dolor sit amet, consectetur adipiscing elit." +
        "Sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam," +
        "quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute" +
        "irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur." +
        "Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim" +
        "id est laborum. Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed do eiusmod tempor" +
        "incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation" +
        "ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit" +
        "in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat" + 
        "non proident, sunt in culpa qui officia deserunt mollit anim id est laborum. Lorem ipsum dolor sit " +
        "amet, consectetur adipiscing elit. Sed do eiusmod tempor incididunt ut labore et dolore magna aliqua." + 
        "Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat." +
        "Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur." +
        "Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum." +
        "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed do eiusmod tempor incididunt ut labore et dolore magna aliqua." +
        "Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat." +
        "Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur." +
        "Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum."+
        "This last line is here to make extra sure that this message is over the 2000 character limit!" + 
        "I think that My string is still slightly too short, so I am again adding another line to ensure I am over the limit." +
        "This is quite funny to me, adding more characters. The line above only brought us to 1976 characters. This one will do it.";

        var createOnlyDAO = new CreateDataOnlyDAO();
        var logTarget = new LogTarget(createOnlyDAO);
        var logger = new Logging(logTarget);

        Stopwatch timer = new Stopwatch();

        // Act
        timer.Start();
        var invalidMessageResponse = await logger.CreateLog(createOnlyDAO, testLogLevel, testLogCategory, invalidLogMessage);
        timer.Stop();

        // Assert
        Assert.True(timer.ElapsedMilliseconds < MAX_EXECUTION_TIME_IN_SECONDS);
        Assert.True(invalidMessageResponse.HasError == true);
    }
}
