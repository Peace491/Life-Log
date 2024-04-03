using DomainModels;
using Peace.Lifelog.Security;

namespace Peace.Lifelog.UserForm;

public class UserFormValidation : IUserFormValidation
{
    private const int NUM_OF_CATEGORIES = 10;

    public Response ValidateUserFormRequest(Response response, IUserFormRequest createUserFormRequest, string requestType)
    {
        var validateAppPrincipalResponse = ValidateAppPrincipal(response, createUserFormRequest.Principal);
        if (validateAppPrincipalResponse.HasError)
        {
            return HandleValidationError(response, validateAppPrincipalResponse.ErrorMessage!);
        }

        var validateUserFormFieldInputValueResponse = ValidateUserFormFieldInputValue(response, requestType, createUserFormRequest.MentalHealthRating, createUserFormRequest.PhysicalHealthRating, createUserFormRequest.OutdoorRating, createUserFormRequest.SportRating, createUserFormRequest.ArtRating, createUserFormRequest.HobbyRating, createUserFormRequest.ThrillRating, createUserFormRequest.TravelRating, createUserFormRequest.VolunteeringRating, createUserFormRequest.FoodRating);
        if (validateUserFormFieldInputValueResponse.HasError)
        {
            return HandleValidationError(response, validateUserFormFieldInputValueResponse.ErrorMessage!);
        }

        var validateUserFormFieldUniquenessResponse = ValidateUserFormFieldUniqueness(response, requestType ,createUserFormRequest.MentalHealthRating, createUserFormRequest.PhysicalHealthRating, createUserFormRequest.OutdoorRating, createUserFormRequest.SportRating, createUserFormRequest.ArtRating, createUserFormRequest.HobbyRating, createUserFormRequest.ThrillRating, createUserFormRequest.TravelRating, createUserFormRequest.VolunteeringRating, createUserFormRequest.FoodRating);
        if (validateUserFormFieldUniquenessResponse.HasError)
        {
            return HandleValidationError(response, validateUserFormFieldUniquenessResponse.ErrorMessage!);
        }

        return response;

    }

    public bool IsValidUserHash(string userHash) {
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

    private Response ValidateUserFormFieldInputValue(
        Response response,
        string requestType,
        int mentalHealthRating, int physicalHealthRating, int outdoorRating,
        int sportRating, int artRating, int hobbyRating,
        int thrillRating, int travelRating, int volunteeringRating,
        int foodRating
    )
    {
        int MIN_RANKING = 1;
        if (requestType == UserFormRequestType.Update)
        {
            MIN_RANKING = 0;
        }

        int MAX_RANKING = 10;

        if (
            mentalHealthRating < MIN_RANKING || mentalHealthRating > MAX_RANKING
            || physicalHealthRating < MIN_RANKING || physicalHealthRating > MAX_RANKING
            || outdoorRating < MIN_RANKING || outdoorRating > MAX_RANKING
            || sportRating < MIN_RANKING || sportRating > MAX_RANKING
            || artRating < MIN_RANKING || artRating > MAX_RANKING
            || hobbyRating < MIN_RANKING || hobbyRating > MAX_RANKING
            || thrillRating < MIN_RANKING || thrillRating > MAX_RANKING
            || travelRating < MIN_RANKING || travelRating > MAX_RANKING
            || volunteeringRating < MIN_RANKING || volunteeringRating > MAX_RANKING
            || foodRating < MIN_RANKING || foodRating > MAX_RANKING
        )
        {
            response.HasError = true;
            response.ErrorMessage = "The LLI rankings are not in range";
        }

        return response;
    }

    private Response ValidateUserFormFieldUniqueness(
        Response response,
        string requestType,
        int mentalHealthRating, int physicalHealthRating, int outdoorRating,
        int sportRating, int artRating, int hobbyRating,
        int thrillRating, int travelRating, int volunteeringRating,
        int foodRating
    )
    {
        HashSet<int> uniqueValues = new HashSet<int>();

        // Add all ratings to the HashSet to check for uniqueness and presence
        if (mentalHealthRating != 0) uniqueValues.Add(mentalHealthRating);
        if (physicalHealthRating != 0) uniqueValues.Add(physicalHealthRating);
        if (outdoorRating != 0) uniqueValues.Add(outdoorRating);
        if (sportRating != 0) uniqueValues.Add(sportRating);
        if (artRating != 0) uniqueValues.Add(artRating);
        if (hobbyRating != 0) uniqueValues.Add(hobbyRating);
        if (thrillRating != 0) uniqueValues.Add(thrillRating);
        if (travelRating != 0) uniqueValues.Add(travelRating);
        if (volunteeringRating != 0) uniqueValues.Add(volunteeringRating);
        if (foodRating != 0) uniqueValues.Add(foodRating);

        // Check if non-zero ratings are unique
        if (uniqueValues.Count != CountNonZeroRatings(mentalHealthRating, physicalHealthRating, outdoorRating, sportRating, artRating, hobbyRating, thrillRating, travelRating, volunteeringRating, foodRating))
        {
            response.HasError = false;
            response.ErrorMessage = "The LLI rankings are not unique";
            return response;
        }

        // Check if all numbers from 1 to 10 are present, if we are validating a create
        if (requestType == UserFormRequestType.Create)
        {
            for (int i = 1; i <= NUM_OF_CATEGORIES; i++)
        {
            if (!uniqueValues.Contains(i))
            {
                response.HasError = true;
                response.ErrorMessage = "The LLI rankings are incomplete";
                return response;
            }
        }
        }

        // All validations passed
        return response;
    }

    // Helper function to count the number of non-zero ratings
    private int CountNonZeroRatings(
        int mentalHealthRating, int physicalHealthRating, int outdoorRating,
        int sportRating, int artRating, int hobbyRating,
        int thrillRating, int travelRating, int volunteeringRating,
        int foodRating
    )
    {
        int count = 0;
        int[] ratings = { mentalHealthRating, physicalHealthRating, outdoorRating, sportRating, artRating, hobbyRating, thrillRating, travelRating, volunteeringRating, foodRating };

        foreach (int rating in ratings)
        {
            if (rating != 0)
            {
                count++;
            }
        }

        return count;
    }

    // Helper function to check if a rating is non-zero
    private bool IsNonZeroRating(int rating)
    {
        return rating != 0;
    }


    private Response HandleValidationError(Response response, string errorMessage)
    {
        response.HasError = true;
        response.ErrorMessage = errorMessage;
        return response;
    }
}
