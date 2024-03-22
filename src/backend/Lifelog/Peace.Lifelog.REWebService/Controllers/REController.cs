namespace Peace.Lifelog.REWebService;

using Peace.Lifelog.RE;
using Microsoft.AspNetCore.Mvc; // Namespace needed for using Controllers

[ApiController]
[Route("re")]  // Defines the default parent URL path for all action methods to be the name of controller
public class REController : ControllerBase
{
    private IReService reService;

    public REController(IReService reService)
    {
        this.reService = reService;

    }


    [HttpPost] 
    [Route("getNumRecs")]
    public async Task<IActionResult> GetNumRecs([FromBody] PostNumRecsRequest request)
    {
        string userHash = "0Yg6cgh/M4+ImmL0GozWqhgcDCqTZEhzm9angvVAC30=";
        int numRecs = request.NumRecs;
        var response = await reService.getNumRecs(userHash, numRecs);

        return Ok(response);
    }

}