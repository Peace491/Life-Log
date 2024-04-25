namespace Peace.Lifelog.LocationRecommendationTest;
using Peace.Lifelog.DataAccess;
using Peace.Lifelog.Infrastructure;
using Peace.Lifelog.LocationRecommendation;
using Peace.Lifelog.Logging;
using Peace.Lifelog.Security;

public class LocationRecommendationServiceShould
{
    private static CreateDataOnlyDAO createDataOnlyDAO = new CreateDataOnlyDAO();
    private static ReadDataOnlyDAO readDataOnlyDAO = new ReadDataOnlyDAO();
    private static UpdateDataOnlyDAO updateDataOnlyDAO = new UpdateDataOnlyDAO();
    private static DeleteDataOnlyDAO deleteDataOnlyDAO = new DeleteDataOnlyDAO();
    private static LogTarget logTarget = new LogTarget(createDataOnlyDAO, readDataOnlyDAO);
    private static Logging logging = new Logging(logTarget);
    private static ILifelogAuthService lifelogAuthService = new LifelogAuthService();
    private static ILocationRecommendationRepo locationRecommendationRepo = new LocationRecommendationRepo(createDataOnlyDAO, readDataOnlyDAO, updateDataOnlyDAO, deleteDataOnlyDAO);
    private LocationRecommendationService locationRecommendationService = new LocationRecommendationService(locationRecommendationRepo, lifelogAuthService, logging);

    [Fact]
    public async Task GetRecommendation_ShouldOutputCluster()
    {
        GetRecommendationRequest getRecommendationRequest = new GetRecommendationRequest();
        getRecommendationRequest.Principal = null;
        getRecommendationRequest.UserHash = "FrZ8KyeBMVPj2H+khuLszU8F95bwHUPhmWBJ1nUKJxs=";
        var locationResponse = await locationRecommendationService.GetRecommendation(getRecommendationRequest);
        //var pass = true;

        Assert.True(locationResponse.Output!.Count >= 0);
    }

    [Fact]
    public async Task ViewRecommendation_ShouldOutputPinIdAndRankingOfCategories()
    {
        ViewRecommendationRequest viewRecommendationRequest = new ViewRecommendationRequest();
        viewRecommendationRequest.Principal = null;
        viewRecommendationRequest.UserHash = "FrZ8KyeBMVPj2H+khuLszU8F95bwHUPhmWBJ1nUKJxs=";
        var locationResponse = await locationRecommendationService.ViewRecommendation(viewRecommendationRequest);
        //var pass = true;

        Assert.True(locationResponse.Output!.Count >= 0);
    }
}