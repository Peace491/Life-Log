namespace Peace.Lifelog.RecSummaryService;

using Peace.Lifelog.DataAccess;

public class RecSummaryServiceShould
{
    [Fact]
    public async Task updateRecommendationDataMartForUser_Should_UpdateUserRecommendationDataMart()
    {
        // Arrange
        var summaryRepository = new SummaryRepository();
        var reService = new RecSummaryService(summaryRepository);
        var userHash = "Test";

        // Act
        var result = await reService.updateUserRecSummary(userHash);

        // Assert
        Assert.False(result.HasError);
    }
    [Fact]
    public async Task updateRecommendationDataMartForSystem_Should_UpdateSystemToHoldMostPopularCategory()
    {
        // Arrange
        var summaryRepository = new SummaryRepository();
        var reService = new  RecSummaryService(summaryRepository);

        // Act
        var result = await reService.updateSystemUserRecSummary();

        // Assert
        Assert.False(result.HasError);
    }
    [Fact]
    public async Task updateRecommendationDataMartForAllUsers_Should_UpdateAllUserRecommendationDataMart()
    {
        // Arrange
        var summaryRepository = new SummaryRepository();
        var reService = new RecSummaryService(summaryRepository);
        

        // Act
        var result = await reService.updateAllUserRecSummary();

        // Assert
        Assert.False(result.HasError);

    }
    [Fact]
    public async Task updateUserRecSummary_Should_ReturnAnErrorIfTheUserHashIsInvalid()
    {
        // Arrange
        var summaryRepository = new SummaryRepository();
        var reService = new RecSummaryService(summaryRepository);
        var userHash = "InvalidUserHash";

        // Act
        var result = await reService.updateUserRecSummary(userHash);

        // Assert
        Assert.True(result.HasError);
    }
    [Fact]
    public async Task updateUserRecSummary_Should_ReturnAnErrorIfTheUserHashIsSystem()
    {
        // Arrange
        var summaryRepository = new SummaryRepository();
        var reService = new RecSummaryService(summaryRepository);
        var userHash = "System";

        // Act
        var result = await reService.updateUserRecSummary(userHash);

        // Assert
        Assert.True(result.HasError);
    }
    [Fact]
    public async Task updateAllUserRecSummary_Should_ReturnAnErrorIfUserNotAdmin()
    {
        // Arrange
        var summaryRepository = new SummaryRepository();
        var reService = new RecSummaryService(summaryRepository);

        // Act
        var result = await reService.updateAllUserRecSummary();

        // Assert
        Assert.False(result.HasError);
    }
    
}