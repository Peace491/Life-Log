using Microsoft.AspNetCore.Mvc;

namespace Peace.Lifelog.UserManagementWebService.Controllers;

using Org.BouncyCastle.Security;
using Peace.Lifelog.Security;
using Peace.Lifelog.UserManagement;
using System.Text.Json;
using back_end;
using Peace.Lifelog.Logging;

[ApiController]
[Route("userManagement")]
public sealed class UserManagementController : ControllerBase
{
    private readonly IJWTService jwtService;
    private readonly ILifelogUserManagementService lifelogUserManagementService;
    public UserManagementController(IJWTService jwtService, ILifelogUserManagementService lifelogUserManagementService)
    {
        this.jwtService = jwtService;
        this.lifelogUserManagementService = lifelogUserManagementService;
    }

    [HttpPost]
    [Route("recoverUser")]
    public async Task<IActionResult> CreateUserRecoveryRequest([FromBody] string userId)
    {
        try
        {
            var lifelogAccountRequest = new LifelogAccountRequest() { UserId = ("UserId", userId) };
            var response = await lifelogUserManagementService.CreateRecoveryAccountRequest(lifelogAccountRequest);

            if (response.HasError)
            {
                return StatusCode(500, response.ErrorMessage);
            }

            return Ok("Account Recovery Request Successfully Created");
        }
        catch (Exception error)
        {
            return StatusCode(500, error.Message);
        }
    }

    [HttpGet]
    [Route("recoverUser")]
    public async Task<IActionResult> RecoverUser()
    {
        try
        {
            var processTokenResponseStatus = jwtService.ProcessToken(Request);
            if (processTokenResponseStatus != 200)
            {
                return StatusCode(processTokenResponseStatus);
            }

            var jwtToken = JsonSerializer.Deserialize<Jwt>(Request.Headers["Token"]!);
            var userHash = jwtToken!.Payload.UserHash;
            var claims = jwtToken.Payload.Claims;
            // var userHash = "System";
            // var claims = new Dictionary<string, string>() { { "Role", "Admin" } };

            var principal = new AppPrincipal(){UserId = userHash!, Claims = claims};

            var response = await lifelogUserManagementService.GetRecoveryAccountRequests(principal);

            if (response.HasError)
            {
                if (response.ErrorMessage == "Unauthorized Request")
                {
                    return StatusCode(401);
                }
                return StatusCode(500);
            }

            return Ok(response);
        }
        catch (Exception error)
        {
            return StatusCode(500, error.Message);
        }

    }

    [HttpPut]
    [Route("recoverUser")]
    public async Task<IActionResult> RecoverUser([FromBody] RecoverAccountRequest recoverAccountRequest)
    {
        try
        {
            var processTokenResponseStatus = jwtService.ProcessToken(Request);
            if (processTokenResponseStatus != 200)
            {
                return StatusCode(processTokenResponseStatus);
            }

            var lifelogAccountRequest = new LifelogAccountRequest() { UserId = ("UserId", recoverAccountRequest.UserId) };
            var response = await lifelogUserManagementService.RecoverLifelogUser(recoverAccountRequest.Principal, lifelogAccountRequest);

            if (response.HasError)
            {
                if (response.ErrorMessage == "Unauthorized Request")
                {
                    return StatusCode(401);
                }
                return StatusCode(500, response.ErrorMessage);
            }

            return Ok(response);
        }
        catch (Exception error)
        {
            return StatusCode(500, error.Message);
        }

    }

