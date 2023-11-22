namespace Peace.Lifelog.LogServiceTest;

using DomainModels;
using Peace.Lifelog.DataAccess;
using Peace.Lifelog.Logging;

using System.Diagnostics;

public class LogRepoShould
{
    [Fact]
    public async void LogTargetShould_CreateALogInDataStore()
    {
        // Arrange
        var logTarget = new LogTarget();
        int FIRSTLISTITEM = 0; 

        // DAO needed for test
        var createOnlyDAO = new CreateDataOnlyDAO();
        var readOnlyDAO = new ReadDataOnlyDAO();
        var deleteDataDAO = new DeleteDataOnlyDAO();

        string testLogLevel = "Debug";
        string testLogCategory = "Persistent Data Store";
        string testLogMessage = "Test Log Creation";

        var logCountSql = $"SELECT COUNT(*) FROM Logs";

        // Act
        var createFirstLogResponse = await logTarget.WriteLog(createOnlyDAO, testLogLevel, testLogCategory, testLogMessage);
        var initialReadResponse = await readOnlyDAO.ReadData(logCountSql);
        var createSecondLogResponse = await logTarget.WriteLog(createOnlyDAO, testLogLevel, testLogCategory, testLogMessage);
        var finalReadResponse = await readOnlyDAO.ReadData(logCountSql);

        var deleteFirstSql = $"DELETE FROM Logs Where '{createFirstLogResponse.LogId}";
        var deleteSecondSql = $"DELETE FROM Logs Where '{createSecondLogResponse.LogId}";

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
        await deleteDataDAO.DeleteData(deleteFirstSql);
        await deleteDataDAO.DeleteData(deleteSecondSql);   
    }
}