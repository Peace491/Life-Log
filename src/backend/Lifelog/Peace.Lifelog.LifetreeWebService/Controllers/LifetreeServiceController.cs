namespace Peace.Lifelog.LifetreeWebService;

using back_end;
using Microsoft.AspNetCore.Mvc;
using Peace.Lifelog.LifetreeService;
using System.Text.Json;


[ApiController]
[Route("lifetreeService")]
public class LifetreeServiceController : ControllerBase
{
    private LifetreeService lifetreeService;
    public LifetreeServiceController()
    {
        this.lifetreeService = new LifetreeService();
    }



    [HttpGet]
    [Route("getCompletedLLI")]
    public async Task<IActionResult> GetCompletedLLI()
    {

        if (Request.Headers == null)
        {
            return StatusCode(401);
        }

        var jwtToken = JsonSerializer.Deserialize<Jwt>(Request.Headers["Token"]!);

        if (jwtToken == null)
        {
            return StatusCode(401);
        }

        var userHash = jwtToken.Payload.UserHash;

        if (userHash == null)
        {
            return StatusCode(401);
        }




        var response = await lifetreeService.getAllCompletedLLI(userHash);

        if (response.HasError == false)
        {
            return Ok(response);
        }


        return StatusCode(500);

    }

}
