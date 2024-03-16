namespace Peace.Lifelog.LLIWebService;

using Peace.Lifelog.LLI;
using Microsoft.AspNetCore.Mvc; // Namespace needed for using Controllers

[ApiController]
[Route("lli")]  // Defines the default parent URL path for all action methods to be the name of controller
public class LLIController : ControllerBase
{
    [HttpPost]
    [Route("postLLI")]
    public async Task<IActionResult> PostLLI([FromBody] PostLLIRequest createLLIRequest)
    {
        var lliService = new LLIService();

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

        var response = await lliService.CreateLLI(createLLIRequest.UserHash, lli);

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
    public async Task<IActionResult> PutLLI([FromBody]PutLLIRequest updateLLIRequest)
    {
        var lliService = new LLIService();

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

        var response = await lliService.UpdateLLI(updateLLIRequest.UserHash, lli);

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