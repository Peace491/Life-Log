using DomainModels;
using Peace.Lifelog.Security;

namespace Peace.Lifelog.UserForm;

public class UserFormValidation : IUserFormValidation
{
    private const int NUM_OF_CATEGORIES = 10;

    public Response validateCreateUserFormRequest(Response response, CreateUserFormRequest createUserFormRequest)
    {
        var validateAppPrincipalResponse = validateAppPrincipal(response, createUserFormRequest.Principal);
        if (validateAppPrincipalResponse.HasError)
        {
            return handleValidationError(response, validateAppPrincipalResponse.ErrorMessage!);
        }

        var validateUserFormFieldInputValueResponse = validateUserFormFieldInputValue(response, createUserFormRequest.MentalHealthRating, createUserFormRequest.PhysicalHealthRating, createUserFormRequest.OutdoorRating, createUserFormRequest.SportRating, createUserFormRequest.ArtRating, createUserFormRequest.HobbyRating, createUserFormRequest.ThrillRating, createUserFormRequest.TravelRating, createUserFormRequest.VolunteeringRating, createUserFormRequest.FoodRating);
        if (validateUserFormFieldInputValueResponse.HasError)
        {
            return handleValidationError(response, validateUserFormFieldInputValueResponse.ErrorMessage!);
        }

        var validateUserFormFieldUniquenessResponse = validateUserFormFieldUniqueness(response, createUserFormRequest.MentalHealthRating, createUserFormRequest.PhysicalHealthRating, createUserFormRequest.OutdoorRating, createUserFormRequest.SportRating, createUserFormRequest.ArtRating, createUserFormRequest.HobbyRating, createUserFormRequest.ThrillRating, createUserFormRequest.TravelRating, createUserFormRequest.VolunteeringRating, createUserFormRequest.FoodRating);
        if (validateUserFormFieldUniquenessResponse.HasError)
        {
            return handleValidationError(response, validateUserFormFieldUniquenessResponse.ErrorMessage!);
        }

        return response;

    }

    public Response validateAppPrincipal(Response response, AppPrincipal? appPrincipal)
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

        if (!appPrincipal.Claims.ContainsKey("RoleName"))
        {
            response.HasError = true;
            response.ErrorMessage = "Claims must contain the user role";
            return response;
        }

        return response;
    }

    public Response validateUserFormFieldInputValue(
        Response response,
        int mentalHealthRating, int physicalHealthRating, int outdoorRating,
        int sportRating, int artRating, int hobbyRating,
        int thrillRating, int travelRating, int volunteeringRating,
        int foodRating
    )
    {
        if (
            mentalHealthRating < 1 || mentalHealthRating > 10
            || physicalHealthRating < 1 || physicalHealthRating > 10
            || outdoorRating < 1 || outdoorRating > 10
            || sportRating < 1 || sportRating > 10
            || artRating < 1 || artRating > 10
            || hobbyRating < 1 || hobbyRating > 10
            || thrillRating < 1 || thrillRating > 10
            || travelRating < 1 || travelRating > 10
            || volunteeringRating < 1 || volunteeringRating > 10
            || foodRating < 1 || foodRating > 10
        )
        {
            response.HasError = true;
            response.ErrorMessage = "The LLI rankings are not in range";
        }

        return response;
    }

    private Response validateUserFormFieldUniqueness(
        Response response,
        int mentalHealthRating, int physicalHealthRating, int outdoorRating,
        int sportRating, int artRating, int hobbyRating,
        int thrillRating, int travelRating, int volunteeringRating,
        int foodRating
    )
    {
        HashSet<int> uniqueValues = new HashSet<int>();

        // Add all ratings to the HashSet to check for uniqueness and presence
        uniqueValues.Add(mentalHealthRating);
        uniqueValues.Add(physicalHealthRating);
        uniqueValues.Add(outdoorRating);
        uniqueValues.Add(sportRating);
        uniqueValues.Add(artRating);
        uniqueValues.Add(hobbyRating);
        uniqueValues.Add(thrillRating);
        uniqueValues.Add(travelRating);
        uniqueValues.Add(volunteeringRating);
        uniqueValues.Add(foodRating);

        // Check if all ratings are unique
        if (uniqueValues.Count != NUM_OF_CATEGORIES)
        {
            response.HasError = true;
            response.ErrorMessage = "The LLI rankings are not unique";
            return response;
        }

        // Check if all numbers from 1 to 10 are present
        for (int i = 1; i <= NUM_OF_CATEGORIES; i++)
        {
            if (!uniqueValues.Contains(i))
            {
                response.HasError = true;
                response.ErrorMessage = "The LLI rankings are incomplete";
                return response;
            }
        }

        // All validations passed
        return response;
    }


    private Response handleValidationError(Response response, string errorMessage)
    {
        response.HasError = true;
        response.ErrorMessage = errorMessage;
        return response;
    }
}
