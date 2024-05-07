namespace Peace.Lifelog.LLIWebService;

using Peace.Lifelog.DataAccess;
using Peace.Lifelog.LLI;
using Peace.Lifelog.Logging;
using Peace.Lifelog.Security;
using Microsoft.AspNetCore.Mvc; // Namespace needed for using Controllers
using System.Text.Json;
using back_end;

[ApiController]
[Route("lli")]  // Defines the default parent URL path for all action methods to be the name of controller
public class LLIController : ControllerBase
{
    private CreateDataOnlyDAO createDataOnlyDAO;
    private ReadDataOnlyDAO readDataOnlyDAO;
    private UpdateDataOnlyDAO updateDataOnlyDAO;
    private DeleteDataOnlyDAO deleteDataOnlyDAO;
    private LogTarget logTarget;
    private Logging logging;
    private LLIService lliService;
    private JWTService jwtService;
    public LLIController()
    {
        this.createDataOnlyDAO = new CreateDataOnlyDAO();
        this.readDataOnlyDAO = new ReadDataOnlyDAO();
        this.updateDataOnlyDAO = new UpdateDataOnlyDAO();
        this.deleteDataOnlyDAO = new DeleteDataOnlyDAO();
        this.logTarget = new LogTarget(this.createDataOnlyDAO, this.readDataOnlyDAO);
        this.logging = new Logging(this.logTarget);
        this.lliService = new LLIService(this.createDataOnlyDAO, this.readDataOnlyDAO, this.updateDataOnlyDAO, this.deleteDataOnlyDAO, this.logging);
        this.jwtService = new JWTService();

    }
    [HttpPost]
    [Route("postLLI")]
    public async Task<IActionResult> PostLLI([FromBody] PostLLIRequest createLLIRequest)
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

        var lli = new LLI();
        lli.Title = createLLIRequest.Title;
        lli.Category1 = createLLIRequest.Category1;
        lli.Category2 = createLLIRequest.Category2;
        lli.Category3 = createLLIRequest.Category3;
        lli.Description = createLLIRequest.Description;
        lli.Status = createLLIRequest.Status;
        lli.Visibility = createLLIRequest.Visibility;
        lli.Deadline = createLLIRequest.Deadline;
        lli.Cost = createLLIRequest.Cost;
        lli.Recurrence.Status = createLLIRequest.RecurrenceStatus;
        lli.Recurrence.Frequency = createLLIRequest.RecurrenceFrequency;

        var response = await this.lliService.CreateLLI(userHash, lli);

        if (response.HasError == false)
        {
            return Ok(response);
        }
        else if (response.ErrorMessage!.Contains("invalid") || response.ErrorMessage!.Contains("completed within the last year"))
        {
            Console.WriteLine(response.ErrorMessage!);
            return StatusCode(400, response.ErrorMessage);
        }
        else
        {
            return StatusCode(500, response.ErrorMessage);
        }

    }

    [HttpGet]
    [Route("getAllLLI")]
    public async Task<IActionResult> GetAllLLI()
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

        var response = await this.lliService.GetAllLLIFromUser(userHash);

        if (response.HasError == false)
        {
            return Ok(response);
        }
        else if (response.ErrorMessage!.Contains("UserHash can not be empty"))
        {
            return StatusCode(400, response.ErrorMessage);
        }
        else
        {
            return StatusCode(500);
        }
    }

    [HttpGet]
    [Route("mostCommonLLICategory")]
    public async Task<IActionResult> GetMostCommonLLICategory(int period)
    {
        try
        {
            var processTokenResponseStatus = ProcessJwtToken();
            if (processTokenResponseStatus != 200)
            {
                return StatusCode(processTokenResponseStatus);
            }

            if (!IsUserAuthenticatedForUADApiPoints()) return StatusCode(401);

            var response = await lliService.GetMostCommonLLICategory(period);
            return Ok(response);
        }
        catch (Exception error)
        {
            return StatusCode(500, error);
        }
    }

    [HttpGet]
    [Route("mostExpensiveLLI")]
    public async Task<IActionResult> GetMostExpensiveLLI(int period)
    {
        try
        {
            var processTokenResponseStatus = ProcessJwtToken();
            if (processTokenResponseStatus != 200)
            {
                return StatusCode(processTokenResponseStatus);
            }

            if (!IsUserAuthenticatedForUADApiPoints()) return StatusCode(401);

            var response = await lliService.GetMostExpensiveLLI(period);
            return Ok(response);
        }
        catch (Exception error)
        {
            return StatusCode(500, error);
        }
    }

    [HttpPut]
    [Route("putLLI")]
    public async Task<IActionResult> PutLLI([FromBody] PutLLIRequest updateLLIRequest)
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

        var lli = new LLI();
        lli.LLIID = updateLLIRequest.LLIID;
        lli.Title = updateLLIRequest.Title;
        lli.Category1 = updateLLIRequest.Category1;
        lli.Category2 = updateLLIRequest.Category2;
        lli.Category3 = updateLLIRequest.Category3;
        lli.Description = updateLLIRequest.Description;
        lli.Status = updateLLIRequest.Status;
        lli.Visibility = updateLLIRequest.Visibility;
        lli.Deadline = updateLLIRequest.Deadline;
        lli.Cost = updateLLIRequest.Cost;
        lli.Recurrence.Status = updateLLIRequest.RecurrenceStatus;
        lli.Recurrence.Frequency = updateLLIRequest.RecurrenceFrequency;

        var response = await this.lliService.UpdateLLI(userHash, lli);

        if (response.HasError == false)
        {
            return Ok(response);
        }
        else if (response.ErrorMessage!.Contains("invalid") || response.ErrorMessage!.Contains("completed within the last year"))
        {
            return StatusCode(400, response.ErrorMessage);
        }
        else
        {
            return StatusCode(500);
        }
    }

    [HttpDelete]
    [Route("deleteLLI")]
    public async Task<IActionResult> DeleteLLI(string lliId)
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

        var lli = new LLI();
        lli.LLIID = lliId;

        var response = await this.lliService.DeleteLLI(userHash, lli);

        if (response.HasError == false)
        {
            return Ok(response);
        }
        else if (response.ErrorMessage!.Contains("invalid"))
        {
            return StatusCode(400, response.ErrorMessage);
        }
        else
        {
            return StatusCode(500);
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

    private bool IsUserAuthenticatedForUADApiPoints() {
        var jwtToken = JsonSerializer.Deserialize<Jwt>(Request.Headers["Token"]!);

        if (jwtToken == null) return false;
        if (jwtToken.Payload.Claims == null) return false;

        var userRole = jwtToken.Payload.Claims["Role"]!;

        if (userRole == "Admin" || userRole == "Root") {
            return true;
        }

        return false;
    }


}


public class RequestHeader()
{
    public string ContentType { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
}