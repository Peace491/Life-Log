namespace Peace.Lifelog.Map;

using DomainModels;
using Peace.Lifelog.DataAccess;
using Peace.Lifelog.Infrastructure;
using Peace.Lifelog.LLI;
using Peace.Lifelog.Logging;
using Peace.Lifelog.Security;
using System.Collections.Generic;

public class PinService : IPinService
{
    private List<string> authorizedRoles = new List<string>() { "Normal", "Admin", "Root" };

    private IMapRepo mapRepo;
    private ILifelogAuthService lifelogAuthService;
    private PinValidation pinValidation;
    private ILogging logging;

    //For LLI 
    private CreateDataOnlyDAO createDataOnlyDAO = new CreateDataOnlyDAO();
    private ReadDataOnlyDAO? readDataOnlyDAO = new ReadDataOnlyDAO();
    private UpdateDataOnlyDAO? updateDataOnlyDAO = new UpdateDataOnlyDAO();
    private DeleteDataOnlyDAO? deleteDataOnlyDAO = new DeleteDataOnlyDAO();
    private LogTarget? logTarget;
    private Logging? loggingLLI;

    public PinService(IMapRepo mapRepo, ILifelogAuthService lifelogAuthService, ILogging logging)
    {
        this.mapRepo = mapRepo;
        this.logTarget = new LogTarget(createDataOnlyDAO, readDataOnlyDAO);
        this.loggingLLI = new Logging(logTarget);
        this.lifelogAuthService = lifelogAuthService;
        this.logging = logging;
        this.pinValidation = new PinValidation();
    }

    public async Task<Response> CreatePin(CreatePinRequest createPinRequest)
    {
        var response = new Response();
        response.HasError = false;
        var errorMessage = "";

        // Validate Input
        var validateCreatePinRequestResponse = this.pinValidation.ValidatePinRequest(response, createPinRequest, PinRequestType.Create);
        if (validateCreatePinRequestResponse.HasError)
        {
            errorMessage = validateCreatePinRequestResponse.ErrorMessage;
            return handlePinError(response, createPinRequest.Principal!, errorMessage!);
        }

        // Authorize request
        if (!IsUserAuthorizedForPin(createPinRequest.Principal!))
        {
            errorMessage = "The User Is Not Authorized To Create a Pin";
            return handlePinError(response, createPinRequest.Principal!, errorMessage!);
        }

        // Create Pin in DB
        Response createPinInDBResponse;
        var userHash = createPinRequest.Principal!.UserId;

        try
        {
            createPinInDBResponse = await this.mapRepo.CreatePinInDB(createPinRequest.LLIId, userHash, createPinRequest.Address, createPinRequest.Latitude, createPinRequest.Longitude);
        }
        catch (Exception error)
        {
            return handlePinError(response, createPinRequest.Principal!, error.Message);
        }

        // Handle Failure Response
        if (createPinInDBResponse.HasError)
        {
            errorMessage = "The Pin failed to save to the persistent data store";
            return handlePinError(response, createPinRequest.Principal!, errorMessage);
        }

        // Handle Success Response
        var logResponse = this.logging.CreateLog("Logs", createPinRequest.Principal!.UserId, "Info", "Business", "Pin creation operation successful");
        response = createPinInDBResponse;
        return response;
    }

    public async Task<Response> UpdatePin(UpdatePinRequest updatePinRequest)
    {
        var response = new Response();
        response.HasError = false;
        var errorMessage = "";

        // Validate Input
        var validateCreatePinRequestResponse = this.pinValidation.ValidatePinRequest(response, updatePinRequest, PinRequestType.Update);
        if (validateCreatePinRequestResponse.HasError)
        {
            errorMessage = validateCreatePinRequestResponse.ErrorMessage;
            return handlePinError(response, updatePinRequest.Principal!, errorMessage!);
        }

        // Authorize request
        if (!IsUserAuthorizedForPin(updatePinRequest.Principal!))
        {
            errorMessage = "The User Is Not Authorized To update a Pin";
            return handlePinError(response, updatePinRequest.Principal!, errorMessage!);
        }

        // Update Pin in DB
        Response updatePinInDBResponse;

        try
        {
            updatePinInDBResponse = await this.mapRepo.UpdatePinInDB(updatePinRequest.PinId, updatePinRequest.Address, updatePinRequest.Latitude, updatePinRequest.Longitude);
        }
        catch (Exception error)
        {
            return handlePinError(response, updatePinRequest.Principal!, error.Message);
        }

        // Handle Failure Response
        if (updatePinInDBResponse.HasError)
        {
            errorMessage = "The Pin location failed to update in the persistent data store";
            return handlePinError(response, updatePinRequest.Principal!, errorMessage);
        }

        // Handle Success Response
        var logResponse = this.logging.CreateLog("Logs", updatePinRequest.Principal!.UserId, "Info", "Business", "Pin update operation successful");
        return response;
    }

