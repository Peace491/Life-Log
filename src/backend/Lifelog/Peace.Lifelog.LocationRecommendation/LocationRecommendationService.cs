namespace Peace.Lifelog.LocationRecommendation;

using DomainModels;
using Peace.Lifelog.DataAccess;
using Peace.Lifelog.Infrastructure;
using Peace.Lifelog.Security;
using System.Collections.Generic;

public class LocationRecommendation : ILoR
{
    private List<string> authorizedRoles = new List<string>() { "Normal", "Admin", "Root" };
    
    private IMapRepo mapRepo;
    private ILifelogAuthService lifelogAuthService;
    private Logging.ILogging logging;

    public async Task<Response> GetRecommendation(GetRecommendationPayload getRecommendationPayload)
    {
        if (getRecommendationPayload == null)
    }
    public async Task<Response> ViewRecommendation(ViewRecommendationPayload viewRecommendationPayload)
    {

    }
    #region View Pin
    public Task<Response> ViewPin(ViewPinRequest viewPinRequest)
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
            // handlePinError(response, viewPinRequest.Principal!, error.Message);
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
                var lliList = ConvertDatabaseResponseOutputToLLIObjectList(readPinLLIInDBResponse);
                response.Output = lliList;
            }
            catch (Exception error)
            {
                return handlePinError(response, viewPinRequest.Principal!, error.Message);
            }

            // Handle Success Response
            //var logResponse = this.logging.CreateLog("Logs", viewPinRequest.Principal!.UserId, "Info", "Business", "Pin view operation successful");
            return response;
        }
        else
        {
            // Handle the case when LLIId is null
            return handlePinError(response, viewPinRequest.Principal!, "LLIId is null");
        }
    }
    #endregion
    #region Switching Views
    public async Task<Response> updateLog(UpdateLogRequest updateLogRequest)
    {
        var response = new Response();
        response.HasError = false;
        var errorMessage = "";

        // Validate Input
        var validateDeletePinRequestResponse = this.pinValidation.ValidatePinRequest(response, updateLogRequest, PinRequestType.UpdateLog);
        if (validateDeletePinRequestResponse.HasError)
        {
            errorMessage = validateDeletePinRequestResponse.ErrorMessage;
            return handlePinError(response, updateLogRequest.Principal!, errorMessage!);
        }

        // Authorize request
        if (!IsUserAuthorizedForPin(updateLogRequest.Principal!))
        {
            errorMessage = "The User Is Not Authorized To view a Pin";
            return handlePinError(response, updateLogRequest.Principal!, errorMessage!);
        }

        var logResponse = await this.logging.CreateLog("Logs", updateLogRequest.Principal!.UserId, "Info", "View", "Map view changed to Location Recommendation");

        return response;
    }
    #endregion
    #region Helper Functions
    private Response loggingError(Response response, AppPrincipal principal, string errorMessage)
    {
        response.HasError = true;
        response.ErrorMessage = errorMessage;
        var logResponse = this.logging.CreateLog("Logs", principal.UserId, "ERROR", "Business", errorMessage);
        return response;
    }

    private bool IsUserAuthorizedForPin(AppPrincipal appPrincipal)
    {

        return lifelogAuthService.IsAuthorized(appPrincipal, authorizedRoles);
    }
    #endregion
}