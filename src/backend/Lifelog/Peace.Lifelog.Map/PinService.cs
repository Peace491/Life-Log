namespace Peace.Lifelog.Map;

using DomainModels;
using Peace.Lifelog.DataAccess;
using Peace.Lifelog.Infrastructure;
using Peace.Lifelog.LLI;
using Peace.Lifelog.Security;
using System.Collections.Generic;

public class PinService : IPinService
{
    private List<string> authorizedRoles = new List<string>() { "Normal", "Admin", "Root" };

    private IMapRepo mapRepo;
    private ILifelogAuthService lifelogAuthService;
    private PinValidation pinValidation;
    private Logging.ILogging logging;
    private LLIService lliService;

    //For LLI 
    private CreateDataOnlyDAO createDataOnlyDAO;
    private ReadDataOnlyDAO readDataOnlyDAO;
    private UpdateDataOnlyDAO updateDataOnlyDAO;
    private DeleteDataOnlyDAO deleteDataOnlyDAO;
    private Logging.Logging loggingLLI;
    public PinService(IMapRepo mapRepo, ILifelogAuthService lifelogAuthService, Logging.ILogging logging)
    {
        this.mapRepo = mapRepo;
        this.lliService = new LLIService(this.createDataOnlyDAO, this.readDataOnlyDAO, this.updateDataOnlyDAO, this.deleteDataOnlyDAO, this.loggingLLI);
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

        try
        {
            createPinInDBResponse = await this.mapRepo.CreatePinInDB(createPinRequest.LLIId, createPinRequest.Address, createPinRequest.Latitude, createPinRequest.Longitude);
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
        var logResponse = this.logging.CreateLog("Logs", "Pin creation operation successful", createPinRequest.Principal.UserId, "Info", "Business");
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
        var logResponse = this.logging.CreateLog("Logs", "Pin update operation successful", updatePinRequest.Principal.UserId, "Info", "Business");
        return response;
    }

    public async Task<Response> DeletePin(DeletePinRequest deletePinRequest)
    {
        var response = new Response();
        response.HasError = false;
        var errorMessage = "";

        // Validate Input
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
        }

        // Create Pin in DB
        Response deletePinInDBResponse;

        try
        {
            deletePinInDBResponse = await this.mapRepo.DeletePinInDB(deletePinRequest.PinId);
        }
        catch (Exception error)
        {
            return handlePinError(response, deletePinRequest.Principal!, error.Message);
        }

        // Handle Failure Response
        if (deletePinInDBResponse.HasError)
        {
            errorMessage = "The Pin failed to save to the persistent data store";
            return handlePinError(response, deletePinRequest.Principal!, errorMessage);
        }

        // Handle Success Response
        var logResponse = this.logging.CreateLog("Logs", "Pin deletion operation successful", deletePinRequest.Principal.UserId, "Info", "Business");
        return response;
    }

    public async Task<Response> ViewPin(ViewPinRequest viewPinRequest)
    {
        var response = new Response();
        response.HasError = false;
        var errorMessage = "";

        // Validate Input
        var validateDeletePinRequestResponse = this.pinValidation.ValidatePinRequest(response, viewPinRequest, PinRequestType.View);
        if (validateDeletePinRequestResponse.HasError)
        {
            errorMessage = validateDeletePinRequestResponse.ErrorMessage;
            return handlePinError(response, viewPinRequest.Principal!, errorMessage!);
        }

        // Authorize request
        if (!IsUserAuthorizedForPin(viewPinRequest.Principal!))
        {
            errorMessage = "The User Is Not Authorized To view a Pin";
            return handlePinError(response, viewPinRequest.Principal!, errorMessage!);
        }

        //Read the Pin in DB 
        Response readPinInDBResponse;

        //Initialize the LLIId
        string? LLIId = null;

        try
        {
            readPinInDBResponse = await this.mapRepo.ReadPinInDB(viewPinRequest.PinId);
            if (readPinInDBResponse.Output is not null)
            {
                List<object> readOutput = (List<object>)readPinInDBResponse.Output;
                if (readOutput.Count > 1 && readOutput[1] is not null) // Check if index is valid and element is of type int
                {
                    LLIId = readOutput[1].ToString();
                }
            }
        }
        catch (Exception error)
        {
            return handlePinError(response, viewPinRequest.Principal!, error.Message);
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
            var logResponse = this.logging.CreateLog("Logs", "Pin view operation successful", viewPinRequest.Principal.UserId, "Info", "Business");
            return response;
        }
        else
        {
            // Handle the case when LLIId is null
            return handlePinError(response, viewPinRequest.Principal!, "LLIId is null");
        }
    }