    [HttpPost]
    public async Task<IActionResult> DeleteUser([FromBody] string userHash)
    {
        try
        {
            var processTokenResponseStatus = jwtService.ProcessToken(Request);
            if (processTokenResponseStatus != 200)
            {
                return StatusCode(processTokenResponseStatus);
            }

            var userId = await lifelogUserManagementService.GetUserIdFromUserHash(userHash);

            var lifelogAccountRequest = new LifelogAccountRequest() { UserId = ("UserId", userId) };
            var lifelogProfileRequest = new LifelogProfileRequest() { UserId = ("UserHash", userHash) };
            var response = await lifelogUserManagementService.DeleteLifelogUser(lifelogAccountRequest, lifelogProfileRequest);

            return Ok(response);
        }
        catch (Exception error)
        {
            return StatusCode(500, error.Message);
        }
    }
    [HttpPost("ViewPII")]
    public async Task<IActionResult> ViewUserPIIData([FromBody] ViewPIIRequest payload)
    {
        try 
        {
            var response = await lifelogUserManagementService.ViewPersonalIdentifiableInformation(payload.userHash);

            if(response.HasError)
            {
                throw new Exception("Error deleting PII data");
            }

            return Ok(response);
        } 
        catch(Exception error) 
        {
            return StatusCode(500, error.Message);
        }
    }

    [HttpPost("updateRoleToAdmin")]
    public async Task<IActionResult> UpdateUserRoleToAdmin([FromBody] UpdateRoleToAdmin payload)
    {
        try 
        {
            var processTokenResponseStatus = ProcessJwtToken();
            if (processTokenResponseStatus != 200)
            {
                return StatusCode(processTokenResponseStatus);
            }
            // first principal is principal of user making the request, second uid is uid of user to update
            var response = await lifelogUserManagementService.UpdateRoleToAdmin(payload.Principal, payload.UserId);

            if(response.HasError)
            {
                throw new Exception("Error updating user role");
            }

            return Ok(response);
        } 
        catch(Exception error) 
        {
            return StatusCode(500, error.Message);
        }
    }

    [HttpPost("updateRoleToNormal")]
    public async Task<IActionResult> UpdateUserRoleToNormal([FromBody] UpdateRoleToNormal payload)
    {
        try 
        {
            var processTokenResponseStatus = ProcessJwtToken();
            if (processTokenResponseStatus != 200)
            {
                return StatusCode(processTokenResponseStatus);
            }
            // first principal is principal of user making the request, second uid is uid of user to update
            var response = await lifelogUserManagementService.UpdateRoleToNormal(payload.Principal, payload.UserId);

            if(response.HasError)
            {
                throw new Exception("Error updating user role");
            }

            return Ok(response);
        } 
        catch(Exception error) 
        {
            return StatusCode(500, error.Message);
        }
    }

    [HttpPost("updateStatus")]
    public async Task<IActionResult> UpdateUserStatusToEnabled([FromBody] UpdateStatus payload)
    {
        try 
        {
            var processTokenResponseStatus = ProcessJwtToken();
            if (processTokenResponseStatus != 200)
            {
                return StatusCode(processTokenResponseStatus);
            }
            // first principal is principal of user making the request, second uid is uid of user to update
            var response = await lifelogUserManagementService.UpdateStatus(payload.Principal, payload.UserId, payload.Status);

            if(response.HasError)
            {
                throw new Exception("Error updating user status");
            }

            return Ok(response);
        } 
        catch(Exception error) 
        {
            return StatusCode(500, error.Message);
        }
    }

    [HttpGet("getAllNonRootUsers")]
    public async Task<IActionResult> GetAllNonRootUsers()
    {
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

            var response = await lifelogUserManagementService.GetAllNonRootUsers(principal);

            if(response.HasError)
            {
                throw new Exception("Error getting all non-root users");
            }

            return Ok(response);
        } 
        catch(Exception error) 
        {
            return StatusCode(500, error.Message);
        }
    }

    [HttpGet("getAllNormalUsers")]
    public async Task<IActionResult> GetAllNormalUsers()
    {
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

            var response = await lifelogUserManagementService.GetAllNormalUsers(principal);

            if(response.HasError)
            {
                throw new Exception("Error getting all normal users");
            }

            return Ok(response);
        } 
        catch(Exception error) 
        {
            return StatusCode(500, error.Message);
        }
    }

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



