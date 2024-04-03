namespace Peace.Lifelog.RecEngineTest;

using Peace.Lifelog.Infrastructure;
using Peace.Lifelog.DataAccess;
using Peace.Lifelog.Logging;
using Peace.Lifelog.RecEngineService;
using Peace.Lifelog.Security;
public class ReServiceShould
{
    private const string USER_HASH = "System";
    
    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    [InlineData(6)]
    [InlineData(7)]
    [InlineData(8)]
    [InlineData(9)]
    [InlineData(10)]
    public async Task REServiceGetNumRecs_Should_GetTheNumberOfRecomendationsItIsPassed(int numRecs)
    {

        var principal = new AppPrincipal();
        principal.UserId = USER_HASH;
        principal.Claims = new Dictionary<string, string>() {{"Role", "Normal"}};
        // Arrange
        CreateDataOnlyDAO createDataOnlyDAO = new CreateDataOnlyDAO();
        ReadDataOnlyDAO readDataOnlyDAO = new ReadDataOnlyDAO();
        LogTarget logTarget = new LogTarget(createOnlyDAO: createDataOnlyDAO);
        Logging logger = new Logging(logTarget: logTarget);
        LifelogAuthService lifelogAuthService = new LifelogAuthService();

        // need to setup a user every single time this test is run 
        var recEngineRepo = new RecEngineRepo(readDataOnlyDAO);
        var recEngineService = new RecEngineService(recEngineRepo, logger, lifelogAuthService);

        // Act
        var response = await recEngineService.getNumRecs(principal, numRecs);

        // Assert
        Assert.NotNull(response);
        Assert.NotNull(response.Output);
        
        Assert.True(response.Output!.Count == numRecs);
        Assert.False(response.HasError);
    }
    // [Fact]
    // public async Task REServiceGetNumRecs_Should_ReturnAnErrorIfTheUserHashIsInvalid()
    // {
    //     // TODO: make principal with invalid user hash

    //     // Arrange
    //     CreateDataOnlyDAO createDataOnlyDAO = new CreateDataOnlyDAO();
    //     ReadDataOnlyDAO readDataOnlyDAO = new ReadDataOnlyDAO();
    //     LogTarget logTarget = new LogTarget(createOnlyDAO: createDataOnlyDAO);
    //     Logging logger = new Logging(logTarget: logTarget);
    //     LifelogAuthService lifelogAuthService = new LifelogAuthService();

    //     // need to setup a user every single time this test is run 
    //     var recEngineRepo = new RecEngineRepo(readDataOnlyDAO);
    //     var recEngineService = new RecEngineService(recEngineRepo, logger, lifelogAuthService);
    //     int numRecs = 5;

    //     // Act
    //     // var response = await recEngineService.getNumRecs(, numRecs);

    //     // Assert
    //     // Assert.NotNull(response);
    //     // Assert.NotNull(response.Output);

    //     // Assert.True(response.HasError);
    // }
    // [Fact]
    // public async Task REServiceGetNumRecs_Should_ReturnAnErrorIfTheNumberOfRecomendationsIsLessThan1()
    // {
    //     // Arrange
    //     CreateDataOnlyDAO createDataOnlyDAO = new CreateDataOnlyDAO();
    //     ReadDataOnlyDAO readDataOnlyDAO = new ReadDataOnlyDAO();
    //     LogTarget logTarget = new LogTarget(createOnlyDAO: createDataOnlyDAO);
    //     Logging logger = new Logging(logTarget: logTarget);
    //     LifelogAuthService lifelogAuthService = new LifelogAuthService();

    //     // need to setup a user every single time this test is run 
    //     var recEngineRepo = new RecEngineRepo(readDataOnlyDAO);
    //     var recEngineService = new RecEngineService(recEngineRepo, logger, lifelogAuthService);
    //     int numRecs = -1;

    //     // Act
    //     var response = await recEngineService.getNumRecs(, numRecs);

    //     // Assert
    //     Assert.NotNull(response);
    //     Assert.NotNull(response.Output);

    //     Assert.True(response.HasError);
    // }
    // [Fact]
    // public async Task REServiceGetNumRecs_Should_ReturnAnErrorIfTheNumberOfRecomendationsIsGreaterThan10()
    // {
    //     // Arrange
    //     CreateDataOnlyDAO createDataOnlyDAO = new CreateDataOnlyDAO();
    //     ReadDataOnlyDAO readDataOnlyDAO = new ReadDataOnlyDAO();
    //     LogTarget logTarget = new LogTarget(createOnlyDAO: createDataOnlyDAO);
    //     Logging logger = new Logging(logTarget: logTarget);

    //     // need to setup a user every single time this test is run 
    //     var recEngineRepo = new RecEngineRepo(readDataOnlyDAO);
    //     var recEngineService = new RecEngineService(recEngineRepo, logger);
    //     int numRecs = 11;

    //     // Act
    //     var response = await recEngineService.getNumRecs(TEST_USER_HASH, numRecs);

    //     // Assert
    //     Assert.NotNull(response);
    //     Assert.NotNull(response.Output);

    //     Assert.True(response.HasError);
    // }
    // [Fact]
    // public async Task REServiceGetNumRecs_Should_RecommendLLIWithTitlesInValidLengthRange()
    // {
    //     // Arrange
    //     CreateDataOnlyDAO createDataOnlyDAO = new CreateDataOnlyDAO();
    //     ReadDataOnlyDAO readDataOnlyDAO = new ReadDataOnlyDAO();
    //     LogTarget logTarget = new LogTarget(createOnlyDAO: createDataOnlyDAO);
    //     Logging logger = new Logging(logTarget: logTarget);

    //     // need to setup a user every single time this test is run 
    //     var recEngineRepo = new RecEngineRepo(readDataOnlyDAO);
    //     var recEngineService = new RecEngineService(recEngineRepo, logger);
    //     int numRecs = 5;

    //     // Act
    //     var response = await recEngineService.getNumRecs(TEST_USER_HASH, numRecs);

    //     // Assert
    //     Assert.NotNull(response);
    //     Assert.NotNull(response.Output);
    //     foreach (var lli in response.Output)
    //     {
    //         // Assert.True(lli.Title.Length >= 1 && lli.Title.Length <= 50);
    //     }
        
    // }
    // [Fact]
    // public async Task REServiceGetNumRecs_ShouldNot_RecommendLLISetAsCompleteByUserWithinTheYear()
    // {
    //     // To test this, creating a temp table of LLI created by a test user. 
    //     // This test will insert a record in the temp table and then call the getNumRecs method
    //     // The method should not return the LLI that was inserted in the temp table, as it will be invalid


    //     // Add 'await' before the method call
        
    // }
    // [Fact]
    // public async Task REServiceGetNumRecs_Should_RecommendLLIWithValidCategories()
    // {
    //     // To test this, creating a temp table of LLI created by a test user.
    //     // This test will insert a record in the temp table and then call the getNumRecs method

    // }
    // [Fact]
    // public async Task REServiceGetNumRecs_Should_RecommendLLIThatExistOnDB()
    // {
    //     // pull recs, and then check that they exist

    // }
    // [Fact]
    // public async Task REServiceGetNumRec5_Should_RecommendLLIFollowingBizRules()
    // {
    //     // check categories of LLI
    //     // two LLI with category 1
    //     // one LLI with category 2
    //     // one LLI with most common public category (system category 1)
    //     // one LLI of none of these categories

    // }
    // [Fact]
    // public async Task REServiceGetNumRecs_Should_LogOnSuccess()
    // {
    //     // one more log in db after test
        
    // }
}