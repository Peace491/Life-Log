namespace Peace.Lifelog.MapsWebService;

using back_end;
using Microsoft.AspNetCore.Mvc;
using Peace.Lifelog.Logging;
using Peace.Lifelog.Map;
using Peace.Lifelog.Security;
using System;
using System.Text.Json;
using System.Threading.Tasks;

[ApiController]
[Route("maps")]
public sealed class MapsController : ControllerBase
{
    private readonly IPinService _pinService;
    private JWTService jwtService;
    private readonly ILogging _logger;

    public MapsController(IPinService pinService, ILogging logger)
    {
        _pinService = pinService;
        this.jwtService = new JWTService();
        _logger = logger;
    }

    [HttpGet("getAllUserPin")]
    public async Task<IActionResult> GetAllPinFromUser()
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

            /*if (payload.AppPrincipal == null)
            {
                return StatusCode(400, "AppPrincipal is null.");
            }*/

            // need to double check what is being passed in here
            var response = await _pinService.GetAllPinFromUser(userHash);

            if (response == null)
            {
                return StatusCode(404, "Couldn't retrieve all Pin for user.");
            }

            if (response.HasError)
            {
                return StatusCode(400, "An error occurred while processing your request.");
            }

            /*if (response.Output == null)
            {
                return StatusCode(404, "Couldn't retrieve all Pin for user.");
            }*/

