using Microsoft.AspNetCore.Mvc;
using Peace.Lifelog.LifelogReminder;
using DomainModels;
using System.Text.Json;
using Peace.Lifelog.Security;
using back_end;

namespace Peace.Lifelog.UserManagementWebService.Controllers;



[ApiController]
[Route("lifelogReminder")]
public sealed class LifelogReminderController : ControllerBase
{
    private readonly ILifelogReminderService lifelogReminderService;
    private readonly IJWTService jwtService;
    public LifelogReminderController(ILifelogReminderService lifelogReminderService, IJWTService jwtService)
    {
        this.lifelogReminderService = lifelogReminderService;
        this.jwtService = jwtService;
    }

    [HttpPut]
    public async Task<IActionResult> UpdateLifelogReminder([FromBody] ReminderFormData reminderFormData)
    {
        var processTokenResponseStatus = ProcessJwtToken();
        if (processTokenResponseStatus != 200)
        {
            return StatusCode(processTokenResponseStatus);
        }
        var response = new Response();
        
        var appPrincipal = new AppPrincipal { UserId = userHash, Claims = new Dictionary<string, string>() { { "Role", role } } };

        try
        {
            response = await lifelogReminderService.UpdateReminderConfiguration(reminderFormData);
        }
        catch (Exception error)
        {
            return StatusCode(500, error);
        }

        if (response.HasError == true)
        {
            return StatusCode(400, response.ErrorMessage);
        }

        return StatusCode(200, JsonSerializer.Serialize<Response>(response));
    }

    [HttpGet]
    public async Task<IActionResult> SendReminderEmail(string userHash)
    {
        var processTokenResponseStatus = ProcessJwtToken();
        if (processTokenResponseStatus != 200)
        {
            return StatusCode(processTokenResponseStatus);
        }
        var response = new Response();

        var appPrincipal = new AppPrincipal { UserId = userHash, Claims = new Dictionary<string, string>() { { "Role", role } } };

        try
        {
            response = await lifelogReminderService.SendReminderEmail(appPrincipal);
        }
        catch (Exception error)
        {
            return StatusCode(500, error);
        }

        if (response.HasError == true)
        {
            return StatusCode(400, response.ErrorMessage);
        }

        return Ok(JsonSerializer.Serialize<Response>(response));

    }

    // Helper Functions

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
