using System.Text.Json;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Peace.Lifelog.Security;
using Peace.Lifelog.Email;
using Peace.Lifelog.UserManagement;
using DomainModels;
using Peace.Lifelog.Logging;
using back_end;

namespace Peace.Lifelog.SecurityWebService;

[ApiController]
[Route("authentication")]
public class AuthenticationController : ControllerBase
{
    private readonly LifelogUserManagementService lifelogUserManagementService;
    private readonly ILogging _logger;
    private readonly IEmailService emailService;

    public AuthenticationController(LifelogUserManagementService lifelogUserManagementService, ILogging logger, IEmailService emailService)
    {
        this.lifelogUserManagementService = lifelogUserManagementService;
        _logger = logger;
        this.emailService = emailService;
    }
    [HttpGet]
    [Route("getOTPEmail")]
    public async Task<IActionResult> GetOTPEmail(string userId)
    {
        try
        {
            
            // var lifelogUserManagementService = new LifelogUserManagementService();

            var userHash = await lifelogUserManagementService.getUserHashFromUserId(userId);

            // Act
            var emailResponse = await emailService.SendOTPEmail(userHash);

            return Ok(JsonSerializer.Serialize<string>(userHash));
        }
        catch
        {
            return StatusCode(500);
        }

    }

    [HttpPost]
    [Route("authenticateOTP")]
    public async Task<IActionResult> AuthenticateOTP([FromBody] AuthenticationRequest authenticationRequest)
    {
        var lifelogAuthService = new LifelogAuthService();
        try
        {
            var appPrincipal = await lifelogAuthService.AuthenticateLifelogUser(authenticationRequest.UserHash, authenticationRequest.OTP)!;

            var jwtService = new JWTService();

            var jwt = jwtService.createJWT(Request, appPrincipal, authenticationRequest.UserHash);

            return Ok(JsonSerializer.Serialize<Jwt>(jwt));
        }
        catch
        {
            return StatusCode(500);
        }


    }

}
