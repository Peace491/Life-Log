namespace Peace.Lifelog.RE;

using Peace.Lifelog.DataAccess;
using Peace.Lifelog.LLI;
using Peace.Lifelog.Logging;
using System.Diagnostics;
using DomainModels;
using Peace.Lifelog.RecEngineService;

public class REService : IRecEngineService
{
    private readonly IRecomendationEngineRepository recomendationEngineRepository;
    private readonly ILogging logger;

    // Inject RecomendationEngineRepository through the constructor
    public REService(IRecomendationEngineRepository recomendationEngineRepository, ILogging logger)
    {
        this.recomendationEngineRepository = recomendationEngineRepository;
        this.logger = logger;
    }
    
    public async Task<Response> getNumRecs(string userhash, int numRecs)
    {
        var timer = new Stopwatch();
        var response = new Response();
        try
        {
            if (!validateNumRecs(numRecs))
            {
                response.ErrorMessage = "Invalid number of recommendations. Number of recommendations must be between 1 and 10";
                return response;
            }

            // Start timer
            timer.Start();

            // Preform operation
            response = await recomendationEngineRepository.GetNumRecs(userhash, numRecs);

            // Stop timer
            timer.Stop();

            if (!timeOperation(timer))
            {
                response.ErrorMessage = "Operation took too long";
                return response;
            }

            // TODO : Method to convert response to LLI objects
            List<object> recommendedLLI = convertResponseToCleanLLI(response) ?? new List<object>();

            if (!validateLLI(recommendedLLI))
            {
                response.ErrorMessage = "One or more LLI is invalid";
                return response;
            }

            // If no error
            response.Output = recommendedLLI;
        }
        catch (Exception ex)
        {
            // TODO specifiy logging stuff
            var logResponse = await logger.CreateLog("Logs", userhash, "ERROR", "Service", ex.Message);
            response.ErrorMessage = "An error occurred while processing your request.";
        }

        return response;
    }

    
    // Helper Functions
    private bool validateNumRecs(int numRecs)
    {
        if (numRecs < 1 || numRecs > 10)
        {
            return false;
        }
        return true;
    }

    private List<Object>? convertResponseToCleanLLI(Response recomendations)
    {
        List<object> lliList = new List<object>();

        if (recomendations.Output == null) return null;

        foreach (List<Object> LLI in recomendations.Output)
        {
            var lli = new LLI();
            int index = 0;
            foreach (var attribute in LLI)
            {
                if (attribute is null) continue;

                switch (index)
                {
                    case 0:
                        lli.LLIID = attribute.ToString() ?? "";
                        break;
                    case 1:
                        lli.UserHash = attribute.ToString() ?? "";
                        break;
                    case 2:
                        lli.Title = attribute.ToString() ?? "";
                        break;
                    case 3:
                        lli.Description = attribute.ToString() ?? "";
                        break;
                    case 4:
                        lli.Status = attribute.ToString() ?? "";
                        break;
                    case 5:
                        lli.Visibility = attribute.ToString() ?? "";
                        break;
                    case 6:
                        lli.Deadline = attribute.ToString() ?? "";
                        break;
                    case 7:
                        lli.Cost = Convert.ToInt32(attribute);
                        break;
                    case 8:
                        lli.Recurrence.Status = attribute.ToString() ?? "";
                        break;
                    case 9:
                        lli.Recurrence.Frequency = attribute.ToString() ?? "";
                        break;
                    case 10:
                        lli.CreationDate = attribute.ToString() ?? "";
                        break;
                    case 11:
                        lli.CompletionDate = attribute.ToString() ?? "";
                        break;
                    case 12:
                        lli.Category1 = attribute.ToString() ?? "None";
                        break;
                    case 13:
                        lli.Category2 = attribute.ToString() ?? "None";
                        break;
                    case 14:
                        lli.Category3 = attribute.ToString() ?? "None";
                        break;
                    default:
                        break;
                }
                index++;
            }
            // Add to return list
            lliList.Add(lli);
        }
        return lliList;
    }

    private bool validateLLI(List<Object> recs)
    {
        List<object> validLLI = new List<Object>();
        return true;
    }
    private bool timeOperation(Stopwatch timer)
    {
        // If the operation takes less than 3 seconds, return true
        if (timer.ElapsedMilliseconds < 3001)
        {
            return true;
        }
        return false;
    }
}
