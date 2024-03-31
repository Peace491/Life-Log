namespace Peace.Lifelog.REWebService;

using DomainModels;
using Peace.Lifelog.RE;
using Peace.Lifelog.LLIWebService;
using Microsoft.AspNetCore.Mvc; // Namespace needed for using Controllers
using System.Text.Json;

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
    [Route("NumRecs")]
    public async Task<IActionResult> GetNumRecs([FromBody] PostNumRecsRequest request)
    {
        try
        {
            // TODO: Token processing
            string userHash = "3\u002B/ZXoeqkYQ9JTJ6vcdAfjl667hgcMxQ\u002BSBLqmVDBuY=";
            int numRecs = request.NumRecs;
            
            var response = await reService.getNumRecs(userHash, numRecs);

            // Consider checking response for errors and handling them accordingly
            if (response.HasError)
            {
                return BadRequest(response.ErrorMessage);
            }
            if (response == null || response.Output == null)
            {
                return NotFound();
            }
            
            return Ok(JsonSerializer.Serialize<ICollection<Object>>(response.Output));
            // return Ok(response);
        }
        catch (Exception ex)
        {
            // Log the exception details here
            // Return a generic error message to the client, optionally with a custom error object
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }
}