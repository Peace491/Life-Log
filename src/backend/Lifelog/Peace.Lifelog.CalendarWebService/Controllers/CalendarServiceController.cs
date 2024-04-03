namespace Peace.Lifelog.CalendarWebService;

using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Peace.Lifelog.CalendarService;
using DomainModels;
using Peace.Lifelog.CalendarWebService.Models;
using Peace.Lifelog.LLI;
using Peace.Lifelog.PersonalNote;
using back_end;




[ApiController]
[Route("calendarService")]
public class CalendarServiceController : ControllerBase
{
    private CalendarService calendarService;
    public CalendarServiceController()
    {
        this.calendarService = new CalendarService();
    }


    [HttpGet]
    [Route("getMonthLLI")]
    public async Task<IActionResult> GetMonthLLI(int month, int year)
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

          

     
        var response = await calendarService.GetMonthLLI(userHash, month, year);

        if (response.HasError == false)
        {
            return Ok(response);
        }


        return StatusCode(500);

    }

    [HttpGet]
    [Route("getMonthPN")]
    public async Task<IActionResult> GetMonthPN(string notedate)
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
        personalnote.NoteDate = notedate;

        var response = await this.calendarService.GetOnePNWithCalendar(userHash, personalnote);

        if (response.HasError == false)
        {
            return Ok(response);
        }


        return StatusCode(500);

    }




    [HttpPost]
    [Route("postLLI")]
    public async Task<IActionResult> PostLLI([FromBody] PostLLIRequest createLLIRequest)
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
        lli.Title = createLLIRequest.Title;
        lli.Category1 = createLLIRequest.Category1;
        lli.Category2 = createLLIRequest.Category2;
        lli.Category3 = createLLIRequest.Category3;
        lli.Description = createLLIRequest.Description;
        lli.Status = createLLIRequest.Status;
        lli.Visibility = createLLIRequest.Visibility;
        lli.Deadline = createLLIRequest.Deadline;
        lli.Cost = createLLIRequest.Cost;
        lli.Recurrence.Status = createLLIRequest.RecurrenceStatus;
        lli.Recurrence.Frequency = createLLIRequest.RecurrenceFrequency;

        var response = await this.calendarService.CreateLLIWithCalendar(userHash, lli);

        if (response.HasError == false)
        {
            return Ok(response);
        }
        else if (response.ErrorMessage!.Contains("invalid") || response.ErrorMessage!.Contains("completed within the last year"))
        {
            return StatusCode(400, response.ErrorMessage);
        }
        
        
        return StatusCode(500, response.ErrorMessage);
        
        
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
        lli.Category1 = updateLLIRequest.Category1;
        lli.Category2 = updateLLIRequest.Category2;
        lli.Category3 = updateLLIRequest.Category3;
        lli.Description = updateLLIRequest.Description;
        lli.Status = updateLLIRequest.Status;
        lli.Visibility = updateLLIRequest.Visibility;
        lli.Deadline = updateLLIRequest.Deadline;
        lli.Cost = updateLLIRequest.Cost;
        lli.Recurrence.Status = updateLLIRequest.RecurrenceStatus;
        lli.Recurrence.Frequency = updateLLIRequest.RecurrenceFrequency;

        var response = await this.calendarService.EditLLIWithCalendar(userHash, lli);

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


    // Create PN

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

        var response = await this.calendarService.CreatePNWithCalendar(userHash, personalnote);

        if (!response.HasError)
        {
            return Ok(response);
        }
        else if (response.ErrorMessage!.Contains("invalid"))
        {
            return StatusCode(400, response.ErrorMessage);
        }
        
        
        return StatusCode(500, response.ErrorMessage);
        

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

        var response = await this.calendarService.UpdatePNWithCalendar(userHash, personalnote);

        if (response.HasError == false)
        {
            return Ok(response);
        }
        else if (response.ErrorMessage!.Contains("invalid") || response.ErrorMessage!.Contains("completed within the last year"))
        {
            return StatusCode(400, response.ErrorMessage);
        }
        
        
        return StatusCode(500);
        
    }

}
