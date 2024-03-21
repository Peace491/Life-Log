﻿namespace Peace.Lifelog.LLIWebService;

using Peace.Lifelog.DataAccess;
using Peace.Lifelog.LLI;
using Peace.Lifelog.Logging;
using Peace.Lifelog.Security;
using Microsoft.AspNetCore.Mvc; // Namespace needed for using Controllers
using System.Text.Json;
using back_end;
 
[ApiController]
[Route("lli")]  // Defines the default parent URL path for all action methods to be the name of controller
public class LLIController : ControllerBase
{
    private CreateDataOnlyDAO createDataOnlyDAO;
    private ReadDataOnlyDAO readDataOnlyDAO;
    private UpdateDataOnlyDAO updateDataOnlyDAO;
    private DeleteDataOnlyDAO deleteDataOnlyDAO;
    private LogTarget logTarget;
    private Logging logging;
    private LLIService lliService;
    public LLIController() {
        this.createDataOnlyDAO = new CreateDataOnlyDAO();
        this.readDataOnlyDAO = new ReadDataOnlyDAO();
        this.updateDataOnlyDAO = new UpdateDataOnlyDAO();
        this.deleteDataOnlyDAO = new DeleteDataOnlyDAO();
        this.logTarget = new LogTarget(this.createDataOnlyDAO);
        this.logging = new Logging(this.logTarget);
        this.lliService = new LLIService(this.createDataOnlyDAO, this.readDataOnlyDAO, this.updateDataOnlyDAO, this.deleteDataOnlyDAO, this.logging);
        
    }
    [HttpPost]
    [Route("postLLI")]
    public async Task<IActionResult> PostLLI([FromBody] PostLLIRequest createLLIRequest)
    {
        var jwtToken = JsonSerializer.Deserialize<Jwt>(Request.Headers["Token"]);
        var userHash = jwtToken.Payload.UserHash;

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

        var response = await this.lliService.CreateLLI(userHash, lli);

        return Ok(response);
    }

    [HttpGet]
    [Route("getAllLLI")]
    public async Task<IActionResult> GetAllLLI()
    {
        
        var jwtToken = JsonSerializer.Deserialize<Jwt>(Request.Headers["Token"]);
        var userHash = jwtToken.Payload.UserHash;

        var response = await this.lliService.GetAllLLIFromUser(userHash);

        return Ok(response);
    }

    [HttpPut]
    [Route("putLLI")]
    public async Task<IActionResult> PutLLI([FromBody]PutLLIRequest updateLLIRequest)
    {
        var jwtToken = JsonSerializer.Deserialize<Jwt>(Request.Headers["Token"]);
        var userHash = jwtToken.Payload.UserHash;

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

        Console.WriteLine(response.ErrorMessage);

        return Ok(response);
    }

    [HttpDelete]
    [Route("deleteLLI")]
    public async Task<IActionResult> DeleteLLI(string lliId)
    {
        var jwtToken = JsonSerializer.Deserialize<Jwt>(Request.Headers["Token"]);
        var userHash = jwtToken.Payload.UserHash;

        var lli = new LLI();
        lli.LLIID = lliId;

        var response = await this.lliService.DeleteLLI(userHash, lli);

        return Ok(response);
    }


}

public class RequestHeader() {
    public string ContentType { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
}