    public async Task<Response> EditPinLLI(EditPinLIIRequest editPinLLIRequest)
    {
        var response = new Response();
        response.HasError = false;
        var errorMessage = "";

        // Validate Input
        var validateCreatePinRequestResponse = this.pinValidation.ValidatePinRequest(response, editPinLLIRequest, PinRequestType.Edit);
        if (validateCreatePinRequestResponse.HasError)
        {
            errorMessage = validateCreatePinRequestResponse.ErrorMessage;
            return handlePinError(response, editPinLLIRequest.Principal!, errorMessage!);
        }

        // Authorize request
        if (!IsUserAuthorizedForPin(editPinLLIRequest.Principal!))
        {
            errorMessage = "The User Is Not Authorized To edit an LLI";
            return handlePinError(response, editPinLLIRequest.Principal!, errorMessage!);
        }

        // Update Pin in DB
        Response readLLIInDBResponse;
        var userHash = editPinLLIRequest.Principal!.UserId;

        try
        {
            readLLIInDBResponse = await this.mapRepo.ReadLLIInDB(editPinLLIRequest.LLIId);

            if (readLLIInDBResponse.Output is not null)
            {
                var lliOutput = ConvertDatabaseResponseOutputToLLIObjectList(readLLIInDBResponse);

                if (lliOutput != null)
                {
                    LLI lli = (LLI)lliOutput[0];
                    var editLLIResponse = this.lliService.UpdateLLI(userHash, lli);
                }
            }
        }
        catch (Exception error)
        {
            return handlePinError(response, editPinLLIRequest.Principal!, error.Message);
        }

        // Handle Success Response
        var logResponse = this.logging.CreateLog("Logs", "LLI edit operation performed through pin", editPinLLIRequest.Principal.UserId, "Info", "Persistent Data Store");
        return response;
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

            var logerrorResponse = this.logging.CreateLog("Logs", error_Message, userHash, "ERROR", "Business");
        }



        // Handle Failure Response
        if (readPinStatusInDBResponse != null && readPinStatusInDBResponse.HasError) // Check if readPinStatusInDBResponse is not null
        {
            string? error_Message = readPinStatusInDBResponse.ErrorMessage;
            var logerrorResponse = this.logging.CreateLog("Logs", error_Message, userHash, "ERROR", "Business");
        }

        // Handle Success Response
        var logResponse = this.logging.CreateLog("Logs", "Pin update operation successful", userHash, "Info", "Business");
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

            return await this.logging.CreateLog("Logs", errorMessage, userHash, "ERROR", "Business");
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

            return await this.logging.CreateLog("Logs", errorMessage, userHash, "ERROR", "Business");
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

        var logResponse = await this.logging.CreateLog("Logs", "Map view changed to Location Recommendation", updateLogRequest.Principal.UserId, "Info", "View");

        return response;
    }

    public async Task<Response> GetAllUserLLI(string userHash)
    {
        var response = new Response();
        response.HasError = false;

        //Authorize #TODO

        //Validate Inpit 
        var validateRequestResponse = this.pinValidation.IsValidUserHash(userHash);
        if (!validateRequestResponse)
        {
            var errorMessage = "invalid user hash";

            return await this.logging.CreateLog("Logs", errorMessage, userHash, "ERROR", "Business");
        }

        //Get all user LLI
        try
        {
            response = await this.lliService.GetAllLLIFromUser(userHash);
        }
        catch (Exception error)
        {
            // Convert the Exception object to a string
            string errorMessage = error.ToString();

            return await this.logging.CreateLog("Logs", errorMessage, userHash, "ERROR", "Business");
        }

        return response;

    }


    #region Helper Functions
    private Response handlePinError(Response response, AppPrincipal principal, string errorMessage)
    {
        response.HasError = true;
        response.ErrorMessage = errorMessage;
        var logResponse = this.logging.CreateLog("Logs", errorMessage, principal.UserId, "ERROR", "Business");
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
    private List<Object>? ConvertDatabaseResponseOutputToPinObjectList(Response readPinResponse)
    {
        List<Object> pinList = new List<Object>();

        if (readPinResponse.Output == null)
        {
            return null;
        }

        foreach (List<Object> Pin in readPinResponse.Output)
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
    private List<Object>? ConvertDatabaseResponseOutputToLLIObjectList(Response readLLIResponse)
    {
        List<Object> lliList = new List<Object>();

        if (readLLIResponse.Output == null)
        {
            return null;
        }

        foreach (List<Object> LLI in readLLIResponse.Output)
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