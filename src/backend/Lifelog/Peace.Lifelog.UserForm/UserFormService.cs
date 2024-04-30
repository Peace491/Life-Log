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

        if (createUserFormRequest == null) {
            response.HasError = true;
            response.ErrorMessage = "User Form Request Must Not Be Null";
            return response;
        }

        // Validate Input
        var validateCreateUserFormRequestResponse = this.userFormValidation.ValidateUserFormRequest(response, createUserFormRequest, UserFormRequestType.Create);
        if (validateCreateUserFormRequestResponse.HasError)
        {
            errorMessage = validateCreateUserFormRequestResponse.ErrorMessage;
            return HandleUserFormError(response, createUserFormRequest.Principal!, errorMessage!);
        }

        // Authorize request
        if (!IsUserAuthorizedForUserForm(createUserFormRequest.Principal!))
        {
            errorMessage = "The User Is Not Authorized To Use The User Form";
            return HandleUserFormError(response, createUserFormRequest.Principal!, errorMessage);
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
            return HandleUserFormError(response, createUserFormRequest.Principal, error.Message);
        }

        // Handle Failure Response
        if (createUserFormInDBResponse.HasError)
        {
            errorMessage = "The User Form failed to save to the persistent data store";
            return HandleUserFormError(response, createUserFormRequest.Principal, errorMessage);
        }

        // Handle Success Response
        var logResponse = this.logging.CreateLog("Logs", "User Form successfully created", createUserFormRequest.Principal.UserId, "Info", "Business");
        return response;
    }

    public async Task<Response> GetUserFormRanking(AppPrincipal principal)
    {
        var response = new Response();
        response.HasError = false;
        var errorMessage = "";

        // Validate App Principal
        var validateAppPrincipalResponse = this.userFormValidation.ValidateAppPrincipal(response, principal);
        if (validateAppPrincipalResponse.HasError)
        {
            errorMessage = validateAppPrincipalResponse.ErrorMessage;
            return HandleUserFormError(response, principal, errorMessage!);
        }

        // Authorize request
        if (!IsUserAuthorizedForUserForm(principal!))
        {
            errorMessage = "The User Is Not Authorized To Use The User Form";
            return HandleUserFormError(response, principal, errorMessage);
        }

        // Get User Form in DB
        var userHash = principal.UserId;

        Response getUserFormRankingFromDBResponse;

        try
        {
            getUserFormRankingFromDBResponse = await this.userFormRepo.ReadUserFormCategoriesRankingInDB(userHash);

            var userFormRanking = ConvertUserFormRankingDBOutputToUserRankingObject(getUserFormRankingFromDBResponse);

            response.Output = [userFormRanking];

        }
        catch (Exception error)
        {
            return HandleUserFormError(response, principal, error.Message);
        }

        // Handle Failure Response
        if (getUserFormRankingFromDBResponse.HasError)
        {
            errorMessage = "The User Form failed to save to the persistent data store";
            return HandleUserFormError(response, principal, errorMessage);
        }

        // Handle Success Response
        var logResponse = this.logging.CreateLog("Logs", "User Form successfully created", userHash, "Info", "Business");
        return response;
    }

    public async Task<Response> UpdateUserForm(UpdateUserFormRequest updateUserFormRequest)
    {
        var response = new Response();
        response.HasError = false;
        var errorMessage = "";

        if (updateUserFormRequest == null) {
            response.HasError = true;
            response.ErrorMessage = "User Form Request Must Not Be Null";
            return response;
        }

        // Validate Input
        var validateUpdateUserFormRequestResponse = this.userFormValidation.ValidateUserFormRequest(response, updateUserFormRequest, UserFormRequestType.Update);
        if (validateUpdateUserFormRequestResponse.HasError)
        {
            errorMessage = validateUpdateUserFormRequestResponse.ErrorMessage;
            return HandleUserFormError(response, updateUserFormRequest.Principal!, errorMessage!);
        }

        // Authorize request
        if (!IsUserAuthorizedForUserForm(updateUserFormRequest.Principal!))
        {
            errorMessage = "The User Is Not Authorized To Use The User Form";
            return HandleUserFormError(response, updateUserFormRequest.Principal!, errorMessage);
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
            return HandleUserFormError(response, updateUserFormRequest.Principal, error.Message);
        }

        // Handle Failure Response
        if (updateUserFormInDBResponse.HasError)
        {
            errorMessage = "The User Form failed to save to the persistent data store";
            return HandleUserFormError(response, updateUserFormRequest.Principal, errorMessage);
        }

        // Handle Success Response
        var logResponse = this.logging.CreateLog("Logs", "User Form successfully updated", updateUserFormRequest.Principal.UserId, "Info", "Business");
        return response;
    }

    public async Task<bool> IsUserFormCompleted(AppPrincipal principal)
    {
        var response = new Response();
        response.HasError = false;

        // Validate App Principal
        var validateAppPrincipalResponse = this.userFormValidation.ValidateAppPrincipal(response, principal);
        if (validateAppPrincipalResponse.HasError)
        {
            return false;
        }

        // Check User Authorization
        // Authorize request
        if (!IsUserAuthorizedForUserForm(principal))
        {
            return false;
        }

        try
        {
            response = await this.userFormRepo.ReadUserFormCompletionStatusInDB(principal.UserId);
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

    // Helper Functions

    private bool IsUserAuthorizedForUserForm(AppPrincipal appPrincipal)
    {

        return lifelogAuthService.IsAuthorized(appPrincipal, authorizedRoles);
    }

    private Response HandleUserFormError(Response response, AppPrincipal principal, string errorMessage)
    {
        response.HasError = true;
        response.ErrorMessage = errorMessage;
        if (principal == null) {
            var logResponse = this.logging.CreateLog("Logs", errorMessage, "System" , "ERROR", "Business");
        } else {
            var logResponse = this.logging.CreateLog("Logs", errorMessage, principal.UserId, "ERROR", "Business");
        }
        return response;
    }

    private UserFormRanking ConvertUserFormRankingDBOutputToUserRankingObject(Response response)
    {
        var userFormRanking = new UserFormRanking();

        foreach (List<Object> rankings in response.Output!)
        {
            int index = 0;

            foreach (var ranking in rankings)
            {
                switch (index)
                {
                    case 0:
                        userFormRanking.MentalHealthRating = Convert.ToInt32(ranking);
                        break;
                    case 1:
                        userFormRanking.PhysicalHealthRating = Convert.ToInt32(ranking);
                        break;
                    case 2:
                        userFormRanking.OutdoorRating = Convert.ToInt32(ranking);
                        break;
                    case 3:
                        userFormRanking.SportRating = Convert.ToInt32(ranking);
                        break;
                    case 4:
                        userFormRanking.ArtRating = Convert.ToInt32(ranking);
                        break;
                    case 5:
                        userFormRanking.HobbyRating = Convert.ToInt32(ranking);
                        break;
                    case 6:
                        userFormRanking.ThrillRating = Convert.ToInt32(ranking);
                        break;
                    case 7:
                        userFormRanking.TravelRating = Convert.ToInt32(ranking);
                        break;
                    case 8:
                        userFormRanking.VolunteeringRating = Convert.ToInt32(ranking);
                        break;
                    case 9:
                        userFormRanking.FoodRating = Convert.ToInt32(ranking);
                        break;
                }
                index++;

            }

        }

        return userFormRanking;
    }
}
