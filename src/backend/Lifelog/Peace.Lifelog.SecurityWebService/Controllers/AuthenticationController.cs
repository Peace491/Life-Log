﻿using System.Text.Json;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Peace.Lifelog.Security;
using Peace.Lifelog.Email;
using Peace.Lifelog.UserManagement;
using DomainModels;

namespace Peace.Lifelog.SecurityWebService;

[ApiController]
[Route("authentication")]
public class AuthenticationController : ControllerBase
{
    [HttpGet]
    [Route("getOTPEmail")]
    public async Task<IActionResult> GetOTPEmail(string userId) {
        var lifelogUserManagementService = new LifelogUserManagementService();

        var userHash = await lifelogUserManagementService.getUserHashFromUserId(userId);

        var emailService = new EmailService();

        // Act
        var emailResponse = await emailService.SendOTPEmail(userHash);
        
        return Ok(JsonSerializer.Serialize<string>(userHash));
    }

    [HttpPost]
    [Route("authenticateOTP")]
    public async Task<IActionResult> AuthenticateOTP([FromBody]AuthenticationRequest authenticationRequest){
        var lifelogAuthService = new LifelogAuthService();

        var appPrincipal = await lifelogAuthService.AuthenticateLifelogUser(authenticationRequest.UserHash, authenticationRequest.OTP)!;

        return Ok(JsonSerializer.Serialize<AppPrincipal>(appPrincipal));
    }

}
