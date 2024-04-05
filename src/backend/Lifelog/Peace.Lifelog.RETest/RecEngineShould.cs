namespace Peace.Lifelog.RecEngineTest;

using Peace.Lifelog.Infrastructure;
using Peace.Lifelog.DataAccess;
using Peace.Lifelog.Logging;
using Peace.Lifelog.RecEngineService;
using Peace.Lifelog.Security;
using Peace.Lifelog.LLI;
using Peace.Lifelog.UserManagement;
using System.Diagnostics;

public class ReServiceShould
{
    private string USER_HASH = "System";
    private const string ROLE = "Normal";
    
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
    public async Task REServiceRecNumLLI_Should_GetTheNumberOfRecomendationsItIsPassed(int numRecs)
    {
        var principal = new AppPrincipal();
        principal.UserId = USER_HASH;
        principal.Claims = new Dictionary<string, string>() {{"Role", ROLE}};
        // Arrange
        CreateDataOnlyDAO createDataOnlyDAO = new CreateDataOnlyDAO();
        ReadDataOnlyDAO readDataOnlyDAO = new ReadDataOnlyDAO();
        LogTarget logTarget = new LogTarget(createOnlyDAO: createDataOnlyDAO);
        Logging logger = new Logging(logTarget: logTarget);
        LifelogAuthService lifelogAuthService = new LifelogAuthService();

        // need to setup a user every single time this test is run 
        var recEngineRepo = new RecEngineRepo(readDataOnlyDAO, logger);
        var recEngineService = new RecEngineService(recEngineRepo, logger, lifelogAuthService);

        // Act
        var response = await recEngineService.RecNumLLI(principal, numRecs);

        // Assert
        Assert.NotNull(response);
        Assert.NotNull(response.Output);
        
        Assert.True(response.Output!.Count == numRecs);
        Assert.False(response.HasError);
    }
    [Fact]
    public async Task REServiceRecNumLLI_Should_ReturnAnErrorIfTheUserClaimsAreInvalid()
    {
        // TODO: make principal with invalid user hash
        var principal = new AppPrincipal();
        principal.UserId = USER_HASH;
        principal.Claims = new Dictionary<string, string>() {{"Claim", "Potato"}};
        // Arrange
        CreateDataOnlyDAO createDataOnlyDAO = new CreateDataOnlyDAO();
        ReadDataOnlyDAO readDataOnlyDAO = new ReadDataOnlyDAO();
        LogTarget logTarget = new LogTarget(createOnlyDAO: createDataOnlyDAO);
        Logging logger = new Logging(logTarget: logTarget);
        LifelogAuthService lifelogAuthService = new LifelogAuthService();

        // need to setup a user every single time this test is run 
        var recEngineRepo = new RecEngineRepo(readDataOnlyDAO, logger);
        var recEngineService = new RecEngineService(recEngineRepo, logger, lifelogAuthService);
        int numRecs = 5;

        // Act
        var response = await recEngineService.RecNumLLI(principal, numRecs);

        Assert.NotNull(response);

        Assert.True(response.HasError);
    }
    [Fact]
    public async Task REServiceRecNumLLI_Should_ReturnAnErrorIfTheNumberOfRecomendationsIsLessThan1()
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
        var recEngineRepo = new RecEngineRepo(readDataOnlyDAO, logger);
        var recEngineService = new RecEngineService(recEngineRepo, logger, lifelogAuthService);
        int numRecs = -1;

        // Act
        var response = await recEngineService.RecNumLLI(principal, numRecs);

        // Assert
        Assert.NotNull(response);

        Assert.True(response.HasError);
    }
    [Fact]
    public async Task REServiceRecNumLLI_Should_ReturnAnErrorIfTheNumberOfRecomendationsIsGreaterThan10()
    {
        // Arrange
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
        var recEngineRepo = new RecEngineRepo(readDataOnlyDAO, logger);
        var recEngineService = new RecEngineService(recEngineRepo, logger, lifelogAuthService);
        int numRecs = 11;

        // Act
        var response = await recEngineService.RecNumLLI(principal, numRecs);

        // Assert
        Assert.NotNull(response);

        Assert.True(response.HasError);
    }
    [Fact]
    public async Task REServiceRecNumLLI_Should_RecommendLLIWithTitlesInValidLengthRange()
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
        var recEngineRepo = new RecEngineRepo(readDataOnlyDAO, logger);
        var recEngineService = new RecEngineService(recEngineRepo, logger, lifelogAuthService);
        int numRecs = 1;

        // Act
        var response = await recEngineService.RecNumLLI(principal, numRecs);

        // Assert
        Assert.NotNull(response);
        Assert.NotNull(response.Output);
        foreach (var lli in response.Output)
        {
            Console.WriteLine("tehe");

            // Assert.True(lliObject.Title.Length >= 1 && lliObject.Title.Length <= 50);
        }
    }
    [Fact]
    public async Task REServiceRecNumLLI_Should_RecommendLLIWithAlphanumericTitle()
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
        var recEngineRepo = new RecEngineRepo(readDataOnlyDAO, logger);
        var recEngineService = new RecEngineService(recEngineRepo, logger, lifelogAuthService);
        int numRecs = 1;

        // Act
        var response = await recEngineService.RecNumLLI(principal, numRecs);

        // Assert
        Assert.NotNull(response);
        Assert.NotNull(response.Output);
        foreach (var lli in response.Output)
        {
            Console.WriteLine("tehe");
            // check if title is alphanumeric
            // Assert.True(lliObject.Title.Length >= 1 && lliObject.Title.Length <= 50);
        }
    }
    [Fact]
    public async Task REServiceRecNumLLI_Should_RecommendLLIWithDescriptionsInValidLengthRange()
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

        var recEngineRepo = new RecEngineRepo(readDataOnlyDAO, logger);
        var recEngineService = new RecEngineService(recEngineRepo, logger, lifelogAuthService);
        int numRecs = 1;

        // Act
        var response = await recEngineService.RecNumLLI(principal, numRecs);

        // Assert
        Assert.NotNull(response);
        Assert.NotNull(response.Output);
        foreach (var lli in response.Output)
        {
            Console.WriteLine("tehe");

            // Assert.True(lliObject.Title.Length >= 1 && lliObject.Title.Length <= 50);
        }
    }

    [Fact]
    public async Task REServiceRecNumLLI_ShouldNot_RecommendLLISetAsCompleteByUserWithinTheYear()
    {
        // To test this, creating a temp table of LLI created by a test user. 
        // This test will insert a record in the temp table and then call the RecNumLLI method
        // The method should not return the LLI that was inserted in the temp table, as it will be invalid


        // Add 'await' before the method call
        
    }
    [Fact]
    public async Task REServiceRecNumLLI_Should_RecommendLLIWithValidCategories()
    {
        // To test this, creating a temp table of LLI created by a test user.
        // This test will insert a record in the temp table and then call the RecNumLLI method

    }
    [Fact]
    public async Task REServiceRecNumLLI_Should_RecommendLLIThatExistOnDB()
    {
        // pull rec, and then check that they exist

    }
    [Fact]
    public async Task REServiceGetNumRec5_Should_RecommendLLIFollowingBizRules()
    {
        // check categories of LLI
        // two LLI with category 1
        // one LLI with category 2
        // one LLI with most common public category (system category 1)
        // one LLI of none of these categories

    }
    [Fact]
    public async Task REServiceRecNumLLI_Should_LogOnSuccess()
    {
        // one more log in db after test
        
    }
}