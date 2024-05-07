namespace Peace.Lifelog.LifetreeWebService;

using back_end;
using Microsoft.AspNetCore.Mvc;
using Peace.Lifelog.LifetreeService;

using Peace.Lifelog.PersonalNote;
using Peace.Lifelog.Security;
using System.Text.Json;



[ApiController]
[Route("lifetreeService")]
public class LifetreeServiceController : ControllerBase
{
    private LifetreeService lifetreeService;
    private JWTService jwtService;

    public LifetreeServiceController()
    {
        this.lifetreeService = new LifetreeService();
        this.jwtService = new JWTService();
    }

    [HttpGet]
    [Route("getCompletedLLI")]
    public async Task<IActionResult> GetCompletedLLI()
    {

        var processTokenResponseStatus = ProcessJwtToken();
        if (processTokenResponseStatus != 200)
        {
            return StatusCode(processTokenResponseStatus);
        }

        var jwtToken = JsonSerializer.Deserialize<Jwt>(Request.Headers["Token"]!);
        var userHash = jwtToken!.Payload.UserHash;

        if (userHash == null)
        {
            return StatusCode(401);
        }

        /*string userHash = "bvjkHx+lmD8D5Dr/0//1P5SXmcu5Mb6oSBh4nKdUKYI=";*/


        var response = await lifetreeService.getAllCompletedLLI(userHash);


        if (response.HasError == false)
        {
            return Ok(response);
        }


        return StatusCode(500);

    }

    [HttpGet]
    [Route("getPN")]
    public async Task<IActionResult> GetPN(string notedate)
    {

        var processTokenResponseStatus = ProcessJwtToken();
        if (processTokenResponseStatus != 200)
        {
            return StatusCode(processTokenResponseStatus);
        }

        var jwtToken = JsonSerializer.Deserialize<Jwt>(Request.Headers["Token"]!);
        var userHash = jwtToken!.Payload.UserHash;

        if (userHash == null)
        {
            return StatusCode(401);
        }


        var personalnote = new PN();
        personalnote.NoteDate = notedate;

        var response = await lifetreeService.GetOnePNWithLifetree(userHash, personalnote);


        if (response.HasError == false)
        {
            return Ok(response);
        }


        return StatusCode(500);
    }


    [HttpPost]
    [Route("postPN")]
    public async Task<IActionResult> PostPN([FromBody] PostPersonalNoteRequest createPersonalNoteRequest)
    {

        var processTokenResponseStatus = ProcessJwtToken();
        if (processTokenResponseStatus != 200)
        {
            return StatusCode(processTokenResponseStatus);
        }

        var jwtToken = JsonSerializer.Deserialize<Jwt>(Request.Headers["Token"]!);
        var userHash = jwtToken!.Payload.UserHash;

        if (userHash == null)
        {
            return StatusCode(401);
        }


        var personalnote = new PN();
        personalnote.NoteDate = createPersonalNoteRequest.NoteDate;
        personalnote.NoteContent = createPersonalNoteRequest.NoteContent;

        var response = await lifetreeService.GetOnePNWithLifetree(userHash, personalnote);


        if (response.HasError == false)
        {
            return Ok(response);
        }


        return StatusCode(500);
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
