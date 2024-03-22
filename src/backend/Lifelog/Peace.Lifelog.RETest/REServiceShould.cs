namespace Peace.Lifelog.RETest;
using Peace.Lifelog.RE; // Add this line to import the 'ReService' class
using Peace.Lifelog.DataAccess;
public class ReServiceShould
{
    [Fact]
    public async void Test_Test()
    {
        // Arrange
        var recomendationEngineRepository = new RecomendationEngineRepository();
        var reService = new REService(recomendationEngineRepository);

        // Act
        var response = await reService.getNumRecs("0Yg6cgh/M4+ImmL0GozWqhgcDCqTZEhzm9angvVAC30=", 5);

        // Assert
        Assert.False(response.HasError);
    }
}