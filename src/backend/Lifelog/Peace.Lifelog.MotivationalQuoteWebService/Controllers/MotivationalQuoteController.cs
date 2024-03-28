using Microsoft.AspNetCore.Mvc;
//using Peace.Lifelog.MotivationalQuotesWebService.Models;

namespace Peace.Lifelog.MotivationalQuoteWebService.Controllers;

using System.Text.Json;
using DomainModels;
using Peace.Lifelog.MotivationalQuote;

[ApiController]
[Route("quotes")]
public class MotivationalQuoteController : ControllerBase
{
    [HttpGet]
    [Route("getQuote")]
    public async Task<IActionResult> GetPhrase()
    {
        var motivationalQuoteService = new MotivationalQuoteService();
        var getQuoteResponse = await motivationalQuoteService.GetPhrase();

        if (getQuoteResponse.HasError)
        {
            // Handle the error case
            return BadRequest(JsonSerializer.Serialize<Response>(getQuoteResponse));
        }
        if (getQuoteResponse.Output == null)
        {
            // Handle the case where the output is null
            return NotFound(JsonSerializer.Serialize<Response>(getQuoteResponse));
        }
        else
        {
            // Return the successful response
            return Ok(JsonSerializer.Serialize<Response>(getQuoteResponse));
        }
    }
}
