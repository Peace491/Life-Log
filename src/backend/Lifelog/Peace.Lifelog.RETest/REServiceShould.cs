namespace Peace.Lifelog.RETest;
using Peace.Lifelog.RE; // Add this line to import the 'ReService' class
using Peace.Lifelog.DataAccess;
public class ReServiceShould
{
    private const string TEST_USER_HASH = "3\u002B/ZXoeqkYQ9JTJ6vcdAfjl667hgcMxQ\u002BSBLqmVDBuY=";
    [Fact]
    public async void REServiceGetNumRecs_Should_GetTheNumberOfRecomendationsItIsPassed()
    {
        // Arrange
        var recomendationEngineRepository = new RecomendationEngineRepository();
        var reService = new REService(recomendationEngineRepository);
        int numRecs = 5;

        // Act
        var response = await reService.getNumRecs(TEST_USER_HASH, numRecs);

        // Assert
        Assert.NotNull(response.Output);
        Assert.True(response.Output!.Count == numRecs);
        Assert.False(response.HasError);
    }
    [Fact]
    public async void REServiceGetNumRecs_Should_ReturnAnErrorIfTheUserHashIsInvalid()
    {
        // Arrange
        var recomendationEngineRepository = new RecomendationEngineRepository();
        var reService = new REService(recomendationEngineRepository);
        int numRecs = 5;

        // Act
        var response = await reService.getNumRecs("InvalidUserHash", numRecs);

        // Assert
        Assert.True(response.HasError);
    }
    [Fact]
    public async void REServiceGetNumRecs_Should_ReturnAnErrorIfTheNumberOfRecomendationsIsLessThan1()
    {
        // Arrange
        var recomendationEngineRepository = new RecomendationEngineRepository();
        var reService = new REService(recomendationEngineRepository);
        int numRecs = -1;

        // Act
        var response = await reService.getNumRecs(TEST_USER_HASH, numRecs);

        // Assert
        Assert.True(response.HasError);
    }
    [Fact]
    public async void REServiceGetNumRecs_Should_ReturnAnErrorIfTheNumberOfRecomendationsIsGreaterThan10()
    {
        // Arrange
        var recomendationEngineRepository = new RecomendationEngineRepository();
        var reService = new REService(recomendationEngineRepository);
        int numRecs = 11;

        // Act
        var response = await reService.getNumRecs(TEST_USER_HASH, numRecs);

        // Assert
        Assert.True(response.HasError);
    }
    [Fact]
    public async void REServiceGetNumRecs_Should_RecommendLLIWithTitlesInValidLengthRange()
    {
        // Arrange
        var recomendationEngineRepository = new RecomendationEngineRepository();
        var reService = new REService(recomendationEngineRepository);
        int numRecs = 5;

        // Act
        var response = await reService.getNumRecs(TEST_USER_HASH, numRecs);

        // Assert
        foreach (var lli in response.Output)
        {
            // Assert.True(lli.Title.Length >= 1 && lli.Title.Length <= 50);
        }
    }
    [Fact]
    public async void REServiceGetNumRecs_ShouldNot_RecommendLLISetAsCompleteByUserWithinTheYear()
    {
        // Add 'await' before the method call
    }
    [Fact]
    public async void REServiceGetNumRecs_Should_RecommendLLIWithValidCategories()
    {

    }
    [Fact]
    public async void REServiceGetNumRecs_Should_RecommendLLIThatExistOnDB()
    {

    }
    [Fact]
    public async void REServiceGetNumRec5_Should_RecommendLLIFollowingBizRules()
    {
        // two LLI with category 1
        // one LLI with category 2
        // one LLI with most common public category (system category 1)
        // one LLI of none of these categories

    }
    [Fact]
    public async void REServiceGetNumRecs_Should_LogOnSuccess()
    {
        
    }
    [Fact]
    public async void updateRecommendationDataMartForUser_Should_UpdateUserRecommendationDataMart()
    {
        // Arrange
        var recomendationEngineRepository = new RecomendationEngineRepository();
        var reService = new REService(recomendationEngineRepository);
        var userHash = "3KWmzw9cIdwcPD9k8V9MV3yFtLdcEjw2gtPtcruKW6Y=";

        // Act
        var result = await reService.updateRecommendationDataMartForUser(userHash);

        // Assert
        Assert.False(result.HasError);
    }
    [Fact]
    public async void updateRecommendationDataMartForSystem_Should_UpdateSystemToHoldMostPopularCategory()
    {
        // Arrange
        var recomendationEngineRepository = new RecomendationEngineRepository();
        var reService = new REService(recomendationEngineRepository);

        // Act
        var result = await reService.updateRecommendationDataMartForSystem();

        // Assert
        Assert.False(result.HasError);
    }
    [Fact]
    public async void updateRecommendationDataMartForAllUsers_Should_UpdateAllUserRecommendationDataMart()
    {
        // Arrange
        var recomendationEngineRepository = new RecomendationEngineRepository();
        var reService = new REService(recomendationEngineRepository);
        

        // Act
        var result = await reService.updateRecommendationDataMartForAllUsers();

        // Assert
        Assert.False(result.HasError);

    }
}