namespace Peace.Lifelog.RETest;
using Peace.Lifelog.RE; 
using Peace.Lifelog.DataAccess;
public class ReServiceShould
{
    private const string TEST_USER_HASH = "3\u002B/ZXoeqkYQ9JTJ6vcdAfjl667hgcMxQ\u002BSBLqmVDBuY=";
    [Fact]
    public async Task REServiceGetNumRecs_Should_GetTheNumberOfRecomendationsItIsPassed()
    {
        // Arrange

        // need to setup a user every single time this test is run 
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
}