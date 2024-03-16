namespace Peace.Lifelog.LLIWebService;

using Peace.Lifelog.LLI;
using Microsoft.AspNetCore.Mvc; // Namespace needed for using Controllers

[ApiController]
[Route("lli")]  // Defines the default parent URL path for all action methods to be the name of controller
public class LLIController : ControllerBase
{
    [HttpPost]
    [Route("postLLI")]
    public async Task<IActionResult> PostLLI(string userHash, string title, string category, string description, string status, string visibility, string deadline, int cost, string recurrenceStatus, string recurrenceFrequency)
    {
        var lliService = new LLIService();

        var lli = new LLI();
        lli.Title = title;
        lli.Category = category;
        lli.Description = description;
        lli.Status = status;
        lli.Visibility = visibility;
        lli.Deadline = deadline;
        lli.Cost = cost;
        lli.Recurrence.Status = recurrenceStatus;
        lli.Recurrence.Frequency = recurrenceFrequency;

        var response = await lliService.CreateLLI(userHash, lli);
  
        return Ok(response);
    }

    [HttpGet] 
    [Route("getAllLLI")]
    public async Task<IActionResult> GetAllLLI(string userHash)
    {
        var lliService = new LLIService();
        var response = await lliService.GetAllLLIFromUser(userHash);
  
        return Ok(response);
    }

    [HttpPut]
    [Route("putLLI")]
    public async Task<IActionResult> PutLLI(
        string userHash, string lliId, string title = "", string category = "", 
        string description = "", string status = "", string visibility = "", string deadline = "", 
        int? cost = null, string recurrenceStatus = "", string recurrenceFrequency = "")
    {
        var lliService = new LLIService();

        var newLLI = new LLI();
        newLLI.LLIID = lliId;
        newLLI.Title = title;
        newLLI.Category = category;
        newLLI.Description = description;
        newLLI.Status = status;
        newLLI.Visibility = visibility;
        newLLI.Deadline = deadline;
        newLLI.Cost = cost;
        newLLI.Recurrence.Status = recurrenceStatus;
        newLLI.Recurrence.Frequency = recurrenceFrequency;

        var response = await lliService.UpdateLLI(userHash, newLLI);

        return Ok(response);
    }

[HttpDelete] 
    [Route("deleteLLI")]
    public async Task<IActionResult> DeleteLLI(string userHash, string lliId)
    {
        var lliService = new LLIService();
        var lli = new LLI();
        lli.LLIID = lliId;

        var response = await lliService.DeleteLLI(userHash, lli);
  
        return Ok(response);
    }


}