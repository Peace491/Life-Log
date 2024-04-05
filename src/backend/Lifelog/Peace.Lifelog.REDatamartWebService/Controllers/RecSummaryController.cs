using System.Text.Json;
using back_end;
using Microsoft.AspNetCore.Mvc;
using Peace.Lifelog.Logging;
using Peace.Lifelog.RecSummaryService;
using Peace.Lifelog.Security;

namespace Peace.Lifelog.REDatamartWebService.Controllers;

[ApiController]
[Route("summary")]
public sealed class RecSummaryController : ControllerBase
{
    private IRecSummaryService recSummaryService;
    private readonly ILogging logger;
    private readonly IJWTService jwtService;

    public RecSummaryController(IRecSummaryService recSummaryService, ILogging logger, IJWTService jwtService)
    {
        this.recSummaryService = recSummaryService;
        this.logger = logger;
        this.jwtService = jwtService;
    }

    [HttpPost]
    [Route("UserRecSummary")]
    public async Task<IActionResult> UpdateRecommendationDataMartForUser(AppPrincipal principal)
    {
        try
        {
            if (Request.Headers == null)
            {
                return StatusCode(401);
            }

            var jwtToken = JsonSerializer.Deserialize<Jwt>(Request.Headers["Token"]!);

            if (jwtToken == null)
            {
                return StatusCode(401);
            }

            var userHash = jwtToken.Payload.UserHash;

            if (userHash == null)
            {
                return StatusCode(401);
            }

            if (!jwtService.IsJwtValid(jwtToken))
            {
                return StatusCode(401);
            }

            var response = await recSummaryService.UpdateUserRecSummary(principal);

            if (response.HasError)
            {
                _ = await logger.CreateLog("Logs", "RecSummaryController", "INFO", "System", $"error: {response.ErrorMessage}");
                return BadRequest("Operation failed. Please try again.");
            }
        
            _ = await logger.CreateLog("Logs", principal.UserId, "INFO", "System", $"Summary data mart updated for user {principal.UserId}");
            return Ok(response);
        }
        catch (Exception ex)
        {
            // Log the exception details here
            _ = await logger.CreateLog("Logs", "RecSummaryController", "INFO", "System", $"error: {ex.Message}");
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    // Update recommendation data mart for ALL users
    [HttpPost]
    [Route("AllUserRecSummary")]
    public async Task<IActionResult> UpdateRecommendationDataMartForAllUsers(AppPrincipal principal)
    {
        try
        {
            // Check that the user is an admin thru token
            var jwtToken = JsonSerializer.Deserialize<Jwt>(Request.Headers["Token"]!);

            if (jwtToken == null)
            {
                return StatusCode(401);
            }

            var statusCode = jwtService.ProcessToken(Request);

            if (statusCode == 401)
            {
                return StatusCode(401, "Unauthorized");
            }

            _ = await logger.CreateLog("Logs", "RecSummaryController", "INFO", "System", $"Summary data mart updated for all users");
            
            var response = await recSummaryService.UpdateAllUserRecSummary(principal);
 
            return Ok(response);
        }
        catch (Exception ex)
        {
            // Log the exception details here
            _ = await logger.CreateLog("Logs", "RecSummaryController", "INFO", "System", $"error: {ex.Message}");
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }
}
