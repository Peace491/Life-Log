using Microsoft.AspNetCore.Mvc;
using Peace.Lifelog.UserManagementWebService.Models;
using DomainModels;

namespace Peace.Lifelog.UserManagementWebService.Controllers;

using System.Text.Json;
using DomainModels;
using Peace.Lifelog.RegistrationService;
using Peace.Lifelog.Security;
using Peace.Lifelog.Email;

[ApiController]
[Route("registration")]
public class RegistrationController : ControllerBase
{
    [HttpPost]
    [Route("registerNormalUser")]
    public async Task<IActionResult> RegisterNormalUser([FromBody] RegisterNormalUserRequest registerNormalUserRequest)
    {
        var registrationService = new RegistrationService();
        var emailService = new EmailService();

        var checkInputResponse = await registrationService.CheckInputValidation(registerNormalUserRequest.UserId, registerNormalUserRequest.DOB, registerNormalUserRequest.ZipCode);

        var registerUserResponse = new Response();
        var userHash = "";

        if (checkInputResponse.HasError == false)
        {
            registerUserResponse = await registrationService.RegisterNormalUser(registerNormalUserRequest.UserId, registerNormalUserRequest.DOB, registerNormalUserRequest.ZipCode);
        }

        if (registerUserResponse.HasError == false)
        {
            if (registerUserResponse.Output is not null)
            {
                foreach (string output in registerUserResponse.Output)
                {
                    userHash = output;
                }
            }
            var emailResponse = emailService.SendOTPEmail(userHash);

        }

        return Ok(JsonSerializer.Serialize<Response>(registerUserResponse));

    }

    [HttpPost]
    [Route("postOTP")]
    public IActionResult PostOTP([FromBody] PostOTPRequest postOTPRequest)
    {

        return Ok();
    }
}
