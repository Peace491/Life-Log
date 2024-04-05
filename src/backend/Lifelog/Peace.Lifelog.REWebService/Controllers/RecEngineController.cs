namespace Peace.Lifelog.REWebService;

using Microsoft.AspNetCore.Mvc;

using System;
using System.Text.Json;
using System.Threading.Tasks;

using Peace.Lifelog.Logging;
using Peace.Lifelog.RecEngineService;
using Peace.Lifelog.Security;

[ApiController]
[Route("re")]
public sealed class RecEngineController : ControllerBase
{
    private readonly IRecEngineService _recEngineService;
    private readonly ILogging _logger;
    private readonly IJWTService _jwtService;

    public RecEngineController(IRecEngineService recEngineService, ILogging logger, IJWTService jwtService)
    {
        _recEngineService = recEngineService;
        _logger = logger;
        _jwtService = jwtService;
    }

    [HttpPost("NumRecs")]
    public async Task<IActionResult> GetNumRecs([FromBody] PostNumRecsRequest payload)
    {
        try
        {
            var statusCode = _jwtService.ProcessToken(Request);

            if (payload.AppPrincipal == null)
            {
                return BadRequest("AppPrincipal is null.");
            }

            var response = await _recEngineService.RecNumLLI(payload.AppPrincipal, payload.NumRecs);

            if (response == null)
            {
                return NotFound("Couldn't retrieve recommendations.");
            }

            if (response.HasError)
            {
                return BadRequest(response.ErrorMessage);
            }

            if (response.Output == null)
            {
                return NotFound("Couldn't retrieve recommendations.");
            }

            return Ok(JsonSerializer.Serialize<ICollection<Object>>(response.Output));
        }
        catch (Exception ex)
        {
            _ = await _logger.CreateLog("Logs", "RecEngineController", "ERROR", "System", ex.Message);
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }
}
