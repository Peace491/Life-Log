namespace Peace.Lifelog.UserForm;

using DomainModels;
using Peace.Lifelog.Infrastructure;
using Peace.Lifelog.Security;

public class UserFormService : IUserFormService
{
    private List<string> authorizedRoles = new List<string>() { "Normal", "Admin", "Root" };

    private IUserFormRepo userFormRepo;
    private LifelogAuthService lifelogAuthService;
    private UserFormValidation userFormValidation;
    private Logging.Logging logging;

    public UserFormService(IUserFormRepo userFormRepo, LifelogAuthService lifelogAuthService, Logging.Logging logging)
    {
        this.userFormRepo = userFormRepo;
        this.lifelogAuthService = lifelogAuthService;
        this.logging = logging;
        this.userFormValidation = new UserFormValidation();
    }

    public async Task<Response> createUserForm(CreateUserFormRequest createUserFormRequest)
    {
        var response = new Response();
        response.HasError = false;
        var errorMessage = "";

        // Validate Input
        var validateCreateUserFormRequestResponse = this.userFormValidation.validateCreateUserFormRequest(response, createUserFormRequest);
        if (validateCreateUserFormRequestResponse.HasError) {
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
        } catch (Exception error)
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

    public Task<Response> updateUserForm()
    {
        throw new NotImplementedException();
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
}
