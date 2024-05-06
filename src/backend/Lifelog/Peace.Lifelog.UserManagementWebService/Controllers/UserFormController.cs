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
        var response = new Response();

        try
        {
            var processTokenResponseStatus = ProcessJwtToken();
            if (processTokenResponseStatus != 200)
            {
                return StatusCode(processTokenResponseStatus);
            }

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
    public async Task<IActionResult> UserForm()
    {

        var response = new Response();

        try
        {
            var processTokenResponseStatus = ProcessJwtToken();
            if (processTokenResponseStatus != 200)
            {
                return StatusCode(processTokenResponseStatus);
            }

            var jwtToken = JsonSerializer.Deserialize<Jwt>(Request.Headers["Token"]!);
            var userHash = jwtToken!.Payload.UserHash;
            var role = jwtToken.Payload.Claims!["Role"];

            var appPrincipal = new AppPrincipal { UserId = userHash!, Claims = new Dictionary<string, string>() { { "Role", role } } };

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
        var response = new Response();

        try
        {
            var processTokenResponseStatus = ProcessJwtToken();
            if (processTokenResponseStatus != 200)
            {
                return StatusCode(processTokenResponseStatus);
            }
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
    public async Task<IActionResult> GetUserFormCompletionStatus()
    {
        var isUserFormCompleted = false;

        try
        {
            var processTokenResponseStatus = ProcessJwtToken();
            if (processTokenResponseStatus != 200)
            {
                return StatusCode(processTokenResponseStatus);
            }

            var jwtToken = JsonSerializer.Deserialize<Jwt>(Request.Headers["Token"]!);
            var userHash = jwtToken!.Payload.UserHash;
            var role = jwtToken.Payload.Claims!["Role"];


            var principal = new AppPrincipal { UserId = userHash!, Claims = new Dictionary<string, string>() { { "Role", role } } };
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
