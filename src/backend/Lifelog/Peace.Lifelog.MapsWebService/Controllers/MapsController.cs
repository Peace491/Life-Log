namespace Peace.Lifelog.MapsWebService;

using Microsoft.AspNetCore.Mvc;

using System;
using System.Text.Json;
using System.Threading.Tasks;

using Peace.Lifelog.Logging;
using Peace.Lifelog.Map;
using Peace.Lifelog.LocationRecommendation;
using Peace.Lifelog.Security;

[ApiController]
[Route("maps")]
public sealed class MapsController : ControllerBase
{
    private readonly IPinService _pinService;
    private readonly ILocationRecommendationService _locationRecommendationService;
    private JWTService jwtService;
    private readonly ILogging _logger;

    public MapsController(IPinService pinService, ILocationRecommendationService locationRecommendationService, ILogging logger)
    {
        _pinService = pinService;
        _locationRecommendationService = locationRecommendationService;
        this.jwtService = new JWTService();
        _logger = logger;
    }

    [HttpPost("GetAllUserLLI")]
    public async Task<IActionResult> GetAllUserLLI([FromBody] PostGetAllUserLLIRequest payload)
    {
        try
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

            if (!jwtService.IsJwtValid(jwtToken))
            {
                return StatusCode(401);
            }

            if (payload.AppPrincipal == null)
            {
                return Status Code(400, "AppPrincipal is null.");
            }

            // need to double check what is being passed in here
            var response = await _pinService.GetAllUserLLI(payload.AppPrincipal, payload.userHash);

            if (response == null)
            {
                return Status Code(404, "Couldn't retrieve all LLI for user.");
            }

            if (response.HasError)
            {
                return Status Code(400, "An error occurred while processing your request.");
            }

            if (response.Output == null)
            {
                return Status Code(404, "Couldn't retrieve all LLI for user.");
            }

            /*need to check if this is what you want below*/
            current = await _logger.CreateLog("Logs", payload.AppPrincipal.UserId, "INFO", "System", "Retrieved all LLI successfully.");
            return Status Code(200, JsonSerializer.Serialize<ICollection<Object>>(response.Output));
        }
        catch (Exception ex)
        {
            current = await _logger.CreateLog("Logs", "MapsController", "ERROR", "System", ex.Message);
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [HttpPost("GetAllPinFromUser")]
    public async Task<IActionResult> GetAllPinFromUser([FromBody] PostGetAllPinFromUserRequest payload)
    {
        try
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

            if (!jwtService.IsJwtValid(jwtToken))
            {
                return StatusCode(401);
            }

            if (payload.AppPrincipal == null)
            {
                return Status Code(400, "AppPrincipal is null.");
            }

            if (payload.AppPrincipal == null)
            {
                return Status Code(400, "AppPrincipal is null.");
            }

            // need to double check what is being passed in here
            var response = await _pinService.GetAllPinFromUser(payload.userHash);

            if (response == null)
            {
                return Status Code(404, "Couldn't retrieve all Pin for user.");
            }

            if (response.HasError)
            {
                return Status Code(400, "An error occurred while processing your request.");
            }

            if (response.Output == null)
            {
                return Status Code(404, "Couldn't retrieve all Pin for user.");
            }

            /*need to check if this is what you want below*/
            current = await _logger.CreateLog("Logs", payload.AppPrincipal.UserId, "INFO", "System", "Retrieved all Pin successfully.");
            return Status Code(200, JsonSerializer.Serialize<ICollection<Object>>(response.Output));
        }
        catch (Exception ex)
        {
            current = await _logger.CreateLog("Logs", "MapsController", "ERROR", "System", ex.Message);
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [HttpPost("CreatePin")]
    public async Task<IActionResult> CreatePin([FromBody] PostCreatePinRequest payload)
    {
        try
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

            if (!jwtService.IsJwtValid(jwtToken))
            {
                return StatusCode(401);
            }

            if (payload.AppPrincipal == null)
            {
                return Status Code(400, "AppPrincipal is null.");
            }

            if (payload.AppPrincipal == null)
            {
                return Status Code(400, "AppPrincipal is null.");
            }

            // need to double check what is being passed in here
            var response = await _pinService.CreatePin(payload);

            if (response == null)
            {
                return Status Code(404, "Couldn't create a Pin.");
            }

            if (response.HasError)
            {
                return Status Code(400, "An error occurred while processing your request.");
            }

            if (response.Output == null)
            {
                return Status Code(404, "Couldn't create a Pin.");
            }

            /*need to check if this is what you want below*/
            current = await _logger.CreateLog("Logs", payload.AppPrincipal.UserId, "INFO", "System", "Created Pin successfully.");
            return Status Code(200, JsonSerializer.Serialize<ICollection<Object>>(response.Output));
        }
        catch (Exception ex)
        {
            current = await _logger.CreateLog("Logs", "MapsController", "ERROR", "System", ex.Message);
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [HttpPost("DeletePin")]
    public async Task<IActionResult> DeletePin([FromBody] PostDeletePinRequest payload)
    {
        try
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

            if (!jwtService.IsJwtValid(jwtToken))
            {
                return StatusCode(401);
            }

            if (payload.AppPrincipal == null)
            {
                return Status Code(400, "AppPrincipal is null.");
            }

            if (payload.AppPrincipal == null)
            {
                return StatusCode(400, "AppPrincipal is null.");
            }

            // need to double check what is being passed in here
            var response = await _pinService.DeletePin(payload);

            if (response == null)
            {
                return Status Code(404, "Couldn't delete a Pin.");
            }

            if (response.HasError)
            {
                return StatusCode(400, "An error occurred while processing your request.");
            }

            if (response.Output == null)
            {
                return StatusCode(404, "Couldn't delete a Pin.");
            }

            /*need to check if this is what you want below*/
            current = await _logger.CreateLog("Logs", payload.AppPrincipal.UserId, "INFO", "System", "Deleted Pin successfully.");
            return StatusCode(200, JsonSerializer.Serialize<ICollection<Object>>(response.Output));
        }
        catch (Exception ex)
        {
            current = await _logger.CreateLog("Logs", "MapsController", "ERROR", "System", ex.Message);
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [HttpPost("ViewPin")]
    public async Task<IActionResult> ViewPin([FromBody] PostViewPinRequest payload)
    {
        try
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

            if (!jwtService.IsJwtValid(jwtToken))
            {
                return StatusCode(401);
            }

            if (payload.AppPrincipal == null)
            {
                return Status Code(400, "AppPrincipal is null.");
            }

            if (payload.AppPrincipal == null)
            {
                return StatusCode(400, "AppPrincipal is null.");
            }

            // need to double check what is being passed in here
            var response = await _pinService.ViewPin(payload);

            if (response == null)
            {
                return Status Code(404, "Couldn't view a Pin.");
            }

            if (response.HasError)
            {
                return StatusCode(400, "An error occurred while processing your request.");
            }

            if (response.Output == null)
            {
                return StatusCode(404, "Couldn't view a Pin.");
            }

            /*need to check if this is what you want below*/
            current = await _logger.CreateLog("Logs", payload.AppPrincipal.UserId, "INFO", "System", "Viewed a Pin successfully.");
            return StatusCode(200, JsonSerializer.Serialize<ICollection<Object>>(response.Output));
        }
        catch (Exception ex)
        {
            current = await _logger.CreateLog("Logs", "MapsController", "ERROR", "System", ex.Message);
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [HttpPost("UpdatePin")]
    public async Task<IActionResult> UpdatePin([FromBody] PostUpdatePinRequest payload)
    {
        try
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

            if (!jwtService.IsJwtValid(jwtToken))
            {
                return StatusCode(401);
            }

            if (payload.AppPrincipal == null)
            {
                return Status Code(400, "AppPrincipal is null.");
            }

            if (payload.AppPrincipal == null)
            {
                return StatusCode(400, "AppPrincipal is null.");
            }

            // need to double check what is being passed in here
            var response = await _pinService.UpdatePin(payload);

            if (response == null)
            {
                return Status Code(404, "Couldn't update a Pin.");
            }

            if (response.HasError)
            {
                return StatusCode(400, "An error occurred while processing your request.");
            }

            if (response.Output == null)
            {
                return StatusCode(404, "Couldn't update a Pin.");
            }

            /*need to check if this is what you want below*/
            current = await _logger.CreateLog("Logs", payload.AppPrincipal.UserId, "INFO", "System", "Updated Pin successfully.");
            return StatusCode(200, JsonSerializer.Serialize<ICollection<Object>>(response.Output));
        }
        catch (Exception ex)
        {
            current = await _logger.CreateLog("Logs", "MapsController", "ERROR", "System", ex.Message);
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [HttpPost("EditPinLLI")]
    public async Task<IActionResult> EditPinLLI([FromBody] PostEditPinLLIRequest payload)
    {
        try
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

            if (!jwtService.IsJwtValid(jwtToken))
            {
                return StatusCode(401);
            }

            if (payload.AppPrincipal == null)
            {
                return Status Code(400, "AppPrincipal is null.");
            }

            if (payload.AppPrincipal == null)
            {
                return StatusCode(400, "AppPrincipal is null.");
            }

            // need to double check what is being passed in here
            var response = await _pinService.EditPinLLI(payload);

            if (response == null)
            {
                return Status Code(404, "Couldn't edit pin LLI.");
            }

            if (response.HasError)
            {
                return StatusCode(400, "An error occurred while processing your request.");
            }

            if (response.Output == null)
            {
                return StatusCode(404, "Couldn't edit pin LLI.");
            }

            /*need to check if this is what you want below*/
            current = await _logger.CreateLog("Logs", payload.AppPrincipal.UserId, "INFO", "System", "Edit Pin LLI successfully.");
            return StatusCode(200, JsonSerializer.Serialize<ICollection<Object>>(response.Output));
        }
        catch (Exception ex)
        {
            current = await _logger.CreateLog("Logs", "MapsController", "ERROR", "System", ex.Message);
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [HttpPost("UpdateLog")]
    public async Task<IActionResult> UpdateLog([FromBody] PostUpdateLogRequest payload)
    {
        try
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

            if (!jwtService.IsJwtValid(jwtToken))
            {
                return StatusCode(401);
            }

            if (payload.AppPrincipal == null)
            {
                return Status Code(400, "AppPrincipal is null.");
            }

            if (payload.AppPrincipal == null)
            {
                return StatusCode(400, "AppPrincipal is null.");
            }

            // need to double check what is being passed in here
            var response = await _pinService.UpdateLog(payload);

            if (response == null)
            {
                return Status Code(404, "Couldn't Update Log.");
            }

            if (response.HasError)
            {
                return StatusCode(400, "An error occurred while processing your request.");
            }

            if (response.Output == null)
            {
                return StatusCode(404, "Couldn't Update Log.");
            }

            /*need to check if this is what you want below*/
            current = await _logger.CreateLog("Logs", payload.AppPrincipal.UserId, "INFO", "System", "Update Log successfully.");
            return StatusCode(200, JsonSerializer.Serialize<ICollection<Object>>(response.Output));
        }
        catch (Exception ex)
        {
            current = await _logger.CreateLog("Logs", "MapsController", "ERROR", "System", ex.Message);
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }
}
