namespace Peace.Lifelog.RecSummaryService;

using DomainModels;
using Peace.Lifelog.Infrastructure;
using Peace.Lifelog.Logging;
using Peace.Lifelog.Security;

public class RecSummaryService : IRecSummaryService
{
    // Inject summary repo through the constructor
    // Summary repo will need a read only dao
    private readonly IRecSummaryRepo recSummaryRepo;
    private readonly ILogging logger;

    public RecSummaryService(IRecSummaryRepo recSummaryRepo, ILogging logger)
    {
        this.recSummaryRepo = recSummaryRepo;
        this.logger = logger;
    }

    // only allow users to do this Y times a day
    public async Task<Response> UpdateUserRecSummary(AppPrincipal principal)
    {
        Response response = new Response();
        try
        {
            // Get userform
            response = await recSummaryRepo.GetUserForm(principal.UserId);

            if (response.Output == null)
            {
                response.HasError = true;
                response.ErrorMessage = "An error occurred fetching user form.";
                _ = await logger.CreateLog("Logs", principal.UserId, "ERROR", "Buisness", response.ErrorMessage);
                return response;
            }
            // Init scoring with userform
            var userScores = scoreInit(response);


            // Get userLLI
            response = await recSummaryRepo.GetNumUserLLI(principal.UserId, null);
            // Update scores with userLLI
            var scoreDict = scoreLLI(userScores, response);


            // Get the user's two highest scoring categories
            var topTwoCategories = getTopTwoCategories(scoreDict);

            // Update the user's data mart with the two highest scoring categories
            response = await recSummaryRepo.UpdateUserDataMart(principal.UserId, topTwoCategories[0], topTwoCategories[1]);

        }
        catch (Exception ex)
        {
            _ = await logger.CreateLog("Logs", "RecSummaryService", "ERROR", "Buisness", ex.Message);
            response.ErrorMessage = ex.Message;
            return response;
        }
        return response;
    }

    public async Task<Response> UpdateSystemUserRecSummary()
    {
        Response response = new Response();
        var mostPopularCategoryResponse = await recSummaryRepo.GetMostPopularCategory();
        if (mostPopularCategoryResponse == null || mostPopularCategoryResponse.Output == null || !mostPopularCategoryResponse.Output.Any())
        {
            response.ErrorMessage = "No categories found or an error occurred.";
            return response;
        }

        // Assuming the intent is to update only the first valid category
        // Cast firstCategory explicitly to List<object>
        var firstCategory = mostPopularCategoryResponse.Output.FirstOrDefault() as List<object>;
        if (firstCategory == null || !firstCategory.Any() || firstCategory[0] == null)
        {
            response.ErrorMessage = "No categories found or an error occurred.";
            return response;
        }

        // Proceed with updating the Data Mart for the first valid category
        var categoryToUpdate = firstCategory[0].ToString();
        if (categoryToUpdate != null)
        {
            _ = await logger.CreateLog("Logs", "RecSummaryService", "INFO", "Buisness", $"Updating system user with category: {categoryToUpdate}");
            return await recSummaryRepo.UpdateUserDataMart("System", categoryToUpdate, categoryToUpdate);
        }
        else
        {
            return new Response
            {
                ErrorMessage = "Category to update is invalid."
            };
        }
    }


    public async Task<Response> UpdateAllUserRecSummary(AppPrincipal principal)
    {
        var response = new Response();
        try
        {
            if (principal == null || principal.Claims == null || (principal.Claims["Role"] != "Admin" && principal.Claims["Role"] != "Root"))
            {
                response.ErrorMessage = "Unauthorized";
                return response;
            }

            // get all userHashes
            var allUserHashResponse = await recSummaryRepo.GetAllUserHash();

            // Num of users
            int numUsers = allUserHashResponse.Output?.Count ?? 0;
            int numUsersProcessed = 0;

            // for each userhash, updateRecommendationDatMartForUser(userhash)

            response = await UpdateSystemUserRecSummary(); // update system first to get most popular category

            if (allUserHashResponse.Output != null)
            {
                foreach (List<Object> userHash in allUserHashResponse.Output)
                {
                    string currentHash = userHash?[0]?.ToString() ?? string.Empty;
                    if (currentHash == "System")
                    {
                        continue;
                    }
                    else
                    {
                        principal.UserId = currentHash;
                        var updateDataMartResponse = await UpdateUserRecSummary(principal);
                    }
                    numUsersProcessed++;
                }
            }            
            response.HasError = false;
            response.Output = [numUsersProcessed];
        }
        catch (Exception ex)
        {
            _ = await logger.CreateLog("Logs", principal.UserId, "ERROR", "System", $"An error occurred while processing your request: {ex.Message}");
            response.ErrorMessage = "An error occurred while processing your request.";
        }
       
        return response;
    }

