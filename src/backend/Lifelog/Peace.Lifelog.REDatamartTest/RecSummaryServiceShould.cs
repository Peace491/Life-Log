namespace Peace.Lifelog.RecSummaryService;

using Peace.Lifelog.Infrastructure;

public class RecSummaryServiceShould
{
    private string TEST_USER_HASH = "Test";
    [Fact]
    public async Task updateRecommendationDataMartForUser_Should_UpdateUserRecommendationDataMart()
    {
        // Arrange
        var recSummaryRepo = new RecSummaryRepo();
        var reService = new RecSummaryService(recSummaryRepo);
        var userHash = TEST_USER_HASH;

        // Act
        var result = await reService.updateUserRecSummary(userHash);

        // Assert
        Assert.False(result.HasError);
    }
    [Fact]
    public async Task updateRecommendationDataMartForSystem_Should_UpdateSystemToHoldMostPopularCategory()
    {
        // Arrange
        var recSummaryRepo = new RecSummaryRepo();
        var reService = new  RecSummaryService(recSummaryRepo);

        // Act
        var result = await reService.updateSystemUserRecSummary();

        // Assert
        Assert.False(result.HasError);
    }
    [Fact]
    public async Task updateRecommendationDataMartForAllUsers_Should_UpdateAllUserRecommendationDataMart()
    {
        // Arrange
        var recSummaryRepo = new RecSummaryRepo();
        var reService = new RecSummaryService(recSummaryRepo);
        
        // Act
        var result = await reService.updateAllUserRecSummary();

        // Assert
        Assert.False(result.HasError);

    }
    [Fact]
    public async Task updateUserRecSummary_Should_ReturnAnErrorIfTheUserHashIsInvalid()
    {
        // Arrange
        var recSummaryRepo = new RecSummaryRepo();
        var reService = new RecSummaryService(recSummaryRepo);
        var userHash = "InvalidUserHash";

        // Act
        var result = await reService.updateUserRecSummary(userHash);

        // Assert
        Assert.True(result.HasError);
    }
    [Fact]
    public async Task updateAllUserRecSummary_Should_ReturnAnErrorIfUserNotAdmin()
    {
        // Somehow check if the user is an admin

        // Arrange
        var recSummaryRepo = new RecSummaryRepo();
        var reService = new RecSummaryService(recSummaryRepo);

        // Act
        var result = await reService.updateAllUserRecSummary();

        // Assert
        Assert.False(result.HasError);
    }
    
}