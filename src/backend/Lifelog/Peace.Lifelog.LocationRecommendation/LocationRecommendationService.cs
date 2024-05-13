namespace Peace.Lifelog.LocationRecommendation;

using System.Collections.Generic;
using DomainModels;
using Peace.Lifelog.Infrastructure;
using Peace.Lifelog.Logging;
using Peace.Lifelog.Security;

public class LocationRecommendationService : ILocationRecommendationService
{
    private List<string> authorizedRoles = new List<string>() { "Normal", "Admin", "Root" };

    private ILocationRecommendationRepo locationRecommendationRepo;
    private ILifelogAuthService lifelogAuthService;
    private LocationRecommendationValidation locationRecommendationValidation;
    private LocationRecommendationCluster locationRecommendationCluster;
    private ILogging logging;

    public LocationRecommendationService(ILocationRecommendationRepo locationRecommendationRepo, ILifelogAuthService lifelogAuthService, ILogging logging)
    {
        this.locationRecommendationRepo = locationRecommendationRepo;
        this.lifelogAuthService = lifelogAuthService;
        this.logging = logging;
        this.locationRecommendationValidation = new LocationRecommendationValidation();
        this.locationRecommendationCluster = new LocationRecommendationCluster();
    }

    #region Get Recommendation
    public async Task<Response> GetRecommendation(string UserHash)
    {
        var response = new Response();
        response.HasError = false;
        string userHash = UserHash;
        //string userHash = getRecommendationRequest.UserHash;
        //string userHash = "i24PV/YIYoFeK73u3B4XqA9YIhpHfsH2M+LjNY+Wf3s=";
        response = this.locationRecommendationValidation.ValidateUser(response, userHash); //response, getRecommendationRequest.Principal!, userHash
        if (response.HasError)
        {
            response = LoggingError(response, userHash, response.ErrorMessage!); //response, getRecommendationRequest.Principal!, userHash
            return response;
        }
        //if (IsUserAuthorizedForLocationRecommendation()) //getRecommendationRequest.Principal!
        //{
        try
        {
            var coordResponse = await this.locationRecommendationRepo.ReadAllUserPinInDB(userHash);
            var clusterDataResponse = locationRecommendationCluster.ClusterRecommendation(coordResponse);
            response.Output = clusterDataResponse.Output;
        }
        catch (Exception error)
        {
            string errorMessage = error.ToString();
            response = LoggingError(response, userHash, errorMessage); //response, getRecommendationRequest.Principal!, errorMessage
        }
        //}
        response = LoggingSuccess(response, userHash, response.ErrorMessage!); //response, getRecommendationRequest.Principal!, errorMessage
        return response;
    }
    #endregion
    #region View Recommendation
    public async Task<Response> ViewRecommendation(ViewRecommendationRequest viewRecommendationRequest)
    {
        var response = new Response();
        response.HasError = false;
        var userHash = viewRecommendationRequest.UserHash;
        var coordResponse = await this.locationRecommendationRepo.ReadAllUserPinInDB(userHash);
        var clusterDataResponse = locationRecommendationCluster.ClusterMarkerCoordinates(coordResponse);
        var retrievePinIdResponse = new Response();
        List<object> pinIdList = new List<object>();
        if (clusterDataResponse.Output != null)
        {
            foreach (var Object in clusterDataResponse.Output)
            {
                if (Object is List<double[]> innerList)
                {
                    foreach (double[] coordinate in innerList)
                    {
                        var lat = coordinate.ElementAtOrDefault(0);
                        var lng = coordinate.ElementAtOrDefault(1);
                        retrievePinIdResponse = await this.locationRecommendationRepo.GetPinId(lat!, lng!);
                        foreach (List<object> pin in retrievePinIdResponse.Output!)
                        {
                            pinIdList.Add(pin);
                        }
                    }
                }
            }
        }
        response = retrievePinIdResponse;
        response.Output = pinIdList;
        //var retrievePinIdResponse = GetPinId(clusterDataResponse);
        return response;
    }

    #region Helper Functions
    private Response LoggingError(Response response, string userHash, string errorMessage) //AppPrincipal principall
    {
        response.HasError = true;
        response.ErrorMessage = errorMessage;
        var logResponse = this.logging.CreateLog("Logs", userHash, "ERROR", "Business", errorMessage);
        return response;
    }
    private Response LoggingSuccess(Response response, string userHash, string errorMessage)
    {
        response.HasError = false;
        response.ErrorMessage = errorMessage;
        var logResponse = this.logging.CreateLog("Logs", userHash, "Info", "Business", errorMessage);
        return response;
    }

    /*private bool IsUserAuthorizedForLocationRecommendation(AppPrincipal appPrincipal)
    {

        return lifelogAuthService.IsAuthorized(appPrincipal, authorizedRoles);
    }*/


    #endregion
    #endregion
}