    public async Task<Response> DeletePin(string pinId, string userHash)
    {
        var response = new Response();
        response.HasError = false;
        var errorMessage = "";

        /*// Validate Input
        var validateDeletePinRequestResponse = this.pinValidation.ValidatePinRequest(response, deletePinRequest, PinRequestType.Delete);
        if (validateDeletePinRequestResponse.HasError)
        {
            errorMessage = validateDeletePinRequestResponse.ErrorMessage;
            return handlePinError(response, deletePinRequest.Principal!, errorMessage!);
        }

        // Authorize request
        if (!IsUserAuthorizedForPin(deletePinRequest.Principal!))
        {
            errorMessage = "The User Is Not Authorized To Delete a Pin";
            return handlePinError(response, deletePinRequest.Principal!, errorMessage!);
        }*/

        // Create Pin in DB
        Response deletePinInDBResponse;

        try
        {
            deletePinInDBResponse = await this.mapRepo.DeletePinInDB(pinId);
        }
        catch (Exception error)
        {
            response.HasError = true;
            response.ErrorMessage = error.Message;
            return response;
        }

        // Handle Failure Response
        if (deletePinInDBResponse.HasError)
        {
            errorMessage = "The Pin failed to save to the persistent data store";
            response.HasError = true;
            response.ErrorMessage = errorMessage;
            return response;
        }

        // Handle Success Response
        var logResponse = this.logging.CreateLog("Logs", userHash, "Info", "Business", "Pin deletion operation successful");
        return response;
    }

    public async Task<Response> ViewPin(ViewPinRequest viewPinRequest)
    {
        var response = new Response();
        response.HasError = false;

        #region TODO
        /*// Validate Input
        var validateDeletePinRequestResponse = this.pinValidation.ValidatePinRequest(response, viewPinRequest, PinRequestType.View);
        if (validateDeletePinRequestResponse.HasError)
        {
            var errorMessage = validateDeletePinRequestResponse.ErrorMessage;
            return handlePinError(response, viewPinRequest.Principal!, errorMessage!);
        }*/

        // Authorize request
        /*if (!IsUserAuthorizedForPin(viewPinRequest.Principal!))
        {
            var errorMessage = "The User Is Not Authorized To view a Pin";
            return handlePinError(response, viewPinRequest.Principal!, errorMessage!);
        }*/
        #endregion

        //Read the Pin in DB 
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

    public async Task<Response> FetchPinStatus(string LLIId, string userHash)
    {
        var response = new Response();
        response.HasError = false;

        // Get all Pin for an LLI in DB
        Response readPinStatusInDBResponse = new(); // Initialize with null

        try
        {
            readPinStatusInDBResponse = await this.mapRepo.ReadAllPinForLLIInDB(LLIId);

            if (readPinStatusInDBResponse.Output != null)
            {
                var pinStatusOutput = ConvertDatabaseResponseOutputToPinStatusObjectList(readPinStatusInDBResponse);
                if (pinStatusOutput != null)
                {
                    ICollection<object> pinStatusCollection = pinStatusOutput.Cast<object>().ToList();
                    response.Output = pinStatusCollection;
                }
            }
        }
        catch (Exception error)
        {
            // Convert the Exception object to a string
            string error_Message = error.ToString();

            var logerrorResponse = this.logging.CreateLog("Logs", userHash, "ERROR", "Business", error_Message);
        }



        // Handle Failure Response
        if (readPinStatusInDBResponse != null && readPinStatusInDBResponse.HasError) // Check if readPinStatusInDBResponse is not null
        {
            string? error_Message = readPinStatusInDBResponse.ErrorMessage;
            var logerrorResponse = this.logging.CreateLog("Logs", userHash, "ERROR", "Business", error_Message);
        }

        // Handle Success Response
        var logResponse = this.logging.CreateLog("Logs", userHash, "Info", "Business", "Pin update operation successful");
        return response;

    }

    public async Task<Response> GetAllPinFromUser(string userHash)
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
        return response;
    }
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


    #region Helper Functions
    private Response handlePinError(Response response, AppPrincipal principal, string errorMessage)
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

    // convert to PinStatus object 
    private List<PinStatus>? ConvertDatabaseResponseOutputToPinStatusObjectList(Response readPinResponse)
    {
        List<PinStatus> pinStatusList = new List<PinStatus>();

        if (readPinResponse.Output == null)
        {
            return null;
        }

        foreach (List<object> Pin in readPinResponse.Output)
        {
            var pinStatus = new PinStatus();
            int index = 0;

            foreach (var attribute in Pin)
            {
                if (attribute is null) continue;

                switch (index)
                {
                    case 0:
                        pinStatus.LLIId = attribute.ToString() ?? "";
                        break;
                    case 1:
                        pinStatus.count = attribute.ToString() ?? "";
                        break;
                    default:
                        break;
                }
                index++;
            }

            pinStatusList.Add(pinStatus);
        }

        return pinStatusList;
    }

    // Convert read response to Pin Object 
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

    // Convert read response to LLI object
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