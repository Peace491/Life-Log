namespace Peace.Lifelog.LocationRecommendation;

using System.Collections.Generic;
using DomainModels;
using Peace.Lifelog.Infrastructure;
using Peace.Lifelog.Security;
using Peace.Lifelog.Logging;
using Peace.Lifelog.LLI;

public class LocationRecommendationService : ILocationRecommendationService
{
    private List<string> authorizedRoles = new List<string>() { "Normal", "Admin", "Root" };

    private IMapRepo mapRepo;
    private ILifelogAuthService lifelogAuthService;
    private LocationRecommendationValidation locationRecommendationValidation;
    private LocationRecommendationCluster locationRecommendationCluster;
    private ILogging logging;

    public LocationRecommendationService(IMapRepo mapRepo, ILifelogAuthService lifelogAuthService, ILogging logging)
    {
        this.mapRepo = mapRepo;
        this.lifelogAuthService = lifelogAuthService;
        this.logging = logging;
        this.locationRecommendationValidation = new LocationRecommendationValidation();
        this.locationRecommendationCluster = new LocationRecommendationCluster();
    }

    #region Get Recommendation
    public async Task<Response> GetRecommendation(GetRecommendationRequest getRecommendationRequest)
    {
        var response = new Response();
        response.HasError = false;
        Response readPinResponse = new();
        var userHash = getRecommendationRequest.UserHash;

        //Validate Inpit 
        var validateRequestResponse = this.locationRecommendationValidation.IsValidUserHash(userHash);
        if (!validateRequestResponse)
        {
            var errorMessage = "invalid user hash";

            return await this.logging.CreateLog("Logs", userHash, "ERROR", "Business", errorMessage);
        }

        //Get all user LLI
        try
        {
            readPinResponse = await this.mapRepo.ReadAllUserPinInDB(userHash);

        }
        catch (Exception error)
        {
            // Convert the Exception object to a string
            string errorMessage = error.ToString();

            return await this.logging.CreateLog("Logs", userHash, "ERROR", "Business", errorMessage);
        }


        var pinOutput = ConvertDatabaseResponseOutputToPinObjectList(readPinResponse);
        response.Output = pinOutput;
        var clusterDataResponse = locationRecommendationCluster.ClusterRequest(response);
        return response;
    }
    #endregion
    #region View Recommendation
    public async Task<Response> ViewRecommendation(ViewRecommendationRequest viewRecommendationRequest)
    {
        return await this.logging.CreateLog("Logs", "userHash", "ERROR", "Business", "errorMessage");
    }
    #endregion
    #region View Pin
    public async Task<Response> ViewPin(ViewPinRequest viewPinRequest)
    {
        var response = new Response();
        response.HasError = false;

        Response readPinInDBResponse;

        //Initialize the LLIId
        string? LLIId = null;

        try
        {
            readPinInDBResponse = await this.mapRepo.ReadPinInDB(viewPinRequest.PinId);
            if (readPinInDBResponse.Output is not null)
            {
                foreach (List<object> lliList in readPinInDBResponse.Output)
                {
                    foreach (Int32 lliId in lliList)
                    {
                        LLIId = lliId.ToString();
                    }
                }
            }
        }
        catch (Exception error)
        {
            response.HasError = true;
            response.ErrorMessage = error.Message;
            return response;
        }

        // Read the LLI in the DB only if LLIId is not null
        if (LLIId is not null)
        {
            Response readPinLLIInDBResponse;
            try
            {
                readPinLLIInDBResponse = await this.mapRepo.ReadLLIInDB(LLIId);
                var lliList = ConvertDatabaseResponseOutputToLLIObjectList(readPinLLIInDBResponse);//change
                response.Output = lliList;
            }
            catch (Exception error)
            {
                return LoggingError(response, viewPinRequest.Principal!, error.Message);
            }

            // Handle Success Response
            return response;
        }
        else
        {
            // Handle the case when LLIId is null
            return LoggingError(response, viewPinRequest.Principal!, "LLIId is null");
        }
    }
    #endregion
    #region Switching Views
    //might be unnessecary
    /*public async Task<Response> UpdateLog(UpdateLogRequest updateLogRequest)
    {
        var response = new Response();
        response.HasError = false;
        var errorMessage = "";

        // Validate Input
        var validateDeletePinRequestResponse = this.locationRecommendationValidation.ValidatePinRequest(response, updateLogRequest, PinRequestType.UpdateLog);
        if (validateDeletePinRequestResponse.HasError)
        {
            errorMessage = validateDeletePinRequestResponse.ErrorMessage;
            return LoggingError(response, updateLogRequest.Principal!, errorMessage!);
        }

        // Authorize request
        if (!IsUserAuthorizedForLocationRecommendation(updateLogRequest.Principal!))
        {
            errorMessage = "The User Is Not Authorized To view a Pin";
            return LoggingError(response, updateLogRequest.Principal!, errorMessage!);
        }

        var logResponse = await this.logging.CreateLog("Logs", updateLogRequest.Principal!.UserId, "Info", "View", "Map view changed to Location Recommendation");

        return response;
    }*/
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