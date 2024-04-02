namespace Peace.Lifelog.RecSummaryService;

using DomainModels;
using Peace.Lifelog.DataAccess;

public class RecSummaryService : IRecSummaryService
{
    // Inject summary repo through the constructor
    // Summary repo will need a read only dao
    private readonly SummaryRepository summaryRepository;

    public RecSummaryService(SummaryRepository summaryRepository)
    {
        this.summaryRepository = summaryRepository;
    }

    // only allow users to do this Y times a day
    public async Task<Response> updateUserRecSummary(string userHash)
    {
        var response = new Response();
        try
        {
            // Get userform
            response = await summaryRepository.GetUserForm(userHash);
            // Init scoring with userform
            var userScores = scoreInit(response);

            // TODO : Evaluate for biz rules

            // Get userLLI
            response = await summaryRepository.GetNumUserLLI(userHash, null);
            // Update scores with userLLI
            var scoreDict = scoreLLI(userScores, response);

            // TODO : Evaluate for biz rules

            // Get the user's two highest scoring categories
            var topTwoCategories = getTopTwoCategories(scoreDict);

            // Update the user's data mart with the two highest scoring categories
            response = await summaryRepository.UpdateUserDataMart(userHash, topTwoCategories[0], topTwoCategories[1]);

            // TODO : Evaluate for biz rules
        }
        catch (Exception ex)
        {
            // Log exception details here using your preferred logging framework
            response.ErrorMessage = "An error occurred while processing your request.";
        }
        return response;
    }

    public async Task<Response> updateSystemUserRecSummary()
    {
        // This method is horrid right now - need to fix takign a break for the night
        var mostPopularCategory = await summaryRepository.GetMostPopularCategory();

        foreach (List<Object> category in mostPopularCategory.Output)
        {
            if (category[0] == null)
            {
                mostPopularCategory.ErrorMessage = "An error occurred while processing your request.";
                return mostPopularCategory;
            }
            var updateDataMartResponse = await summaryRepository.UpdateUserDataMart("System", category[0].ToString(), null);
            return updateDataMartResponse;
        }
        return mostPopularCategory;
    }


    // only system admins can do this, and only once a day
    public async Task<Response> updateAllUserRecSummary()
    {
        var response = new Response();
        try
        {
            // get all userHashes
            var allUserHashResponse = await summaryRepository.GetAllUserHash();

            // Num of users
            int numUsers = allUserHashResponse.Output.Count;
            int numUsersProcessed = 0;
            // for each userhash, updateRecommendationDatMartForUser(userhash)
            foreach (List<Object> userHash in allUserHashResponse.Output)
            {
                string currentHash = userHash?[0]?.ToString() ?? string.Empty;
                if (currentHash == "System")  
                {
                    var updateDataMartResponse = await updateSystemUserRecSummary(); 
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

    private Dictionary<string, int> scoreInit(Response formResponse)
    {
        Dictionary<string, int> userScores = new Dictionary<string, int>();
        if (formResponse.Output == null) return userScores;
        foreach (List<Object> pair in formResponse.Output)
        {
            if (pair[0] == null || pair[1] == null || pair == null)
            {
                return userScores;
            }
            
            string category = pair[0].ToString();
            int rank = Convert.ToInt32(pair[1]);
            switch (rank)
            // Score categories using f1 scoring system
            {
                case 1:
                    userScores.Add(category, 25);
                    break;
                case 2:
                    userScores.Add(category, 18);
                    break;
                case 3:
                    userScores.Add(category, 15);
                    break;
                case 4:
                    userScores.Add(category, 12);
                    break;
                case 5:
                    userScores.Add(category, 10);
                    break;
                case 6:
                    userScores.Add(category, 8);
                    break;
                case 7:
                    userScores.Add(category, 6);
                    break;
                case 8:
                    userScores.Add(category, 4);
                    break;
                case 9:
                    userScores.Add(category, 2);
                    break;
                case 10:
                    userScores.Add(category, 1);
                    break;
                default:
                    userScores.Add(category, 0);
                    break;
            }
        }
        return userScores;
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
