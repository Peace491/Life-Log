namespace Peace.Lifelog.LocationRecommendationWebService.Controllers;

using back_end;
using Microsoft.AspNetCore.Mvc;
using Peace.Lifelog.Logging;
using Peace.Lifelog.LocationRecommendation;
using Peace.Lifelog.Security;
using System;
using System.Text.Json;
using System.Threading.Tasks;

// LocationRecommendation

// locationRecommendation

[ApiController]
[Route("locationrecommendation")]
public sealed class LocationRecommendationController : ControllerBase
{
    private readonly ILocationRecommendationService _locationRecommendationService;
    private JWTService jwtService;
    private readonly ILogging _logger;

    public LocationRecommendationController(ILocationRecommendationService locationRecommendationService, ILogging logger)
    {
        _locationRecommendationService = locationRecommendationService;
        this.jwtService = new JWTService();
        _logger = logger;
    }

    [HttpGet("getLocationRecommendation")]
    public async Task<IActionResult> GetLocationRecommendation()
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
            var response = await _locationRecommendationService.GetLocationRecommendation();

            if (response == null)
            {
                return StatusCode(404, "Couldn't retrieve all Pin for user.");
            }

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
            _ = await _logger.CreateLog("Logs", "LocationRecommendationController
        ", "ERROR", "System", ex.Message);
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [HttpGet("viewLocationRecommendation")]
    public async Task<IActionResult> ViewLocationRecommendation([FromBody])
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

            // need to double check what is being passed in here
            var response = await _locationRecommendationService.ViewLocationRecommendation();

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
            _ = await _logger.CreateLog("Logs", "LocationRecommendationController", "ERROR", "System", ex.Message);
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
            var response = await _locationRecommendationService.ViewPin(viewPinRequest);

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
            var response = await _locationRecommendationService.updateLog(updateLogRequest);

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
}
