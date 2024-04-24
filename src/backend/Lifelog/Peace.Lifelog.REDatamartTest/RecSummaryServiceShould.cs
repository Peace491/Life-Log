namespace Peace.Lifelog.RecSummaryService;

using Peace.Lifelog.DataAccess;
using Peace.Lifelog.Infrastructure;
using Peace.Lifelog.Logging;
using Peace.Lifelog.Security;

public class RecSummaryServiceShould
{   
    private const string USER_HASH = "TestUser";

    [Fact]
    public async Task updateRecommendationDataMartForUser_Should_UpdateUserRecommendationDataMart()
    {
        var principal = new AppPrincipal();
        principal.UserId = USER_HASH;
        principal.Claims = new Dictionary<string, string>() {{"Role", "Normal"}};
        // Arrange
        IReadDataOnlyDAO readDataOnlyDAO = new ReadDataOnlyDAO();
        IUpdateDataOnlyDAO updateDataOnlyDAO = new UpdateDataOnlyDAO();
        ICreateDataOnlyDAO createDataOnlyDAO = new CreateDataOnlyDAO();
        ILogTarget logTarget = new LogTarget(createDataOnlyDAO, readDataOnlyDAO);
        ILogging logger = new Logging(logTarget);

        var recSummaryRepo = new RecSummaryRepo(readDataOnlyDAO, updateDataOnlyDAO, logger);
        var reService = new RecSummaryService(recSummaryRepo, logger);

        // Act
        var result = await reService.UpdateUserRecSummary(principal);

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
        ILogTarget logTarget = new LogTarget(createDataOnlyDAO, readDataOnlyDAO);
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
        var principal = new AppPrincipal();
        principal.UserId = USER_HASH;
        principal.Claims = new Dictionary<string, string>() {{"Role", "Admin"}};
        // Arrange
        IReadDataOnlyDAO readDataOnlyDAO = new ReadDataOnlyDAO();
        IUpdateDataOnlyDAO updateDataOnlyDAO = new UpdateDataOnlyDAO();
        ICreateDataOnlyDAO createDataOnlyDAO = new CreateDataOnlyDAO();
        ILogTarget logTarget = new LogTarget(createDataOnlyDAO, readDataOnlyDAO);
        ILogging logger = new Logging(logTarget);

        var recSummaryRepo = new RecSummaryRepo(readDataOnlyDAO, updateDataOnlyDAO, logger);
        var reService = new RecSummaryService(recSummaryRepo, logger);
        
        // Act
        var result = await reService.UpdateAllUserRecSummary(principal);

        // Assert
        Assert.False(result.HasError);

    }
    [Fact]
    public async Task updateUserRecSummary_Should_ReturnAnErrorIfTheUserHashIsInvalid()
    {
        var principal = new AppPrincipal();
        principal.UserId = "nope";
        principal.Claims = new Dictionary<string, string>() {{"Role", "Normal"}};
        // Arrange
        IReadDataOnlyDAO readDataOnlyDAO = new ReadDataOnlyDAO();
        IUpdateDataOnlyDAO updateDataOnlyDAO = new UpdateDataOnlyDAO();
        ICreateDataOnlyDAO createDataOnlyDAO = new CreateDataOnlyDAO();
        ILogTarget logTarget = new LogTarget(createDataOnlyDAO, readDataOnlyDAO);
        ILogging logger = new Logging(logTarget);

        var recSummaryRepo = new RecSummaryRepo(readDataOnlyDAO, updateDataOnlyDAO, logger);
        var reService = new RecSummaryService(recSummaryRepo, logger);

        // Act
        var result = await reService.UpdateUserRecSummary(principal);

        // Assert
        Assert.True(result.HasError);
    }
    [Fact]
    public async Task updateAllUserRecSummary_Should_ReturnAnErrorIfUserNotAdmin()
    {
        var principal = new AppPrincipal();
        principal.UserId = USER_HASH;
        principal.Claims = new Dictionary<string, string>() {{"Role", "Potato"}};

        // Arrange
        IReadDataOnlyDAO readDataOnlyDAO = new ReadDataOnlyDAO();
        IUpdateDataOnlyDAO updateDataOnlyDAO = new UpdateDataOnlyDAO();
        ICreateDataOnlyDAO createDataOnlyDAO = new CreateDataOnlyDAO();
        ILogTarget logTarget = new LogTarget(createDataOnlyDAO, readDataOnlyDAO);
        ILogging logger = new Logging(logTarget);

        var recSummaryRepo = new RecSummaryRepo(readDataOnlyDAO, updateDataOnlyDAO, logger);
        var reService = new RecSummaryService(recSummaryRepo, logger);

        // Act
        var result = await reService.UpdateAllUserRecSummary(principal);

        // Assert
        Assert.True(result.HasError);
    }
    
}