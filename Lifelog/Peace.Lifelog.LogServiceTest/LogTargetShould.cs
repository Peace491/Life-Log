namespace Peace.Lifelog.LogServiceTest;

using DomainModels;
using Peace.Lifelog.DataAccess;
using Peace.Lifelog.Logging;

using System.Diagnostics;

public class LogTargetShould
{
    private const int LOG_ID_INDEX = 0;
    private const string TABLE = "MockLogs";

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

        var logCountSql = $"SELECT COUNT(*) FROM Logs";

        // Act
        var createFirstLogResponse = await logTarget.WriteLog(TABLE, testLogLevel, testLogCategory, testLogMessage);
        var initialReadResponse = await readOnlyDAO.ReadData(logCountSql);
        var createSecondLogResponse = await logTarget.WriteLog(TABLE, testLogLevel, testLogCategory, testLogMessage);
        var finalReadResponse = await readOnlyDAO.ReadData(logCountSql);

        var deleteFirstSql = $"DELETE FROM Logs Where LogId={createFirstLogResponse.Output.ElementAt(LOG_ID_INDEX)}";
        var deleteSecondSql = $"DELETE FROM Logs Where LogId={createSecondLogResponse.Output.ElementAt(LOG_ID_INDEX)}";

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
        var createLogResponse = await logTarget.WriteLog(TABLE, testLogLevel, testLogCategory, testLogMessage);
        string updateAttemptSql = $"UPDATE Logs SET LogMessage = 'barn burner' WHERE LogId={createLogResponse.Output.ElementAt(LOG_ID_INDEX)}";
        var updateLogResponse = await updateOnlyDao.UpdateData(updateAttemptSql);
        timer.Stop();


        // Arrange
        Assert.True(updateLogResponse.HasError == true); 
        Assert.True(timer.Elapsed.TotalSeconds <= 3); 
        
        // Cleanup
        var cleanupSql = $"DELETE FROM Logs WHERE LogId='{createLogResponse.Output.ElementAt(LOG_ID_INDEX)}'";
        var deleteLogResponse = await deleteDataDAO.DeleteData(cleanupSql);

        var logTransaction = new LogTransaction();
        await logTransaction.DeleteDataAccessTransactionLog(createLogResponse.LogId);
        await logTransaction.DeleteDataAccessTransactionLog(updateLogResponse.LogId);
        await logTransaction.DeleteDataAccessTransactionLog(deleteLogResponse.LogId);
    }
}