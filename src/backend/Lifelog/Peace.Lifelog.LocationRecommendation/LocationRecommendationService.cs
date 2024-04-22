namespace Peace.Lifelog.LocationRecommendation;

using System.Collections.Generic;
using DomainModels;
using Peace.Lifelog.Infrastructure;
using Peace.Lifelog.Security;

public class LocationRecommendationService : ILocationRecommendationService
{
    private List<string> authorizedRoles = new List<string>() { "Normal", "Admin", "Root" };

    private IMapRepo mapRepo;
    private ILifelogAuthService lifelogAuthService;
    private LocationRecommendationValidation locationRecommendationValidation;
    private ILogging logging;

    public LocationRecommendationService(IMapRepo mapRepo, ILifelogAuthService lifelogAuthService, ILogging logging)
    {
        this.mapRepo = mapRepo;
        this.lifelogAuthService = lifelogAuthService;
        this.logging = logging;
        this.locationRecommendationValidation = new LocationRecommendationValidation();
    }

    #region Get Recommendation
    public async Task<Response> GetRecommendation(GetRecommendationRequest getRecommendationRequest)
    {
        var response = new Response();
        response.HasError = false;
        Response readPinResponse = new();

        //Validate Inpit 
        var validateRequestResponse = this.pinValidation.IsValidUserHash(userHash);
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
        var clusterDataResponse = locationRecommendationValidation.Cluster(response);
        return response;
    }
    #endregion
    #region View Recommendation
    public async Task<Response> ViewRecommendation(ViewRecommendationRequest viewRecommendationRequest)
    {

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
    public async Task<Response> UpdateLog(UpdateLogRequest updateLogRequest)
    {
        var response = new Response();
        response.HasError = false;
        var errorMessage = "";

        // Validate Input
        var validateDeletePinRequestResponse = this.pinValidation.ValidatePinRequest(response, updateLogRequest, PinRequestType.UpdateLog);
        if (validateDeletePinRequestResponse.HasError)
        {
            errorMessage = validateDeletePinRequestResponse.ErrorMessage;
            return LoggingError(response, updateLogRequest.Principal!, errorMessage!);
        }

        // Authorize request
        if (!IsUserAuthorizedForPin(updateLogRequest.Principal!))
        {
            errorMessage = "The User Is Not Authorized To view a Pin";
            return LoggingError(response, updateLogRequest.Principal!, errorMessage!);
        }

        var logResponse = await this.logging.CreateLog("Logs", updateLogRequest.Principal!.UserId, "Info", "View", "Map view changed to Location Recommendation");

        return response;
    }
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
    #endregion
}