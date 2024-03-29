namespace Peace.Lifelog.CalendarWebService;

using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Peace.Lifelog.CalendarService;
using DomainModels;
using Peace.Lifelog.CalendarWebService.Models;
using Peace.Lifelog.LLI;
using back_end;




[ApiController]
[Route("calendarService")]
public class CalendarServiceController : ControllerBase
{
    private CalendarService calendarService =  new CalendarService();
    /*public CalendarServiceController() { 
        this.calendarService = new CalendarService();
    }*/


    [HttpGet]
    [Route("getMonthData")]
    public async Task<IActionResult> GetMonthData()
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

        var response = await this.calendarService.GetMonthData(userHash);

        if (response.HasError == false)
        {
            return Ok(response);
        }

        return StatusCode(500);
    }

    [HttpPost]
    [Route("postNextMonth")]
    public async Task<IActionResult> PostNextMonth()
    {

        /*if (Request.Headers == null)
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
        }*/

        string userHash = "e1ZWxq+V6lOBKjppU9R+AtOToy5zSRaxV40jfllVcXY=";

        var response = await this.calendarService.NextMonth(userHash);

        if (response.HasError == false)
        {
            return Ok(response);
        }
        

        return StatusCode(500);

    }

    [HttpGet]
    [Route("getPrevMonth")]
    public async Task<IActionResult> GetPrevMonth()
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

        var response = await this.calendarService.PrevMonth(userHash);

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
        lli.Categories = createLLIRequest.Categories;
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
        lli.Categories = updateLLIRequest.Categories;
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

}
