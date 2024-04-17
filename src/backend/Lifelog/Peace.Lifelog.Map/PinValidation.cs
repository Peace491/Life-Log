using DomainModels;
using Peace.Lifelog.Security;

namespace Peace.Lifelog.Map;

public class PinValidation : IPinValidation
{
    public Response ValidatePinRequest(Response response, IPinRequest pinRequest, string requestType)
    {
        var validateAppPrincipalResponse = ValidateAppPrincipal(response, pinRequest.Principal);
        if (validateAppPrincipalResponse.HasError)
        {
            return HandleValidationError(response, validateAppPrincipalResponse.ErrorMessage!);
        }

        var validatePinValueResponse = ValidatePinValue(response, requestType, pinRequest.PinId, pinRequest.LLIId, pinRequest.Address, pinRequest.Latitude, pinRequest.Longitude);
        if (validatePinValueResponse.HasError)
        {
            return HandleValidationError(response, validatePinValueResponse.ErrorMessage!);
        }

        return response;

    }

    public bool IsValidUserHash(string userHash)
    {
        if (userHash is null || userHash == string.Empty)
        {
            return false;
        }

        return true;
    }

    private Response ValidateAppPrincipal(Response response, AppPrincipal? appPrincipal)
    {
        if (appPrincipal == null)
        {
            response.HasError = true;
            response.ErrorMessage = "App Principal must not be empty";
            return response;
        }

        if (appPrincipal.UserId == null || appPrincipal.UserId == string.Empty)
        {
            response.HasError = true;
            response.ErrorMessage = "User Hash must not be empty";
            return response;
        }

        if (appPrincipal.Claims == null)
        {
            response.HasError = true;
            response.ErrorMessage = "Claims must not be empty";
            return response;
        }

        if (!appPrincipal.Claims.ContainsKey("Role"))
        {
            response.HasError = true;
            response.ErrorMessage = "Claims must contain the user role";
            return response;
        }

        return response;
    }

    private Response ValidatePinValue(
        Response response,
        string requestType,
        string PinId,
        string LLIId,
        string Address,
        double Latitude,
        double Longitude
    )
    {
        if (requestType == PinRequestType.Create || requestType == PinRequestType.Update)
        {
            if (LLIId == string.Empty || Address == string.Empty || Latitude == 0 || Longitude == 0)
            {
                response.HasError = true;
                response.ErrorMessage = "Invalid Field entry";
            }

        }
        if (requestType == PinRequestType.Delete)
        {
            if (PinId == string.Empty)
            {
                response.HasError = true;
                response.ErrorMessage = "Invalid pin selection";
            }

        }
        if (requestType == PinRequestType.Edit)
        {
            if (LLIId == string.Empty)
            {
                response.HasError = true;
                response.ErrorMessage = "Invalid LLI";
            }
        }
        if (requestType == PinRequestType.View)
        {
            if (LLIId == string.Empty || PinId == string.Empty)
            {
                response.HasError = true;
                response.ErrorMessage = "Invalid Pin selection";
            }

        }
        return response;
    }


    private Response HandleValidationError(Response response, string errorMessage)
    {
        response.HasError = true;
        response.ErrorMessage = errorMessage;
        return response;
    }
