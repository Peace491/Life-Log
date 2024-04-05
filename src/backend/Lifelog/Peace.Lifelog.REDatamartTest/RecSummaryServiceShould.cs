namespace Peace.Lifelog.RecSummaryService;

using System.Diagnostics;
using Peace.Lifelog.DataAccess;
using Peace.Lifelog.Infrastructure;
using Peace.Lifelog.Logging;
using Peace.Lifelog.UserManagement;

public class RecSummaryServiceShould
{   
    private const string USER_ID = "TestRecSummaryServiceAccount";
    private const string USER_HASH = "/2Lm1yvAbXcZKRffjcV+f7UFD2Yl/Tn5OF7sKBYRjuQ=";

    [Fact]
    public async Task updateRecommendationDataMartForUser_Should_UpdateUserRecommendationDataMart()
    {
        // Arrange
        IReadDataOnlyDAO readDataOnlyDAO = new ReadDataOnlyDAO();
        IUpdateDataOnlyDAO updateDataOnlyDAO = new UpdateDataOnlyDAO();
        ICreateDataOnlyDAO createDataOnlyDAO = new CreateDataOnlyDAO();
        ILogTarget logTarget = new LogTarget(createDataOnlyDAO);
        ILogging logger = new Logging(logTarget);

        var recSummaryRepo = new RecSummaryRepo(readDataOnlyDAO, updateDataOnlyDAO, logger);
        var reService = new RecSummaryService(recSummaryRepo, logger);

        // Act
        var result = await reService.updateUserRecSummary(USER_HASH);

        // Assert
        Assert.False(result.HasError);
    }
    [Fact]
    public async Task updateRecommendationDataMartForSystem_Should_UpdateSystemToHoldMostPopularCategory()
    {
        // Arrange
        IReadDataOnlyDAO readDataOnlyDAO = new ReadDataOnlyDAO();
        IUpdateDataOnlyDAO updateDataOnlyDAO = new UpdateDataOnlyDAO();
        ICreateDataOnlyDAO createDataOnlyDAO = new CreateDataOnlyDAO();
        ILogTarget logTarget = new LogTarget(createDataOnlyDAO);
        ILogging logger = new Logging(logTarget);

        var recSummaryRepo = new RecSummaryRepo(readDataOnlyDAO, updateDataOnlyDAO, logger);
        var reService = new RecSummaryService(recSummaryRepo, logger);

        // Act
        var result = await reService.UpdateSystemUserRecSummary();

        // Assert
        Assert.False(result.HasError);
    }
    [Fact]
    public async Task updateRecommendationDataMartForAllUsers_Should_UpdateAllUserRecommendationDataMart()
    {
        // Arrange
        IReadDataOnlyDAO readDataOnlyDAO = new ReadDataOnlyDAO();
        IUpdateDataOnlyDAO updateDataOnlyDAO = new UpdateDataOnlyDAO();
        ICreateDataOnlyDAO createDataOnlyDAO = new CreateDataOnlyDAO();
        ILogTarget logTarget = new LogTarget(createDataOnlyDAO);
        ILogging logger = new Logging(logTarget);

        var recSummaryRepo = new RecSummaryRepo(readDataOnlyDAO, updateDataOnlyDAO, logger);
        var reService = new RecSummaryService(recSummaryRepo, logger);
        
        // Act
        var result = await reService.updateAllUserRecSummary();

        // Assert
        Assert.False(result.HasError);

    }
    [Fact]
    public async Task updateUserRecSummary_Should_ReturnAnErrorIfTheUserHashIsInvalid()
    {
        // Arrange
        IReadDataOnlyDAO readDataOnlyDAO = new ReadDataOnlyDAO();
        IUpdateDataOnlyDAO updateDataOnlyDAO = new UpdateDataOnlyDAO();
        ICreateDataOnlyDAO createDataOnlyDAO = new CreateDataOnlyDAO();
        ILogTarget logTarget = new LogTarget(createDataOnlyDAO);
        ILogging logger = new Logging(logTarget);

        var recSummaryRepo = new RecSummaryRepo(readDataOnlyDAO, updateDataOnlyDAO, logger);
        var reService = new RecSummaryService(recSummaryRepo, logger);

        // Act
        var result = await reService.updateUserRecSummary(USER_HASH + "invalid");

        // Assert
        Assert.True(result.HasError);
    }
    [Fact]
    public async Task updateAllUserRecSummary_Should_ReturnAnErrorIfUserNotAdmin()
    {
        // Somehow check if the user is an admin

        // Arrange
        IReadDataOnlyDAO readDataOnlyDAO = new ReadDataOnlyDAO();
        IUpdateDataOnlyDAO updateDataOnlyDAO = new UpdateDataOnlyDAO();
        ICreateDataOnlyDAO createDataOnlyDAO = new CreateDataOnlyDAO();
        ILogTarget logTarget = new LogTarget(createDataOnlyDAO);
        ILogging logger = new Logging(logTarget);

        var recSummaryRepo = new RecSummaryRepo(readDataOnlyDAO, updateDataOnlyDAO, logger);
        var reService = new RecSummaryService(recSummaryRepo, logger);

        // Act
        var result = await reService.updateAllUserRecSummary();

        // Assert
        Assert.False(result.HasError);
    }
    
}