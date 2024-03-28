namespace Peace.Lifelog.REDatamartTest;

using Peace.Lifelog.DataAccess;
using Peace.Lifelog.REDatamartService;
public class UnitTest1
{
    [Fact]
    public async void updateRecommendationDataMartForUser_Should_UpdateUserRecommendationDataMart()
    {
        // Arrange
        var summaryRepository = new SummaryRepository();
        var reService = new REDatamartService(summaryRepository);
        var userHash = "Test";

        // Act
        var result = await reService.updateRecommendationDataMartForUser(userHash);

        // Assert
        Assert.False(result.HasError);
    }
    [Fact]
    public async void updateRecommendationDataMartForSystem_Should_UpdateSystemToHoldMostPopularCategory()
    {
        // Arrange
        var summaryRepository = new SummaryRepository();
        var reService = new  REDatamartService(summaryRepository);

        // Act
        var result = await reService.updateRecommendationDataMartForSystem();

        // Assert
        Assert.False(result.HasError);
    }
    [Fact]
    public async void updateRecommendationDataMartForAllUsers_Should_UpdateAllUserRecommendationDataMart()
    {
        // Arrange
        var summaryRepository = new SummaryRepository();
        var reService = new  REDatamartService(summaryRepository);
        

        // Act
        var result = await reService.updateRecommendationDataMartForAllUsers();

        // Assert
        Assert.False(result.HasError);

    }
}