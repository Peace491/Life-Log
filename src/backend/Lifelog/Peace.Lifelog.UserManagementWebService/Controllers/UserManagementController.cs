using Microsoft.AspNetCore.Mvc;

namespace Peace.Lifelog.UserManagementWebService.Controllers;


using Peace.Lifelog.Security;
using Peace.Lifelog.UserManagement;

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

}
