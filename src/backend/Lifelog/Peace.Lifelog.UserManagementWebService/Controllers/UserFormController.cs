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
    public UserFormController(IUserFormService userFormService) 
    {
        this.userFormService = userFormService;
    }

    [HttpPost]
    public async Task<IActionResult> UserForm([FromBody] CreateUserFormRequest createUserFormRequest)
    {
        var response = new Response();

        try {
            response = await userFormService.CreateUserForm(createUserFormRequest);
        }
        catch (Exception error){
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

        try {
            response = await userFormService.UpdateUserForm(updateUserFormRequest);
        }
        catch (Exception error){
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
    public async Task<IActionResult> UserForm()
    {
        if (Request.Headers == null) {
            return StatusCode(401);
        }

        var jwtToken = JsonSerializer.Deserialize<Jwt>(Request.Headers["Token"]!);

        if (jwtToken == null) {
            return StatusCode(401);
        }

        var userHash = jwtToken.Payload.UserHash;

        if (userHash == null) {
            return StatusCode(401);
        }

        var isUserFormCompleted = false;

        try {
            isUserFormCompleted = await userFormService.IsUserFormCompleted(userHash);
        } catch {
            return StatusCode(500);
        }

        return Ok(isUserFormCompleted);
    }
}
