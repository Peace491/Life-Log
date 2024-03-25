namespace Peace.Lifelog.PersonalNoteWebService;

using back_end;
using Microsoft.AspNetCore.Mvc; // Namespace needed for using Controllers
using Peace.Lifelog.DataAccess;
using Peace.Lifelog.Logging;
using Peace.Lifelog.PersonalNote;
using System.Text.Json;

[ApiController]
[Route("personalnote")]  // Defines the default parent URL path for all action methods to be the name of controller
public class PersonalNoteController : ControllerBase
{
    private CreateDataOnlyDAO createDataOnlyDAO;
    private ReadDataOnlyDAO readDataOnlyDAO;
    private UpdateDataOnlyDAO updateDataOnlyDAO;
    private DeleteDataOnlyDAO deleteDataOnlyDAO;
    private LogTarget logTarget;
    private Logging logging;
    private PersonalNoteService personalNoteService;
    public PersonalNoteController()
    {
        this.createDataOnlyDAO = new CreateDataOnlyDAO();
        this.readDataOnlyDAO = new ReadDataOnlyDAO();
        this.updateDataOnlyDAO = new UpdateDataOnlyDAO();
        this.deleteDataOnlyDAO = new DeleteDataOnlyDAO();
        this.logTarget = new LogTarget(this.createDataOnlyDAO);
        this.logging = new Logging(this.logTarget);
        this.personalNoteService = new PersonalNoteService(this.createDataOnlyDAO, this.readDataOnlyDAO, this.updateDataOnlyDAO, this.deleteDataOnlyDAO, this.logging);

    }
    [HttpPost]
    [Route("postpersonalnote")]
    public async Task<IActionResult> PostLLI([FromBody] PostPersonalNoteRequest createPersonalNoteRequest)
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

        var personalnote = new PN();
        personalnote.NoteDate = createPersonalNoteRequest.NoteDate;
        personalnote.NoteContent = createPersonalNoteRequest.NoteContent;

        var response = await this.personalNoteService.CreatePersonalNote(userHash, personalnote);

        if (!response.HasError)
        {
            return Ok(response);
        }
        else if (response.ErrorMessage!.Contains("invalid"))
        {
            return StatusCode(400, response.ErrorMessage);
        }
        else
        {
            return StatusCode(500, response.ErrorMessage);
        }

    }

    /*[HttpGet]
    [Route("getAllLLI")]
    public async Task<IActionResult> GetAllLLI()
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

        var response = await this.lliService.GetAllLLIFromUser(userHash);

        if (response.HasError == false)
        {
            return Ok(response);
        }
        else if (response.ErrorMessage!.Contains("UserHash can not be empty"))
        {
            return StatusCode(400, response.ErrorMessage);
        }
        else
        {
            return StatusCode(500);
        }
    }

    [HttpPut]
    [Route("putLLI")]
    public async Task<IActionResult> PutLLI([FromBody] PutLLIRequest updateLLIRequest)
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

        var lli = new LLI();
        lli.LLIID = updateLLIRequest.LLIID;
        lli.Title = updateLLIRequest.Title;
        lli.Categories = updateLLIRequest.Categories;
        lli.Description = updateLLIRequest.Description;
        lli.Status = updateLLIRequest.Status;
        lli.Visibility = updateLLIRequest.Visibility;
        lli.Deadline = updateLLIRequest.Deadline;
        lli.Cost = updateLLIRequest.Cost;
        lli.Recurrence.Status = updateLLIRequest.RecurrenceStatus;
        lli.Recurrence.Frequency = updateLLIRequest.RecurrenceFrequency;

        var response = await this.lliService.UpdateLLI(userHash, lli);

        if (response.HasError == false)
        {
            return Ok(response);
        }
        else if (response.ErrorMessage!.Contains("invalid") || response.ErrorMessage!.Contains("completed within the last year"))
        {
            return StatusCode(400, response.ErrorMessage);
        }
        else
        {
            return StatusCode(500);
        }
    }

    [HttpDelete]
    [Route("deleteLLI")]
    public async Task<IActionResult> DeleteLLI(string lliId)
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

        var lli = new LLI();
        lli.LLIID = lliId;

        var response = await this.lliService.DeleteLLI(userHash, lli);

        if (response.HasError == false)
        {
            return Ok(response);
        }
        else if (response.ErrorMessage!.Contains("invalid"))
        {
            return StatusCode(400, response.ErrorMessage);
        }
        else
        {
            return StatusCode(500);
        }
    }*/


}


public class RequestHeader()
{
    public string ContentType { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
}