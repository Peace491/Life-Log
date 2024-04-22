using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Peace.Lifelog.Security;
using Peace.Lifelog.Logging;
using back_end;

namespace Peace.Lifelog.LogWebService;



[ApiController]
[Route("log")]
public sealed class LogController : ControllerBase
{
    private readonly IJWTService jwtService;
    private readonly ILogging logging;
    public LogController(IJWTService jwtService, ILogging logging)
    {
        this.jwtService = jwtService;
        this.logging = logging;

    }

    [HttpPost]
    public async Task<IActionResult> Log([FromBody] Log log)
    {
        var processTokenResponseStatus = ProcessJwtToken();
        if (processTokenResponseStatus != 200)
        {
            return StatusCode(processTokenResponseStatus);
        }

        try
        {
            var response = await logging.CreateLog(log.Table, log.UserHash, log.Level, log.Category, log.Message);
            return Ok(response);
        }
        catch (Exception error)
        {
            return StatusCode(500, error.Message);
        }

    }

    [HttpPost]
    [Route("auth")]
    public async Task<IActionResult> LogAuth([FromBody] Log log)
    {
        try
        {
            var response = await logging.CreateLog(log.Table, log.UserHash, log.Level, log.Category, log.Message);
            return Ok(response);
        }
        catch (Exception error)
        {
            return StatusCode(500, error.Message);
        }
    }

    [HttpGet]
    [Route("getTopNVisitedPage")]
    public async Task<IActionResult> GetTopNVisitedPage(int numOfPage, int periodInMonth)
    {
        // var processTokenResponseStatus = ProcessJwtToken();
        // if (processTokenResponseStatus != 200)
        // {
        //     return StatusCode(processTokenResponseStatus);
        // }

        try
        {
            var response = await logging.ReadTopNMostVisitedPage("Logs", numOfPage, periodInMonth);

            return Ok(response);
        }
        catch (Exception error)
        {
            return StatusCode(500, error.Message);
        }
    }

    [HttpGet]
    [Route("getTopNLongestPageVisit")]
    public async Task<IActionResult> GetTopNLongestPageVisit(int numOfPage, int periodInMonth)
    {
        // var processTokenResponseStatus = ProcessJwtToken();
        // if (processTokenResponseStatus != 200)
        // {
        //     return StatusCode(processTokenResponseStatus);
        // }

        try
        {
            var response = await logging.ReadTopNLongestPageVisit("Logs", numOfPage, periodInMonth);

            return Ok(response);
        }
        catch (Exception error)
        {
            return StatusCode(500, error.Message);
        }

    }

    [HttpGet]
    [Route("getLoginLogsCount")]
    public async Task<IActionResult> GetLoginLogsCount(string type)
    {
        try
        {
            var response = await logging.ReadLoginLogsCount("Logs", type);
            return Ok(response);
        } catch (Exception error) 
        {
            return StatusCode(500, error.Message);
        }
    }

    [HttpGet]
    [Route("getRegLogsCount")]
    public async Task<IActionResult> GetRegLogsCount(string type)
    {
        try
        {
            var response = await logging.ReadRegLogsCount("Logs", type);
            return Ok(response);
        } catch (Exception error) 
        {
            return StatusCode(500, error.Message);
        }
    }


    private int ProcessJwtToken()
    {
        var jwtToken = JsonSerializer.Deserialize<Jwt>(Request.Headers["Token"]!);

        if (jwtToken == null)
        {
            return 401;
        }

        var statusCode = jwtService.ProcessToken(Request);

        if (statusCode != 200)
        {
            return statusCode;
        }

        return 200;
    }
}
