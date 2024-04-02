namespace Peace.Lifelog.REWebService;

using DomainModels;
using Peace.Lifelog.RE;
using Peace.Lifelog.LLIWebService;
using Peace.Lifelog.Logging;
using Microsoft.AspNetCore.Mvc; // Namespace needed for using Controllers
using System.Text.Json;
using Peace.Lifelog.RecEngineService;

[ApiController]
[Route("re")]  // Defines the default parent URL path for all action methods to be the name of controller
public class REController : ControllerBase
{
    private IRecEngineService reService;
    private readonly ILogging logger;

    public REController(IRecEngineService reService, ILogging logger)
    {
        this.reService = reService;
        this.logger = logger;
    }

    [HttpPost]
    [Route("NumRecs")]
    public async Task<IActionResult> GetNumRecs([FromBody] PostNumRecsRequest request)
    {
        var response = new Response();
        try
        {
            // TODO: Token processing
            string userHash = "3\u002B/ZXoeqkYQ9JTJ6vcdAfjl667hgcMxQ\u002BSBLqmVDBuY=";
            int numRecs = request.NumRecs;
            
            response = await reService.getNumRecs(userHash, numRecs);

            // Consider checking response for errors and handling them accordingly
            if (response.HasError)
            {
                // Create a clean, unrevealing error message to return to the client
                return BadRequest(response.ErrorMessage);
            }
            if (response == null || response.Output == null)
            {
                // Return a 404 Not Found status code if the response is null or the output is null
                return NotFound();
            }

            // On sucess, only return the output
            return Ok(JsonSerializer.Serialize<ICollection<Object>>(response.Output));
        }
        catch (Exception ex)
        {
            // Log the exception details here
            await logger.CreateLog("Logs", "RE", "ERROR", "REController", ex.Message);
            // Return a generic error message to the client, optionally with a custom error object
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }
    // Creating an LLI from RE utilizes the LLI API
}