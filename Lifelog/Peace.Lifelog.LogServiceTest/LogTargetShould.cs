namespace Peace.Lifelog.LogServiceTest;

using DomainModels;
using Peace.Lifelog.DataAccess;
using Peace.Lifelog.Logging;

using System.Diagnostics;

public class LogTargetShould : IDisposable
{
    private const int LOG_ID_INDEX = 0;
    private const string TABLE = "MockLogTarget";

    private const string TEST_HASH = "TxT3KzlpTG0ExziT6GhXfJDStrAssjrEZjbe14UBfvU=";

    // Setup for all test
    public LogTargetShould()
    {
        var DDLTransactionDAO = new DDLTransactionDAO();

        var createMockTableSql = $"CREATE TABLE {TABLE} ("
            + "LogID INT PRIMARY KEY AUTO_INCREMENT,"
            + "LogTimestamp TIMESTAMP,"
            + "LogUserHash VARCHAR(255),"
            + "LogLevel VARCHAR(255),"
            + "LogCategory VARCHAR(255),"
            + "LogMessage TEXT"
        + ");";

        var createImmutableTriggerSql = "CREATE TRIGGER prevent_mock_log_updates_trigger "
            + $"BEFORE UPDATE ON {TABLE} "
            + "FOR EACH ROW "
            + "BEGIN "
            + "    SIGNAL SQLSTATE '45000' "
            + $"    SET MESSAGE_TEXT = 'Updates to the {TABLE} table are not allowed.'; "
            + "END;";

        var test1 = DDLTransactionDAO.ExecuteDDLCommand(createMockTableSql);
        var test = DDLTransactionDAO.ExecuteDDLCommand(createImmutableTriggerSql);

    }

    // Cleanup for all tests
    public async void Dispose()
    {
        var DDLTransactionDAO = new DDLTransactionDAO();

        var deleteMockTableSql = $"DROP TABLE {TABLE}";

        await DDLTransactionDAO.ExecuteDDLCommand(deleteMockTableSql);
    }

    [Fact]
    public async void LogTargetShould_CreateALogInDataStore()
    {
        // Arrange
        var createOnlyDAO = new CreateDataOnlyDAO();
        var logTarget = new LogTarget(createOnlyDAO);
        int FIRSTLISTITEM = LOG_ID_INDEX; 

        // DAO needed for test
        
        var readOnlyDAO = new ReadDataOnlyDAO();
        var deleteDataDAO = new DeleteDataOnlyDAO();

        string testLogLevel = "Debug";
        string testLogCategory = "Persistent Data Store";
        string testLogMessage = "Test Log Creation";

        var logCountSql = $"SELECT COUNT(*) FROM {TABLE}";

        // Act
        var createFirstLogResponse = await logTarget.WriteLog(TABLE, TEST_HASH, testLogLevel, testLogCategory, testLogMessage);
        var initialReadResponse = await readOnlyDAO.ReadData(logCountSql);
        var createSecondLogResponse = await logTarget.WriteLog(TABLE, TEST_HASH, testLogLevel, testLogCategory, testLogMessage);
        var finalReadResponse = await readOnlyDAO.ReadData(logCountSql);

        var deleteFirstSql = $"DELETE FROM {TABLE} Where LogId={createFirstLogResponse.Output.ElementAt(LOG_ID_INDEX)}";
        var deleteSecondSql = $"DELETE FROM {TABLE} Where LogId={createSecondLogResponse.Output.ElementAt(LOG_ID_INDEX)}";

        // Assert
        Assert.True(initialReadResponse.HasError == false);
        Assert.True(createFirstLogResponse.HasError == false);
        Assert.True(finalReadResponse.HasError == false);
        Assert.True(createSecondLogResponse.HasError == false);

        foreach (List<Object> readResponseDataOne in initialReadResponse.Output)
        {
            foreach (List<Object> readResponseDataTwo in finalReadResponse.Output)
            {
                Assert.True(Convert.ToInt32(readResponseDataOne[FIRSTLISTITEM]) < Convert.ToInt32(readResponseDataTwo[FIRSTLISTITEM]));
            }
        }

        // Cleanup
        var deleteFirstLogResponse = await deleteDataDAO.DeleteData(deleteFirstSql);
        var deleteSecondLogResponse = await deleteDataDAO.DeleteData(deleteSecondSql);   

        // Cleanup
        var logTransaction = new LogTransaction();
        await logTransaction.DeleteDataAccessTransactionLog(createFirstLogResponse.LogId);
        await logTransaction.DeleteDataAccessTransactionLog(createSecondLogResponse.LogId);
        await logTransaction.DeleteDataAccessTransactionLog(deleteFirstLogResponse.LogId);
        await logTransaction.DeleteDataAccessTransactionLog(deleteSecondLogResponse.LogId);
        await logTransaction.DeleteDataAccessTransactionLog(initialReadResponse.LogId);
        await logTransaction.DeleteDataAccessTransactionLog(finalReadResponse.LogId);
    }

    [Fact]
    public async void LogTargetShould_BeImmutable()
    {
        var createOnlyDAO = new CreateDataOnlyDAO();
        var logTarget = new LogTarget(createOnlyDAO); 
        Stopwatch timer = new Stopwatch();


        
        var updateOnlyDao = new UpdateDataOnlyDAO();
        var deleteDataDAO = new DeleteDataOnlyDAO();

        string testLogLevel = "Debug";
        string testLogCategory = "Persistent Data Store";
        string? testLogMessage = null;

        // Act
        timer.Start();
        var createLogResponse = await logTarget.WriteLog(TABLE, TEST_HASH, testLogLevel, testLogCategory, testLogMessage);
        string updateAttemptSql = $"UPDATE {TABLE} SET LogMessage = 'barn burner' WHERE LogId={createLogResponse.Output.ElementAt(LOG_ID_INDEX)}";
        var updateLogResponse = await updateOnlyDao.UpdateData(updateAttemptSql);
        timer.Stop();


        // Arrange
        Assert.True(updateLogResponse.HasError == true); 
        Assert.True(timer.Elapsed.TotalSeconds <= 3); 
        
        // Cleanup
        var cleanupSql = $"DELETE FROM {TABLE} WHERE LogId='{createLogResponse.Output.ElementAt(LOG_ID_INDEX)}'";
        var deleteLogResponse = await deleteDataDAO.DeleteData(cleanupSql);

        var logTransaction = new LogTransaction();
        await logTransaction.DeleteDataAccessTransactionLog(createLogResponse.LogId);
        await logTransaction.DeleteDataAccessTransactionLog(updateLogResponse.LogId);
        await logTransaction.DeleteDataAccessTransactionLog(deleteLogResponse.LogId);
    }
}