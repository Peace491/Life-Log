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
    [Route("postPN")]
    public async Task<IActionResult> PostPersonalNote([FromBody] PostPersonalNoteRequest createPersonalNoteRequest)
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

    [HttpGet]
    [Route("getPN")]
    public async Task<IActionResult> GetPersonalNote(string notedate)
    {
        if (Request.Headers == null)
        {
            return StatusCode(401);
        }

        // Console.WriteLine(Request.Headers);
        Console.WriteLine(Request.Headers["Token"]);
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
        personalnote.NoteDate = notedate;

        var response = await this.personalNoteService.ViewPersonalNote(userHash, personalnote);

        if (response.HasError == false)
        {
            return Ok(response);
        }
        if (response.ErrorMessage!.Contains("UserHash can not be empty"))
        {
            Console.WriteLine(response.ErrorMessage);
            return StatusCode(400, response.ErrorMessage);
        }
        if (response.ErrorMessage!.Equals("The Date is Invalid"))
        {
            Console.WriteLine(response.ErrorMessage);
            return StatusCode(400, response.ErrorMessage);
        }
        else
        {
            Console.WriteLine(response.ErrorMessage);
            return StatusCode(500);
        }
    }

    [HttpPut]
    [Route("putPN")]
    public async Task<IActionResult> PutPersonalNote([FromBody] PutPersonalNoteRequest updatePersonalNoteRequest)
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
        personalnote.NoteId = updatePersonalNoteRequest.NoteId;
        personalnote.NoteDate = updatePersonalNoteRequest.NoteDate;
        personalnote.NoteContent = updatePersonalNoteRequest.NoteContent;

        var response = await this.personalNoteService.UpdatePersonalNote(userHash, personalnote);

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
    [Route("deletePN")]
    public async Task<IActionResult> DeleteLLI(string noteId)
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

        // initializing the note object with an NoteId
        var personalnote = new PN();
        personalnote.NoteId = noteId;

        var response = await this.personalNoteService.DeletePersonalNote(userHash, personalnote);

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
    }


}


public class RequestHeader()
{
    public string ContentType { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
}