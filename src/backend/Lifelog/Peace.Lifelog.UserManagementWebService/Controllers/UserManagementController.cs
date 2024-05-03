using Microsoft.AspNetCore.Mvc;

namespace Peace.Lifelog.UserManagementWebService.Controllers;

using Org.BouncyCastle.Security;
using Peace.Lifelog.Security;
using Peace.Lifelog.UserManagement;
using Peace.Lifelog.Logging;

[ApiController]
[Route("userManagement")]
public class UserManagementController : ControllerBase
{
    private readonly LifelogUserManagementService lifelogUserManagementService;
    private readonly ILogging _logger;
    public UserManagementController(LifelogUserManagementService lifelogUserManagementService, ILogging logger)
    {
        this.lifelogUserManagementService = lifelogUserManagementService;
        _logger = logger;
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteUser(string userHash)
    {
        try 
        {
            // var lifelogUserManagementService = new LifelogUserManagementService(userManagementRepo);
            var userId = await lifelogUserManagementService.getUserIdFromUserHash(userHash);

            var lifelogAccountRequest = new LifelogAccountRequest(){UserId = ("UserId", userId)};
            var lifelogProfileRequest = new LifelogProfileRequest(){UserId = ("UserHash", userHash)};
            var response = await lifelogUserManagementService.DeleteLifelogUser(lifelogAccountRequest, lifelogProfileRequest);

            return Ok(response);
        } 
        catch(Exception error) 
        {
            return StatusCode(500, error.Message);
        }
    }
    [HttpDelete("DeletePII")]
    public async Task<IActionResult> DeleteUserPIIData(string userHash)
    {
        try 
        {
            var response = await lifelogUserManagementService.DeletePersonalIdentifiableInformation(userHash);

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
    [HttpGet("ViewPII")]
    public async Task<IActionResult> ViewUserPIIData(string userHash)
    {
        try 
        {
            var response = await lifelogUserManagementService.ViewPersonalIdentifiableInformation(userHash);

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
}
