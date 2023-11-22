namespace Peace.Lifelog.LogServiceTest;

using DomainModels;
using Peace.Lifelog.DataAccess;
using Peace.Lifelog.Logging;

using System.Diagnostics;
public class LoggingShould
{

    // Level Testing

    [Fact]
    public void LoggingShould_CreateAnInfoLog()
    {
        // Arrange

        // Act

        // Assert
    }
    [Fact]
    public void LoggingShould_CreateADebugLog()
    {
        // TODO implement
    }
    [Fact]
    public void LoggingShould_CreateAWarningLog()
    {
        // TODO implement
    }
    [Fact]
    public void LoggingShould_CreateAnErrorLog()
    {
        // TODO implement
    }
    [Fact]
    public void LoggingShouldNot_CreateALogWithInvalidLevel()
    {
        // TODO implement 
    }

    // Category Testing

    [Fact]
    public void LoggingShould_CreateAViewLog()
    {
        // TODO implement
    }
    [Fact]
    public void LoggingShould_CreateABuisnessLog()
    {
        // TODO implement
    }
    [Fact]
    public void LoggingShould_CreateAServerLog()
    {
        // TODO implement
    }
    [Fact]
    public void LoggingShould_CreateADataLog()
    {
        // TODO implement
    }
    [Fact]
    public void LoggingShould_CreateAPersistentDataStoreLog()
    {
        // TODO implement
    }
    [Fact]
    public void LoggingShouldNot_CreateALogWithInvalidCategory()
    {
        // TODO implement 
    }

    // Message Testing

    [Fact]
    public void LoggingShould_CreateALogWithAValidMessage()
    {
        // TODO implement
    }
    [Fact]
    public void LoggingShouldNot_CreateALogWithAnInvalidMessage()
    {
        // TODO implement
    }
}
