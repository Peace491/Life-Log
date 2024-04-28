using DomainModels;
using Peace.Lifelog.Security;

namespace Peace.Lifelog.LocationRecommendation;

public class LocationRecommendationValidation : IUserValidation
{
    public Response ValidateUser(Response response, AppPrincipal principal, string userHash)
    {
        var validateUserResponse = ValidateAppPrincipal(response, principal);
        validateUserResponse = IsValidUserHash(validateUserResponse, userHash);
        return validateUserResponse;
    }

    private Response IsValidUserHash(Response response, string userHash)
    {
        if (userHash is null || userHash == string.Empty)
        {
            response.HasError = true;
            response.ErrorMessage = "User Hash must not be empty";
            return response;
        }

        return response;
    }

    private Response ValidateAppPrincipal(Response response, AppPrincipal? principal)
    {
        if (principal == null)
        {
            response.HasError = true;
            response.ErrorMessage = "App Principal must not be empty";
            return response;
        }

        if (principal.UserId == null || principal.UserId == string.Empty)
        {
            response.HasError = true;
            response.ErrorMessage = "User Hash must not be empty";
            return response;
        }

        if (principal.Claims == null)
        {
            response.HasError = true;
            response.ErrorMessage = "Claims must not be empty";
            return response;
        }

        if (!principal.Claims.ContainsKey("Role"))
        {
            response.HasError = true;
            response.ErrorMessage = "Claims must contain the user role";
            return response;
        }

        return response;
    }
}