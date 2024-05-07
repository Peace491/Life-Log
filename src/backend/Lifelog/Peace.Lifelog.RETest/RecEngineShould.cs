namespace Peace.Lifelog.RecEngineTest;

using Peace.Lifelog.Infrastructure;
using Peace.Lifelog.DataAccess;
using Peace.Lifelog.Logging;
using Peace.Lifelog.RecEngineService;
using Peace.Lifelog.Security;

public class ReServiceShould
{
    private string USER_HASH = "TestUser";
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
        LogTarget logTarget = new LogTarget(createOnlyDAO: createDataOnlyDAO, readDataOnlyDAO);
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
        LogTarget logTarget = new LogTarget(createOnlyDAO: createDataOnlyDAO, readDataOnlyDAO);
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
        LogTarget logTarget = new LogTarget(createOnlyDAO: createDataOnlyDAO, readDataOnlyDAO);
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
        LogTarget logTarget = new LogTarget(createOnlyDAO: createDataOnlyDAO, readDataOnlyDAO);
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
}