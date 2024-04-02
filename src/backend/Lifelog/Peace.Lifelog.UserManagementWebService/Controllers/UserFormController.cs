using Microsoft.AspNetCore.Mvc;
using Peace.Lifelog.UserForm;
using DomainModels;
using System.Text.Json;

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
    [Route("UserForm")]
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
    [Route("UserForm")]
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
}
