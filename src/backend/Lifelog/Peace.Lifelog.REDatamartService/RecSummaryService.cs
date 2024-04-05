namespace Peace.Lifelog.RecSummaryService;

using System.Collections;
using DomainModels;
using Peace.Lifelog.DataAccess;
using Peace.Lifelog.Infrastructure;
using Peace.Lifelog.Logging;

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
    public async Task<Response> updateUserRecSummary(string userHash)
    {
        var response = new Response();
        try
        {
            // Get userform
            response = await recSummaryRepo.GetUserForm(userHash);

            if (response.Output == null)
            {
                response.HasError = true;
                response.ErrorMessage = "An error occurred fetching user form.";
                return response;
            }
            // Init scoring with userform
            var userScores = scoreInit(response);

            // TODO : Evaluate for biz rules

            // Get userLLI
            response = await recSummaryRepo.GetNumUserLLI(userHash, null);
            // Update scores with userLLI
            var scoreDict = scoreLLI(userScores, response);

            // TODO : Evaluate for biz rules

            // Get the user's two highest scoring categories
            var topTwoCategories = getTopTwoCategories(scoreDict);

            // Get system most popular category
            response = await recSummaryRepo.GetMostPopularCategory();

            // Update the user's data mart with the two highest scoring categories
            response = await recSummaryRepo.UpdateUserDataMart(userHash, topTwoCategories[0], topTwoCategories[1]);

            // TODO : Evaluate for biz rules
        }
        catch (Exception ex)
        {
            // Log exception details here using your preferred logging framework
            response.ErrorMessage = "An error occurred while processing your request.";
        }
        return response;
    }

    public async Task<Response> UpdateSystemUserRecSummary()
    {
        var mostPopularCategoryResponse = await recSummaryRepo.GetMostPopularCategory();
        if (mostPopularCategoryResponse == null || mostPopularCategoryResponse.Output == null || !mostPopularCategoryResponse.Output.Any())
        {
            return new Response
            {
                ErrorMessage = "No categories found or an error occurred."
            };
        }

        // Assuming the intent is to update only the first valid category
        // Cast firstCategory explicitly to List<object>
        var firstCategory = mostPopularCategoryResponse.Output.FirstOrDefault() as List<object>;
        if (firstCategory == null || !firstCategory.Any() || firstCategory[0] == null)
        {
            return new Response
            {
                ErrorMessage = "First category is invalid or not found."
            };
        }

        // Proceed with updating the Data Mart for the first valid category
        var categoryToUpdate = firstCategory[0].ToString();
        var updateDataMartResponse = await recSummaryRepo.UpdateUserDataMart("System", categoryToUpdate, categoryToUpdate);
        return updateDataMartResponse;
    }


    public async Task<Response> updateAllUserRecSummary()
    {
        var response = new Response();
        try
        {
            // get all userHashes
            var allUserHashResponse = await recSummaryRepo.GetAllUserHash();

            // Num of users
            int numUsers = allUserHashResponse.Output.Count;
            int numUsersProcessed = 0;

            // for each userhash, updateRecommendationDatMartForUser(userhash)

            response = await UpdateSystemUserRecSummary(); // update system first to get most popular category

            foreach (List<Object> userHash in allUserHashResponse.Output)
            {
                string currentHash = userHash?[0]?.ToString() ?? string.Empty;
                if (currentHash == "System")
                {
                    continue;
                }
                else
                {
                    var updateDataMartResponse = await updateUserRecSummary(currentHash);
                }
                numUsersProcessed++;
            }
            response.HasError = false;
            response.Output = [numUsersProcessed];
        }
        catch (Exception ex)
        {
            // Log exception details here using your preferred logging framework
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
                if (nonNoneCategories > 1) points = 3; // Adjust points here based on your requirement
                if (nonNoneCategories > 2) points = 2; // Adjust points here based on your requirement

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

        foreach (List<Object> row in response.Output)
        {
            for (int i = 0; i < row.Count; i++)
            {
                try
                {
                    // Assuming ScoreHelper can take an int and does something with it
                    // Convert row[i] to int before passing to ScoreHelper
                    int score = Convert.ToInt32(row[i]);
                    userScores.Add(categories[i], ScoreHelper(score));
                }
                catch (Exception ex)
                {
                    // Handle or log the exception as needed
                    // For example, you might want to log conversion errors or continue with default values
                    Console.WriteLine($"Error converting score for category {categories[i]}: {ex.Message}");
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
