namespace Peace.Lifelog.LocationRecommendationWebService.Controllers;

using System;
using System.Text.Json;
using System.Threading.Tasks;
using back_end;
using Microsoft.AspNetCore.Mvc;
using Peace.Lifelog.LocationRecommendation;
using Peace.Lifelog.Logging;
using Peace.Lifelog.Security;

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
            GetRecommendationRequest getRecommendationPayload = new GetRecommendationRequest();
            getRecommendationPayload.UserHash = userHash;
            // need to double check what is being passed in here
            var response = await _locationRecommendationService.GetRecommendation(userHash); //getRecommendationPayload

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
            _ = await _logger.CreateLog("Logs", "LocationRecommendationController", "ERROR", "System", ex.Message);
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [HttpGet("viewLocationRecommendation")]
    public async Task<IActionResult> ViewLocationRecommendation()
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

            var userHash = jwtToken!.Payload.UserHash;

            if (userHash == null)
            {
                return StatusCode(401);
            }

            if (!jwtService.IsJwtValid(jwtToken))
            {
                return StatusCode(401);
            }
            ViewRecommendationRequest viewRecommendationPayload = new ViewRecommendationRequest();
            viewRecommendationPayload.UserHash = userHash;

            // need to double check what is being passed in here
            var response = await _locationRecommendationService.ViewRecommendation(viewRecommendationPayload!);

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
            _ = await _logger.CreateLog("Logs", userHash, "INFO", "System", "Created Pin successfully.");
            return Ok(response);
        }
        catch (Exception ex)
        {
            _ = await _logger.CreateLog("Logs", "LocationRecommendationController", "ERROR", "System", ex.Message);
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [HttpGet("updateLog")]
    public async Task<IActionResult> UpdateLog()//[FromBody] PutUpdateLogRequest payload
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
            _ = await _logger.CreateLog("Logs", userHash, "INFO", "System", "Update Log successfully.");
            return StatusCode(200);
        }
        catch (Exception ex)
        {
            _ = await _logger.CreateLog("Logs", "MapsController", "ERROR", "System", ex.Message);
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    
}