            /*need to check if this is what you want below*/
            _ = await _logger.CreateLog("Logs", userHash, "INFO", "System", "Retrieved all Pin successfully.");
            return Ok(response);
        }
        catch (Exception ex)
        {
            _ = await _logger.CreateLog("Logs", "MapsController", "ERROR", "System", ex.Message);
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [HttpPost("createPin")]
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
                return StatusCode(400, "AppPrincipal is null.");
            }
 
            CreatePinRequest createPinRequest = new();
            createPinRequest.Principal = payload.AppPrincipal;
            createPinRequest.PinId = payload.PinId;
            createPinRequest.LLIId = payload.LLIId;
            createPinRequest.Address = payload.Address;
            createPinRequest.Latitude = payload.Latitude;
            createPinRequest.Longitude = payload.Longitude;

            // need to double check what is being passed in here
            var response = await _pinService.CreatePin(createPinRequest);

            if (response == null)
            {
                return StatusCode(404, "Couldn't create a Pin.");
            }

            if (response.HasError)
            {
                return StatusCode(400, "An error occurred while processing your request.");
            }

            if (response.Output == null)
            {
                return StatusCode(404, "Couldn't create a Pin.");
            }

            /*need to check if this is what you want below*/
            _ = await _logger.CreateLog("Logs", payload.AppPrincipal.UserId, "INFO", "System", "Created Pin successfully.");
            return Ok(response);
        }
        catch (Exception ex)
        {
            _ = await _logger.CreateLog("Logs", "MapsController", "ERROR", "System", ex.Message);
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [HttpDelete("deletePin")]
    public async Task<IActionResult> DeletePin(string PinId)
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
            // need to double check what is being passed in here
            var response = await _pinService.DeletePin(PinId, userHash);

            if (response == null)
            {
                return StatusCode(404, "Couldn't delete a Pin.");
            }

            if (response.HasError)
            {
                return StatusCode(400, "An error occurred while processing your request.");
            }

            /*need to check if this is what you want below*/
            _ = await _logger.CreateLog("Logs", userHash, "INFO", "System", "Deleted Pin successfully.");
            return Ok(response);
        }
        catch (Exception ex)
        {
            _ = await _logger.CreateLog("Logs", "MapsController", "ERROR", "System", ex.Message);
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [HttpGet("viewPin")]
    public async Task<IActionResult> ViewPin(string pinId)
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

            ViewPinRequest viewPinRequest = new();
            viewPinRequest.PinId = pinId;

            // need to double check what is being passed in here
            var response = await _pinService.ViewPin(viewPinRequest);

            if (response == null)
            {
                return StatusCode(404, "Couldn't view a Pin.");
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
            _ = await _logger.CreateLog("Logs", userHash, "INFO", "System", "Viewed a Pin successfully.");
            return Ok(response);
        }
        catch (Exception ex)
        {
            _ = await _logger.CreateLog("Logs", "MapsController", "ERROR", "System", ex.Message);
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [HttpPut("updatePin")]
    public async Task<IActionResult> UpdatePin([FromBody] PutUpdatePinRequest payload)
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
                return StatusCode(400, "AppPrincipal is null.");
            }

            if (payload.AppPrincipal == null)
            {
                return StatusCode(400, "AppPrincipal is null.");
            }

            UpdatePinRequest updatePinRequest = new();
            updatePinRequest.Principal = payload.AppPrincipal;
            updatePinRequest.PinId = payload.PinId;
            updatePinRequest.LLIId = payload.LLIId;
            updatePinRequest.Address = payload.Address;
            updatePinRequest.Latitude = payload.Latitude;
            updatePinRequest.Longitude = payload.Longitude;

            // need to double check what is being passed in here
            var response = await _pinService.UpdatePin(updatePinRequest);

            if (response == null)
            {
                return StatusCode(404, "Couldn't update a Pin.");
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
            _ = await _logger.CreateLog("Logs", payload.AppPrincipal.UserId, "INFO", "System", "Updated Pin successfully.");
            return Ok(response);
        }
        catch (Exception ex)
        {
            _ = await _logger.CreateLog("Logs", "MapsController", "ERROR", "System", ex.Message);
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [HttpPut("editPinLLI")]
    public async Task<IActionResult> EditPinLLI([FromBody] PutEditPinLIIRequest payload)
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
                return StatusCode(400, "AppPrincipal is null.");
            }

            if (payload.AppPrincipal == null)
            {
                return StatusCode(400, "AppPrincipal is null.");
            }

            // Testing
            EditPinLIIRequest editPinLIIRequest = new();
            editPinLIIRequest.Principal = payload.AppPrincipal;
            editPinLIIRequest.PinId = payload.PinId;
            editPinLIIRequest.LLIId = payload.LLIId;
            editPinLIIRequest.Address = payload.Address;
            editPinLIIRequest.Latitude = payload.Latitude;
            editPinLIIRequest.Longitude = payload.Longitude;

            // need to double check what is being passed in here
            var response = await _pinService.EditPinLLI(editPinLIIRequest);

            if (response == null)
            {
                return StatusCode(404, "Couldn't edit pin LLI.");
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
            _ = await _logger.CreateLog("Logs", payload.AppPrincipal.UserId, "INFO", "System", "Edited Pin LLI successfully.");
            return Ok(response);
        }
        catch (Exception ex)
        {
            _ = await _logger.CreateLog("Logs", "MapsController", "ERROR", "System", ex.Message);
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [HttpPut("updateLog")]
    public async Task<IActionResult> UpdateLog([FromBody] PutUpdateLogRequest payload)
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
                return StatusCode(400, "AppPrincipal is null.");
            }

            if (payload.AppPrincipal == null)
            {
                return StatusCode(400, "AppPrincipal is null.");
            }

            UpdateLogRequest updateLogRequest = new();
            updateLogRequest.Principal = payload.AppPrincipal;
            // need to double check what is being passed in here
            var response = await _pinService.updateLog(updateLogRequest);

            if (response == null)
            {
                return StatusCode(404, "Couldn't Update Log.");
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
            _ = await _logger.CreateLog("Logs", payload.AppPrincipal.UserId, "INFO", "System", "Update Log successfully.");
            return Ok(response);
        }
        catch (Exception ex)
        {
            _ = await _logger.CreateLog("Logs", "MapsController", "ERROR", "System", ex.Message);
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [HttpGet("getPinStatus")]
    public async Task<IActionResult> GetPinStatusUser(string lliId)
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

            // need to double check what is being passed in here
            var response = await _pinService.FetchPinStatus(lliId, userHash);

            if (response.HasError)
            {
                return StatusCode(400, "An error occurred while processing your request.");
            }

            /*need to check if this is what you want below*/
            _ = await _logger.CreateLog("Logs", userHash, "INFO", "System", "Retrieved all Pin successfully.");
            return Ok(response);
        }
        catch (Exception ex)
        {
            _ = await _logger.CreateLog("Logs", "MapsController", "ERROR", "System", ex.Message);
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }
}
