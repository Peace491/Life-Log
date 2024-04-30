using Microsoft.AspNetCore.Mvc;
using Peace.Lifelog.UserForm;
using DomainModels;
using System.Text.Json;
using Peace.Lifelog.Security;
using back_end;

namespace Peace.Lifelog.UserManagementWebService.Controllers;



[ApiController]
[Route("userForm")]
public sealed class UserFormController : ControllerBase
{
    private readonly IUserFormService userFormService;
    private readonly IJWTService jwtService;
    public UserFormController(IUserFormService userFormService, IJWTService jwtService)
    {
        this.userFormService = userFormService;
        this.jwtService = jwtService;
    }

    [HttpPost]
    public async Task<IActionResult> UserForm([FromBody] CreateUserFormRequest createUserFormRequest)
    {
        var processTokenResponseStatus = ProcessJwtToken();
        if (processTokenResponseStatus != 200)
        {
            return StatusCode(processTokenResponseStatus);
        }

        var response = new Response();

        try
        {
            response = await userFormService.CreateUserForm(createUserFormRequest);
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

    [HttpGet]
    public async Task<IActionResult> UserForm(string userHash, string role)
    {
        var processTokenResponseStatus = ProcessJwtToken();
        if (processTokenResponseStatus != 200)
        {
            return StatusCode(processTokenResponseStatus);
        }
        var response = new Response();

        Console.WriteLine(userHash);

        var appPrincipal = new AppPrincipal { UserId = userHash, Claims = new Dictionary<string, string>() { { "Role", role } } };

        try
        {
            response = await userFormService.GetUserFormRanking(appPrincipal);
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

    [HttpPut]
    public async Task<IActionResult> UserForm([FromBody] UpdateUserFormRequest updateUserFormRequest)
    {
        var processTokenResponseStatus = ProcessJwtToken();
        if (processTokenResponseStatus != 200)
        {
            return StatusCode(processTokenResponseStatus);
        }
        var response = new Response();

        try
        {
            response = await userFormService.UpdateUserForm(updateUserFormRequest);
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

    [HttpGet]
    [Route("isUserFormCompleted")]
    public async Task<IActionResult> GetUserFormCompletionStatus(string userHash, string role)
    {
        var processTokenResponseStatus = ProcessJwtToken();
        if (processTokenResponseStatus != 200)
        {
            return StatusCode(processTokenResponseStatus);
        }

        if (userHash == null)
        {
            return StatusCode(401);
        }

        var isUserFormCompleted = false;

        try
        {
            var principal = new AppPrincipal { UserId = userHash, Claims = new Dictionary<string, string>() { { "Role", role } } };
            isUserFormCompleted = await userFormService.IsUserFormCompleted(principal);
        }
        catch
        {
            return StatusCode(500);
        }

        return Ok(isUserFormCompleted);
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
