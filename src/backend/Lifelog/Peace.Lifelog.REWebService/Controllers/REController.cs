namespace Peace.Lifelog.LLIWebService;

using Peace.Lifelog.RE;
using Microsoft.AspNetCore.Mvc; // Namespace needed for using Controllers

[ApiController]
[Route("re")]  // Defines the default parent URL path for all action methods to be the name of controller
public class LLIController : ControllerBase
{
    [HttpGet] 
    [Route("getNumRecs")]
    public async Task<IActionResult> GetAllLLI(string userHash)
    {
        var rEManager = new REManager();
        var response = await rEManager.getRecs(userHash);

        return Ok(response);
    }
}