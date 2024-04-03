namespace Peace.Lifelog.REWebService;

using DomainModels;


using Peace.Lifelog.Logging;
using Microsoft.AspNetCore.Mvc; // Namespace needed for using Controllers
using System.Text.Json;
using Peace.Lifelog.RecEngineService;
using Peace.Lifelog.Security;
using back_end;

[ApiController]
[Route("re")]  // Defines the default parent URL path for all action methods to be the name of controller
public sealed class RecEngineController : ControllerBase
{
    private readonly IRecEngineService recEngineService;
    private readonly ILogging logger;
    private readonly IJWTService jwtService;

    public RecEngineController(IRecEngineService recEngineService, ILogging logger, IJWTService jwtService)
    {
        this.recEngineService = recEngineService;
        this.logger = logger;
        this.jwtService = jwtService;
    }

    [HttpPost]
    [Route("NumRecs")]
    public async Task<IActionResult> GetNumRecs([FromBody] PostNumRecsRequest payload)
    {
        var response = new Response();
        try
        {    
            
            // TODO: Token processing
            var statusCode = jwtService.ProcessToken(Request);

            if (statusCode == 401)
            {
                return StatusCode(401, "Unauthorized");
            }

            int numRecs = payload.NumRecs;

            Console.WriteLine("NumRecs: " + numRecs);
            
            response = await recEngineService.getNumRecs(payload.AppPrincipal, numRecs);

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
            Console.WriteLine(ex.Message);
            await logger.CreateLog("Logs", "RE", "ERROR", "REController", ex.Message);
            // Return a generic error message to the client, optionally with a custom error object
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }
    // Creating an LLI from RE utilizes the LLI API
}