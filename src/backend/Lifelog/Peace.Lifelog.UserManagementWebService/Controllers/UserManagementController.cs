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

    [HttpDelete]
    public async Task<IActionResult> DeleteUser(string userHash)
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
            Console.WriteLine("Viewing PII data for user: " + payload.userHash);
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
    [HttpPost("UpdateRoleToAdmin")]
    public async Task<IActionResult> UpdateUserRoleToAdmin([FromBody] UpdateRoleToAdmin payload)
    {
        try 
        {
            Console.WriteLine("Updating user role for user: " + payload.UserId);
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
    [HttpPost("UpdateRoleToNormal")]
    public async Task<IActionResult> UpdateUserRoleToNormal([FromBody] UpdateRoleToNormal payload)
    {
        try 
        {
            Console.WriteLine("Updating user role for user: " + payload.UserId);
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
    [HttpPost("GetAllNonRootUsers")]
    public async Task<IActionResult> GetAllNonRootUsers([FromBody] GetNonRootUsersRequest payload)
    {
        try 
        {
            Console.WriteLine("Getting all non-root users");
            var response = await lifelogUserManagementService.GetAllNonRootUsers(payload.Principal);

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
    [HttpPost("GetAllNormalUsers")]
    public async Task<IActionResult> GetAllNormalUsers([FromBody] GetNormalUsersRequest payload)
    {
        try 
        {
            Console.WriteLine("Getting all normal users");
            var response = await lifelogUserManagementService.GetAllNormalUsers(payload.Principal);

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
}

