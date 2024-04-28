using Microsoft.AspNetCore.Mvc;

namespace Peace.Lifelog.UserManagementWebService.Controllers;


using Peace.Lifelog.Security;
using Peace.Lifelog.UserManagement;

[ApiController]
[Route("userManagement")]
public class UserManagementController : ControllerBase
{
    [HttpDelete]
    public async Task<IActionResult> DeleteUser(string userHash)
    {
        try {
            var lifelogUserManagementService = new LifelogUserManagementService();
            var userId = await lifelogUserManagementService.getUserIdFromUserHash(userHash);

            var lifelogAccountRequest = new LifelogAccountRequest(){UserId = ("UserId", userId)};
            var lifelogProfileRequest = new LifelogProfileRequest(){UserId = ("UserHash", userHash)};
            var response = await lifelogUserManagementService.DeleteLifelogUser(lifelogAccountRequest, lifelogProfileRequest);

            return Ok(response);
        } catch(Exception error) {
            return StatusCode(500, error.Message);
        }

    }

}
