namespace Peace.Lifelog.REWebService;

using Peace.Lifelog.RE;
using Peace.Lifelog.LLIWebService;
using Microsoft.AspNetCore.Mvc; // Namespace needed for using Controllers

[ApiController]
[Route("re")]  // Defines the default parent URL path for all action methods to be the name of controller
public class REController : ControllerBase
{
    private IReService reService;

    public REController(IReService reService)
    {
        this.reService = reService;

    }

    [HttpPost]
    [Route("getNumRecs")]
    public async Task<IActionResult> GetNumRecs([FromBody] PostNumRecsRequest request)
    {
        try
        {
            // TODO: Token processing
            string userHash = "0Yg6cgh/M4+ImmL0GozWqhgcDCqTZEhzm9angvVAC30=";
            int numRecs = request.NumRecs;
            
            var response = await reService.getNumRecs(userHash, numRecs);

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
    
    // Update recommendation data mart for a user
    [HttpGet]
    [Route("updateRecommendationDataMartForUser")]
    public async Task<IActionResult> UpdateRecommendationDataMartForUser(string userHash)
    {
        try
        {
            var response = await reService.updateRecommendationDataMartForUser(userHash);

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
            var response = await reService.updateRecommendationDataMartForAllUsers();

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