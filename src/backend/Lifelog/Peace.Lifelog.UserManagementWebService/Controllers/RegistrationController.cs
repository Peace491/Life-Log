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
using Peace.Lifelog.DataAccess;
using Peace.Lifelog.Security;
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
        try
        {
            var registrationService = new RegistrationService(lifelogUserManagementService, logger);
            IReadDataOnlyDAO readDataOnlyDAO = new ReadDataOnlyDAO();
            IUpdateDataOnlyDAO updateDataOnlyDAO = new UpdateDataOnlyDAO();
            IEmailService emailService = new EmailService(readDataOnlyDAO, new OTPService(updateDataOnlyDAO), updateDataOnlyDAO);

            var checkInputResponse = await registrationService.CheckInputValidation(registerNormalUserRequest.UserId, registerNormalUserRequest.DOB, registerNormalUserRequest.ZipCode);

            var registerUserResponse = new Response();
            var userHash = "";
        var ip = HttpContext?.Connection?.RemoteIpAddress?.ToString();
            if (checkInputResponse.HasError == false && ip != null)
            {
                registerUserResponse = await registrationService.RegisterNormalUser(registerNormalUserRequest.UserId, registerNormalUserRequest.DOB, registerNormalUserRequest.ZipCode, ip);
            }
            else
            
            {
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

            if (registerUserResponse.HasError == true)
            {
                throw new Exception(registerUserResponse.ErrorMessage);
            }

            return Ok(JsonSerializer.Serialize<Response>(registerUserResponse));
        }
        catch (Exception error)
        {
            return StatusCode(500, error.Message);
        }


    }

    // [HttpPost]
    // [Route("postOTP")]
    // public IActionResult PostOTP([FromBody] PostOTPRequest postOTPRequest)
    // {

    //     return Ok();
    // }
}
