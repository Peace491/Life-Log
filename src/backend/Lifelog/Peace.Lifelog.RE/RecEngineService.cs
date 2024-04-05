namespace Peace.Lifelog.RecEngineService;

using Peace.Lifelog.Infrastructure;
using Peace.Lifelog.LLI;
using Peace.Lifelog.Logging;
using Peace.Lifelog.Security;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using DomainModels;
using ZstdSharp.Unsafe;

public class RecEngineService : IRecEngineService
{
    #region Fields

    // List of roles that are authorized to use the recommendation engine
    private List<string> authorizedRoles = new List<string>() { "Normal", "Admin", "Root" };

    // Dependency injected services
    private int expectedAttributeCount = 15; // Assuming this is the expected number of attributes in a recommendation
    private readonly IRecEngineRepo recEngineRepo;
    private readonly ILogging logger;
    private readonly ILifelogAuthService lifelogAuthService;

    #endregion

    #region Constructor

    // Constructor for dependency injection
    public RecEngineService(IRecEngineRepo recEngineRepo, ILogging logger, ILifelogAuthService lifelogAuthService)
    {
        this.recEngineRepo = recEngineRepo;
        this.logger = logger;
        this.lifelogAuthService = lifelogAuthService;
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Retrieves a specified number of recommendations for the given user.
    /// </summary>
    /// <param name="appPrincipal">The user's application principal containing user identity.</param>
    /// <param name="numRecs">The number of recommendations to retrieve.</param>
    /// <returns>A response object containing the recommendations or an error message.</returns>
    public async Task<Response> RecNumLLI(AppPrincipal appPrincipal, int numRecs)
    {
        var timer = new Stopwatch();
        var response = new Response();
        try
        {
            // Check if the user is authorized to use this service
            if (!IsUserAuthorized(appPrincipal))
            {
                response.ErrorMessage = "User is not authorized to access this service";
                _ = await logger.CreateLog("Logs", appPrincipal.UserId, "ERROR", "Buisness", response.ErrorMessage);
                return response;
            }

            // Validate the requested number of recommendations
            if (!ValidateNumRecs(numRecs))
            {
                response.ErrorMessage = "Invalid number of recommendations. Number of recommendations must be between 1 and 10";
                _ = await logger.CreateLog("Logs", appPrincipal.UserId, "ERROR", "Buisness", response.ErrorMessage);
                return response;
            }

            timer.Start(); // Start operation timer
            response = await recEngineRepo.GetNumRecs(appPrincipal.UserId, numRecs);
            timer.Stop(); // Stop operation timer

            // Check if the operation took too long
            if (!TimeOperation(timer))
            {
                response.ErrorMessage = "Operation took too long";
                _ = await logger.CreateLog("Logs", appPrincipal.UserId, "ERROR", "Business", response.ErrorMessage);
                return response;
            }

            // Convert the raw response into a list of LLI objects
            List<Object>? recommendedLLI = ConvertResponseToCleanLLI(response);

            response.Output = recommendedLLI; // Set the response output
        }
        catch (Exception ex)
        {
            // Log the exception and set an error message
            _ = await logger.CreateLog("Logs", appPrincipal.UserId, "ERROR", "Service", ex.Message);
            response.ErrorMessage = "An error occurred while processing your request.";
        }

        return response;
    }

    #endregion

    #region Private Helper Methods

    // Validates the number of recommendations requested
    private bool ValidateNumRecs(int numRecs) => numRecs >= 1 && numRecs <= 10;

    // Converts the raw response into a more structured list of LLI objects
    private List<Object>? ConvertResponseToCleanLLI(Response recommendations)
    {
        // Check if the recommendations output is null or empty
        if (recommendations.Output == null || !recommendations.Output.Any())
        {
            return null;
        }

        var lliList = new List<Object>();

        // Iterate through each recommendation, assuming each is a List<Object>
        foreach (List<object> recommendation in recommendations.Output)
        {
            // Check if the recommendation itself is not null and has the expected number of attributes
            if (recommendation == null || recommendation.Count < expectedAttributeCount) continue; // Assuming 'expectedAttributeCount' is defined

            // Instantiate a new LLI object
            var lli = new LLI();

            // Use a more resilient and clear way of assigning properties, avoiding magic numbers for indexes
            lli.LLIID = recommendation.ElementAtOrDefault(0)?.ToString() ?? "";
            lli.UserHash = recommendation.ElementAtOrDefault(1)?.ToString() ?? "";
            lli.Title = recommendation.ElementAtOrDefault(2)?.ToString() ?? "";
            lli.Description = recommendation.ElementAtOrDefault(3)?.ToString() ?? "";
            lli.Status = recommendation.ElementAtOrDefault(4)?.ToString() ?? "";
            lli.Visibility = recommendation.ElementAtOrDefault(5)?.ToString() ?? "";
            lli.Deadline = recommendation.ElementAtOrDefault(6)?.ToString() ?? "";
            lli.Cost = Convert.ToInt32(recommendation.ElementAtOrDefault(7) ?? 0);
            // Assuming Recurrence is a nested object within LLI and properly instantiated
            lli.Recurrence.Status = recommendation.ElementAtOrDefault(8)?.ToString() ?? "";
            lli.Recurrence.Frequency = recommendation.ElementAtOrDefault(9)?.ToString() ?? "";
            lli.CreationDate = recommendation.ElementAtOrDefault(10)?.ToString() ?? "";
            lli.CompletionDate = recommendation.ElementAtOrDefault(11)?.ToString() ?? "";
            lli.Category1 = recommendation.ElementAtOrDefault(12)?.ToString() ?? "None";
            lli.Category2 = recommendation.ElementAtOrDefault(13)?.ToString() ?? "None";
            lli.Category3 = recommendation.ElementAtOrDefault(14)?.ToString() ?? "None";

            lliList.Add(lli);
        }

        return lliList;
    }

    // Checks if the operation was completed in an acceptable amount of time
    private bool TimeOperation(Stopwatch timer) => timer.ElapsedMilliseconds < 3001;

    // Checks if the user is authorized to use this service
    private bool IsUserAuthorized(AppPrincipal appPrincipal) => lifelogAuthService.IsAuthorized(appPrincipal, authorizedRoles);

    #endregion
}
