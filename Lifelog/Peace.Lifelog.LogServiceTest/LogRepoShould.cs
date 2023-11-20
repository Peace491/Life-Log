namespace Peace.Lifelog.LogServiceTest;

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

        string testLogLevel = "Debug";
        string testLogCategory = "Persistent Data Store";
        string testLogMessage = "Test Log Create";

        // Act
        timer.Start();
        var createLogResponse = logRepo.CreateLog(testLogLevel, testLogCategory, testLogMessage);
        timer.Stop();

        var readLogResponse = logRepo.ReadLog(testLogLevel);

        // Assert
        Assert.True(createLogResponse.HasError == false);
        foreach (List<Object> readLogResponseData in readLogResponse.Output)
        {
            // Use the MySqlDataReader
            Assert.True(readLogResponseData[1].ToString() == testLogLevel);
            Assert.True(readLogResponseData[2].ToString() == testLogCategory);
            Assert.True(readLogResponseData[3].ToString() == testLogMessage);

        }
        Assert.True(timer.Elapsed.TotalSeconds <= 3);

        // Cleanup
        logRepo.DeleteLog(testLogLevel);
    }

    [Fact]
    public void LogRepoShould_ReadALogInDataStore()
    {
        // Arrange
        var timer = new Stopwatch();

        var logRepo = new LogRepo();

        string testLogLevel = "Debug";
        string testLogCategory = "Persistent Data Store";
        string testLogMessage = "Test Log Read";

        //Act
        logRepo.CreateLog(testLogLevel, testLogCategory, testLogMessage);
        
        timer.Start();
        var readLogResponse = logRepo.ReadLog(testLogLevel);
        timer.Stop();

        //Assert
        Assert.True(readLogResponse.HasError == false);
        foreach (List<Object> readLogResponseData in readLogResponse.Output)
        {
            // Use the MySqlDataReader
            Assert.True(readLogResponseData[1].ToString() == testLogLevel);
            Assert.True(readLogResponseData[2].ToString() == testLogCategory);
            Assert.True(readLogResponseData[3].ToString() == testLogMessage);

        }
        Assert.True(timer.Elapsed.TotalSeconds <= 3);

        // Cleanup
        logRepo.DeleteLog(testLogLevel);
    }

    [Fact]
    public void LogRepoShould_DeleteALogInDataStore()
    {
        // Arrange
        var timer = new Stopwatch();

        var logRepo = new LogRepo();

        string testLogLevel = "Debug";
        string testLogCategory = "Persistent Data Store";
        string testLogMessage = "Test Log Read";

        //Act
        logRepo.CreateLog(testLogLevel, testLogCategory, testLogMessage);
        
        timer.Start();
        var deleteLogResponse = logRepo.DeleteLog(testLogLevel);
        timer.Stop();


        //Assert
        Assert.True(deleteLogResponse.HasError == false);
        Assert.True(timer.Elapsed.TotalSeconds <= 3);
        
    }
}