using Microsoft.AspNetCore.Mvc;
using Peace.Lifelog.REDatamartService;

namespace Peace.Lifelog.REDatamartWebService.Controllers;

[ApiController]
[Route("summary")]
public class SummaryController : ControllerBase
{
    private IREDatamart reDMService;

    public SummaryController(IREDatamart reDMService)
    {
        this.reDMService = reDMService;

    }
    [HttpGet]
    [Route("updateRecommendationDataMartForUser")]
    public async Task<IActionResult> UpdateRecommendationDataMartForUser(string userHash)
    {
        try
        {
            var response = await reDMService.updateRecommendationDataMartForUser(userHash);

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
    [Route("updateRecommendationDataMartForAllUsers")]
    public async Task<IActionResult> UpdateRecommendationDataMartForAllUsers()
    {
        try
        {
            var response = await reDMService.updateRecommendationDataMartForAllUsers();

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
}
