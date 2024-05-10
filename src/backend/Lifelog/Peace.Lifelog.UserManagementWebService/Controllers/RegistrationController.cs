using Microsoft.AspNetCore.Mvc;
using Peace.Lifelog.UserManagementWebService.Models;
using DomainModels;

namespace Peace.Lifelog.UserManagementWebService.Controllers;

using System.Text.Json;
using DomainModels;
using Peace.Lifelog.RegistrationService;
using Peace.Lifelog.Email;
using Peace.Lifelog.UserManagement;
using Peace.Lifelog.Logging;

[ApiController]
[Route("registration")]
public class RegistrationController : ControllerBase
{
    private readonly ILifelogUserManagementService lifelogUserManagementService;
    private readonly ILogging logger;

    public RegistrationController(ILifelogUserManagementService lifelogUserManagementService, ILogging logger)
    {
        this.lifelogUserManagementService = lifelogUserManagementService;
        this.logger = logger;
    }
    [HttpPost]
    [Route("registerNormalUser")]
    public async Task<IActionResult> RegisterNormalUser([FromBody] RegisterNormalUserRequest registerNormalUserRequest)
    {
        var registrationService = new RegistrationService(lifelogUserManagementService, logger);
        var emailService = new EmailService();

        var checkInputResponse = await registrationService.CheckInputValidation(registerNormalUserRequest.UserId, registerNormalUserRequest.DOB, registerNormalUserRequest.ZipCode);

        var registerUserResponse = new Response();
        var userHash = "";

        if (checkInputResponse.HasError == false)
        {
            registerUserResponse = await registrationService.RegisterNormalUser(registerNormalUserRequest.UserId, registerNormalUserRequest.DOB, registerNormalUserRequest.ZipCode);
        }
        else {
            Console.WriteLine("CheckInputResponse has error");
            return BadRequest();
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
