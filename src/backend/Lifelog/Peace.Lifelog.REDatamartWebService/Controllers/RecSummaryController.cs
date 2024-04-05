using System.Text.Json;
using back_end;
using Microsoft.AspNetCore.Mvc;
using Peace.Lifelog.Logging;
using Peace.Lifelog.RecSummaryService;
using Peace.Lifelog.Security;

namespace Peace.Lifelog.REDatamartWebService.Controllers;

[ApiController]
[Route("summary")]
public class RecSummaryController : ControllerBase
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

    [HttpGet]
    [Route("UserRecSummary")]
    public async Task<IActionResult> UpdateRecommendationDataMartForUser(string userHash)
    {
        try
        {
            var statusCode = jwtService.ProcessToken(Request);

            if (statusCode == 401)
            {
                return StatusCode(401, "Unauthorized");
            }

            var response = await recSummaryService.updateUserRecSummary(userHash);

            // Consider checking response for errors and handling them accordingly
            if (response.HasError)
            {
                return BadRequest(response.ErrorMessage);
            }

            return Ok(response);
        }
        catch (Exception ex)
        {
            // Log the exception details here
            // Return a generic error message to the client, optionally with a custom error object
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    // Update recommendation data mart for ALL users
    [HttpGet]
    [Route("AllUserRecSummary")]
    public async Task<IActionResult> UpdateRecommendationDataMartForAllUsers()
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

            // if role is admin or root, do stuff
            if (jwtToken.Payload.Claims["Role"] == "Admin" || jwtToken.Payload.Claims["Role"] != "Root")
            {
                var response = await recSummaryService.updateAllUserRecSummary();
                if (response.HasError)
                {
                    return BadRequest(response.ErrorMessage);
                }
                return Ok(response);

            }
            else
            {
                return StatusCode(401, "Unauthorized");
            }
        }
        catch (Exception ex)
        {
            // Log the exception details here
            // Return a generic error message to the client, optionally with a custom error object
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }
}
