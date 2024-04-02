using Microsoft.AspNetCore.Mvc;
using Peace.Lifelog.RecSummaryService;

namespace Peace.Lifelog.REDatamartWebService.Controllers;

[ApiController]
[Route("summary")]
public class RecSummaryController : ControllerBase
{
    private IRecSummaryService recSummaryService;

    public RecSummaryController(IRecSummaryService recSummaryService)
    {
        this.recSummaryService = recSummaryService;
    }
    
    [HttpGet]
    [Route("UserRecSummary")]
    public async Task<IActionResult> UpdateRecommendationDataMartForUser(string userHash)
    {
        try
        {
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
            
            var response = await recSummaryService.updateAllUserRecSummary();

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
