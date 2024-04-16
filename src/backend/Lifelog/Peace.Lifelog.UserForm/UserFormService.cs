namespace Peace.Lifelog.UserForm;

using DomainModels;
using Peace.Lifelog.Infrastructure;
using Peace.Lifelog.Security;

public class UserFormService : IUserFormService
{
    private List<string> authorizedRoles = new List<string>() { "Normal", "Admin", "Root" };

    private IUserFormRepo userFormRepo;
    private ILifelogAuthService lifelogAuthService;
    private UserFormValidation userFormValidation;
    private Logging.ILogging logging;

    public UserFormService(IUserFormRepo userFormRepo, ILifelogAuthService lifelogAuthService, Logging.ILogging logging)
    {
        this.userFormRepo = userFormRepo;
        this.lifelogAuthService = lifelogAuthService;
        this.logging = logging;
        this.userFormValidation = new UserFormValidation();
    }

    public async Task<Response> CreateUserForm(CreateUserFormRequest createUserFormRequest)
    {
        var response = new Response();
        response.HasError = false;
        var errorMessage = "";

        // Validate Input
        var validateCreateUserFormRequestResponse = this.userFormValidation.ValidateUserFormRequest(response, createUserFormRequest, UserFormRequestType.Create);
        if (validateCreateUserFormRequestResponse.HasError)
        {
            errorMessage = validateCreateUserFormRequestResponse.ErrorMessage;
            return handleUserFormError(response, createUserFormRequest.Principal!, errorMessage!);
        }

        // Authorize request
        if (!IsUserAuthorizedForUserForm(createUserFormRequest.Principal!))
        {
            errorMessage = "The User Is Not Authorized To Use The User Form";
            return handleUserFormError(response, createUserFormRequest.Principal!, errorMessage);
        }

        // Create User Form in DB
        var userHash = createUserFormRequest.Principal!.UserId;

        Response createUserFormInDBResponse;

        try
        {
            createUserFormInDBResponse = await this.userFormRepo.CreateUserFormInDB(userHash, createUserFormRequest.MentalHealthRating, createUserFormRequest.PhysicalHealthRating, createUserFormRequest.OutdoorRating, createUserFormRequest.SportRating, createUserFormRequest.ArtRating, createUserFormRequest.HobbyRating, createUserFormRequest.ThrillRating, createUserFormRequest.TravelRating, createUserFormRequest.VolunteeringRating, createUserFormRequest.FoodRating);
        }
        catch (Exception error)
        {
            return handleUserFormError(response, createUserFormRequest.Principal, error.Message);
        }

        // Handle Failure Response
        if (createUserFormInDBResponse.HasError)
        {
            errorMessage = "The User Form failed to save to the persistent data store";
            return handleUserFormError(response, createUserFormRequest.Principal, errorMessage);
        }

        // Handle Success Response
        var logResponse = this.logging.CreateLog("Logs", "User Form successfully created", createUserFormRequest.Principal.UserId, "Info", "Business");
        return response;
    }

    public async Task<Response> UpdateUserForm(UpdateUserFormRequest updateUserFormRequest)
    {
        var response = new Response();
        response.HasError = false;
        var errorMessage = "";

        // Validate Input
        var validateUpdateUserFormRequestResponse = this.userFormValidation.ValidateUserFormRequest(response, updateUserFormRequest, UserFormRequestType.Update);
        if (validateUpdateUserFormRequestResponse.HasError)
        {
            errorMessage = validateUpdateUserFormRequestResponse.ErrorMessage;
            return handleUserFormError(response, updateUserFormRequest.Principal!, errorMessage!);
        }

        // Authorize request
        if (!IsUserAuthorizedForUserForm(updateUserFormRequest.Principal!))
        {
            errorMessage = "The User Is Not Authorized To Use The User Form";
            return handleUserFormError(response, updateUserFormRequest.Principal!, errorMessage);
        }

        // Update User Form in DB
        var userHash = updateUserFormRequest.Principal!.UserId;

        Response updateUserFormInDBResponse;

        try
        {
            updateUserFormInDBResponse = await this.userFormRepo.UpdateUserFormInDB(userHash, updateUserFormRequest.MentalHealthRating, updateUserFormRequest.PhysicalHealthRating, updateUserFormRequest.OutdoorRating, updateUserFormRequest.SportRating, updateUserFormRequest.ArtRating, updateUserFormRequest.HobbyRating, updateUserFormRequest.ThrillRating, updateUserFormRequest.TravelRating, updateUserFormRequest.VolunteeringRating, updateUserFormRequest.FoodRating);
        }
        catch (Exception error)
        {
            return handleUserFormError(response, updateUserFormRequest.Principal, error.Message);
        }

        // Handle Failure Response
        if (updateUserFormInDBResponse.HasError)
        {
            errorMessage = "The User Form failed to save to the persistent data store";
            return handleUserFormError(response, updateUserFormRequest.Principal, errorMessage);
        }

        // Handle Success Response
        var logResponse = this.logging.CreateLog("Logs", "User Form successfully updated", updateUserFormRequest.Principal.UserId, "Info", "Business");
        return response;
    }

    public async Task<bool> IsUserFormCompleted(string userHash)
    {
        var response = new Response();

        if (!userFormValidation.IsValidUserHash(userHash))
        {
            return false;
        }

        try
        {
            response = await this.userFormRepo.ReadUserFormCompletionStatusInDB(userHash);
        }
        catch
        {
            return false;
        }

        if (response.Output == null)
        {
            return false;
        }

        try
        {
            foreach (List<Object> output in response.Output)
            {
                foreach (bool completionStatus in output)
                {
                    return completionStatus;
                }
            }
        }
        catch
        {
            return false;
        }

        return false;
    }


    private bool IsUserAuthorizedForUserForm(AppPrincipal appPrincipal)
    {

        return lifelogAuthService.IsAuthorized(appPrincipal, authorizedRoles);
    }

    private Response handleUserFormError(Response response, AppPrincipal principal, string errorMessage)
    {
        response.HasError = true;
        response.ErrorMessage = errorMessage;
        var logResponse = this.logging.CreateLog("Logs", errorMessage, principal.UserId, "ERROR", "Business");
        return response;
    }

    public Task<UserFormRanking> GetUserFormRanking(string userHash)
    {
        throw new NotImplementedException();
    }
}