    private Dictionary<string, int> scoreLLI(Dictionary<string, int> scoreDict, Response lliResponse)
    {
        if (lliResponse.Output != null)
        {
            foreach (List<Object> lli in lliResponse.Output)
            {
                int statusMultiplier;
                switch (lli[0]) // index by whereever status is in the lli response
                {
                    case "Postponed":
                        statusMultiplier = (int)0.8;
                        break;
                    case "Active":
                        statusMultiplier = 1;
                        break;
                    case "Completed":
                        statusMultiplier = (int)1.2;
                        break;
                    default:
                        statusMultiplier = 1;
                        break;
                }

                string? currentCategory1 = lli[1]?.ToString();
                string? currentCategory2 = lli[2]?.ToString();
                string? currentCategory3 = lli[3]?.ToString();

                int nonNoneCategories = 0;
                if (currentCategory1 != null && currentCategory1 != "None") nonNoneCategories++;
                if (currentCategory2 != null && currentCategory2 != "None") nonNoneCategories++;
                if (currentCategory3 != null && currentCategory3 != "None") nonNoneCategories++;

                int points = 6;
                if (nonNoneCategories > 1) points = 3; 
                if (nonNoneCategories > 2) points = 2; 

                if (currentCategory1 != null && scoreDict.ContainsKey(currentCategory1)) // contains key category
                {
                    scoreDict[currentCategory1] += points * statusMultiplier;
                }

                if (currentCategory2 != null && scoreDict.ContainsKey(currentCategory2)) // contains key category
                {
                    scoreDict[currentCategory2] += points * statusMultiplier;
                }

                if (currentCategory3 != null && scoreDict.ContainsKey(currentCategory3)) // contains key category
                {
                    scoreDict[currentCategory3] += points * statusMultiplier;
                }
            }
        }
        return scoreDict;
    }

    private Dictionary<string, int> scoreInit(Response response)
    {
        Dictionary<string, int> userScores = new Dictionary<string, int>();

        List<string> categories = new List<string>
    {
        "Mental Health",
        "Physical Health",
        "Outdoor",
        "Sport",
        "Art",
        "Hobby",
        "Thrill",
        "Travel",
        "Volunteering",
        "Food"
    };

        if (response.Output != null)
        {
            foreach (List<Object> row in response.Output)
            {
                for (int i = 0; i < row.Count; i++)
                {
                    int score = Convert.ToInt32(row[i]);
                    userScores.Add(categories[i], ScoreHelper(score));
                }
            }
        }
        return userScores;
    }


    private int ScoreHelper(int input)
    {
        switch (input)
        {
            case 1:
                return 25;
            case 2:
                return 18;
            case 3:
                return 15;
            case 4:
                return 12;
            case 5:
                return 10;
            case 6:
                return 8;
            case 7:
                return 6;
            case 8:
                return 4;
            case 9:
                return 2;
            case 10:
                return 1;
            default:
                return 0;
        }
    }


    private List<string> getTopTwoCategories(Dictionary<string, int> scoreDict)
    {
        // https://stackoverflow.com/questions/22957537/dictionary-getting-top-most-elements
        var topTwoKeys = scoreDict.OrderByDescending(kvp => kvp.Value)
                                    .Take(2)
                                    .Select(kvp => kvp.Key)
                                    .ToList();
        return topTwoKeys;
    }
}
