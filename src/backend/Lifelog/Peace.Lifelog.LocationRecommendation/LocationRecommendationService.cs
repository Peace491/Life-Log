namespace Peace.Lifelog.LocationRecommendation;

using System.Collections.Generic;
using DomainModels;
using Peace.Lifelog.Infrastructure;
using Peace.Lifelog.LLI;
using Peace.Lifelog.Logging;
using Peace.Lifelog.Security;

public class LocationRecommendationService : ILocationRecommendationService
{
    private List<string> authorizedRoles = new List<string>() { "Normal", "Admin", "Root" };

    private ILocationRecommendationRepo locationRecommendationRepo;
    private ILifelogAuthService lifelogAuthService;
    private LocationRecommendationValidation locationRecommendationValidation;
    private LocationRecommendationCluster locationRecommendationCluster;
    //private PinService pinService;
    private ILogging logging;

    public LocationRecommendationService(ILocationRecommendationRepo locationRecommendationRepo, ILifelogAuthService lifelogAuthService, ILogging logging)
    {
        this.locationRecommendationRepo = locationRecommendationRepo;
        this.lifelogAuthService = lifelogAuthService;
        this.logging = logging;
        this.locationRecommendationValidation = new LocationRecommendationValidation();
        this.locationRecommendationCluster = new LocationRecommendationCluster();
        //this.pinService = new PinService(mapRepo, lifelogAuthService, logging);
    }

    #region Get Recommendation
    public async Task<Response> GetRecommendation(GetRecommendationRequest getRecommendationRequest)
    {
        var response = new Response();
        response.HasError = false;
        //string userHash = getRecommendationRequest.UserHash; 
        var userHash = "FrZ8KyeBMVPj2H+khuLszU8F95bwHUPhmWBJ1nUKJxs=";

        //var readPinResponse = await pinService.GetAllPinFromUser(userHash);
        //var readDataOnlyDAO = new ReadDataOnlyDAO();
        //var getAllCoord = $"SELECT Latitude, Longitude FROM mappin WHERE UserHash = 'FrZ8KyeBMVPj2H+khuLszU8F95bwHUPhmWBJ1nUKJxs='";
        //var coordResponse = await readDataOnlyDAO.ReadData(getAllCoord);
        //var pinOutput = ConvertDatabaseResponseOutputToPinObjectList(readPinResponse);
        //response.Output = pinOutput;
        var coordResponse = await this.locationRecommendationRepo.ReadAllUserPinInDB(userHash);
        var clusterDataResponse = locationRecommendationCluster.ClusterRecommendation(coordResponse);
        //var retrievePinIdResponse = GetPinId(clusterDataResponse);
        response.Output = clusterDataResponse.Output;
        return response;
    }
    #endregion
    #region View Recommendation
    public async Task<Response> ViewRecommendation(ViewRecommendationRequest viewRecommendationRequest)
    {
        var response = new Response();
        response.HasError = false;
        //var userHash = viewRecommendationRequest.UserHash;
        var userHash = "FrZ8KyeBMVPj2H+khuLszU8F95bwHUPhmWBJ1nUKJxs=";
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
        //return await this.logging.CreateLog("Logs", "userHash", "ERROR", "Business", "errorMessage");
    }
    /*public async Task<Response> GetPinId(Response response)
    {
        var retrievePinIdResponse = new Response();
        List<object> pinIdList = new List<object>();
        if (response.Output != null)
        {
            foreach (var Object in response.Output)
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
        return response;
    }*/
    #endregion
    #region View Pin
    #endregion
    #region Switching Views
    #endregion
    #region Helper Functions
    private Response LoggingError(Response response, AppPrincipal principal, string errorMessage)
    {
        response.HasError = true;
        response.ErrorMessage = errorMessage;
        var logResponse = this.logging.CreateLog("Logs", principal.UserId, "ERROR", "Business", errorMessage);
        return response;
    }

    private bool IsUserAuthorizedForLocationRecommendation(AppPrincipal appPrincipal)
    {

        return lifelogAuthService.IsAuthorized(appPrincipal, authorizedRoles);
    }

    private List<object>? ConvertDatabaseResponseOutputToPinObjectList(Response readPinResponse)
    {
        List<object> pinList = new List<object>();

        if (readPinResponse.Output == null)
        {
            return null;
        }

        foreach (List<object> Pin in readPinResponse.Output)
        {

            var pin = new Pin();

            int index = 0;

            foreach (var attribute in Pin)
            {
                if (attribute is null) continue;

                switch (index)
                {
                    case 0:
                        pin.PinId = attribute?.ToString() ?? "";
                        break;
                    case 1:
                        pin.LLIId = attribute?.ToString() ?? "";
                        break;
                    case 2:
                        pin.UserHash = attribute?.ToString() ?? "";
                        break;
                    case 3:
                        pin.Address = attribute?.ToString() ?? "";
                        break;
                    case 4:
                        pin.Latitude = attribute?.ToString() ?? "";
                        break;
                    case 5:
                        pin.Longitude = attribute?.ToString() ?? "";
                        break;
                    default:
                        break;
                }
                index++;

            }

            pinList.Add(pin);

        }

        return pinList;
    }

    private List<object>? ConvertDatabaseResponseOutputToLLIObjectList(Response readLLIResponse)
    {
        List<object> lliList = new List<object>();

        if (readLLIResponse.Output == null)
        {
            return null;
        }

        foreach (List<object> LLI in readLLIResponse.Output)
        {

            var lli = new LLI();

            int index = 0;

            foreach (var attribute in LLI)
            {
                if (attribute is null) continue;

                switch (index)
                {
                    case 0:
                        lli.LLIID = attribute.ToString() ?? "";
                        break;
                    case 1:
                        lli.UserHash = attribute.ToString() ?? "";
                        break;
                    case 2:
                        lli.Title = attribute.ToString() ?? "";
                        break;
                    case 3:
                        lli.Description = attribute.ToString() ?? "";
                        break;
                    case 4:
                        lli.Status = attribute.ToString() ?? "";
                        break;
                    case 5:
                        lli.Visibility = attribute.ToString() ?? "";
                        break;
                    case 6:
                        lli.Deadline = attribute.ToString() ?? "";
                        break;
                    case 7:
                        lli.Cost = Convert.ToInt32(attribute);
                        break;
                    case 8:
                        lli.Recurrence.Status = attribute.ToString() ?? "";
                        break;
                    case 9:
                        lli.Recurrence.Frequency = attribute.ToString() ?? "";
                        break;
                    case 10:
                        lli.CreationDate = attribute.ToString() ?? "";
                        break;
                    case 11:
                        lli.CompletionDate = attribute.ToString() ?? "";
                        break;
                    case 12:
                        lli.Category1 = attribute.ToString() ?? "";
                        break;
                    case 13:
                        lli.Category2 = attribute.ToString() ?? "";
                        break;
                    case 14:
                        lli.Category3 = attribute.ToString() ?? "";
                        break;
                    default:
                        break;
                }
                index++;

            }

            lliList.Add(lli);

        }

        return lliList;
    }
    #endregion
}