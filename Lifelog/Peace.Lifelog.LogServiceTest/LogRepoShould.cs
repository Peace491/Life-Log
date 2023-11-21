namespace Peace.Lifelog.LogServiceTest;

using Peace.Lifelog.DataAccess;
using Peace.Lifelog.Logging;

using System.Diagnostics;

public class LogRepoShould
{
    [Fact]
    public void LogRepoShould_CreateALogInDataStore()
    {
        // Arrange
        var timer = new Stopwatch();

        var logRepo = new LogRepo();

        // Need to initlaize all types of DAO for checking and cleanup.
        var createOnlyDAO = new CreateDataOnlyDAO();
        var readOnlyDAO = new ReadDataOnlyDAO();
        var deleteDataDAO = new DeleteDataOnlyDAO(); 

        string testLogLevel = "Debug";
        string testLogCategory = "Persistent Data Store";
        string testLogMessage = "Test Log Create";

        // Act
        timer.Start();
        var createLogResponse = logRepo.CreateLog(createOnlyDAO, testLogLevel, testLogCategory, testLogMessage);
        timer.Stop();

        var readLogResponse = logRepo.ReadLog(readOnlyDAO, testLogLevel);

        // Assert
        Assert.True(createLogResponse.HasError == false);
        foreach (List<Object> readLogResponseData in readLogResponse.Output)
        {
            // Use the MySqlDataReader
            Assert.True(readLogResponseData[2].ToString() == testLogLevel);
            Assert.True(readLogResponseData[3].ToString() == testLogCategory);
            Assert.True(readLogResponseData[4].ToString() == testLogMessage);

        }
        Assert.True(timer.Elapsed.TotalSeconds <= 3);

        // Cleanup
        logRepo.DeleteLog(deleteDataDAO, testLogLevel);
    }

    [Fact]
    public void LogRepoShould_ReadALogInDataStore()
    {
        // Arrange
        var timer = new Stopwatch();

        var logRepo = new LogRepo();

        // Need to initlaize all types of DAO for checking and cleanup.
        var createOnlyDAO = new CreateDataOnlyDAO();
        var readOnlyDAO = new ReadDataOnlyDAO();
        var deleteDataDAO = new DeleteDataOnlyDAO(); 

        string testLogLevel = "Debug";
        string testLogCategory = "Persistent Data Store";
        string testLogMessage = "Test Log Read";

        //Act
        logRepo.CreateLog(createOnlyDAO, testLogLevel, testLogCategory, testLogMessage);
        
        timer.Start();
        var readLogResponse = logRepo.ReadLog(readOnlyDAO, testLogLevel);
        timer.Stop();

        //Assert
        Assert.True(readLogResponse.HasError == false);
        foreach (List<Object> readLogResponseData in readLogResponse.Output)
        {
            // Use the MySqlDataReader
            Assert.True(readLogResponseData[2].ToString() == testLogLevel);
            Assert.True(readLogResponseData[3].ToString() == testLogCategory);
            Assert.True(readLogResponseData[4].ToString() == testLogMessage);

        }
        Assert.True(timer.Elapsed.TotalSeconds <= 3);

        // Cleanup
        logRepo.DeleteLog(deleteDataDAO, testLogLevel);
    }

    [Fact]
    public void LogRepoShould_DeleteALogInDataStore()
    {
        // Arrange
        var timer = new Stopwatch();

        var logRepo = new LogRepo();

        // Need to initlaize types of DAO for checking and cleanup.
        var createOnlyDAO = new CreateDataOnlyDAO();
        var deleteDataDAO = new DeleteDataOnlyDAO(); 

        string testLogLevel = "Debug";
        string testLogCategory = "Persistent Data Store";
        string testLogMessage = "Test Log Read";

        //Act
        logRepo.CreateLog(createOnlyDAO, testLogLevel, testLogCategory, testLogMessage);
        
        timer.Start();
        var deleteLogResponse = logRepo.DeleteLog(deleteDataDAO, testLogLevel);
        timer.Stop();


        //Assert
        Assert.True(deleteLogResponse.HasError == false);
        Assert.True(timer.Elapsed.TotalSeconds <= 3);
        
    }
}