namespace Peace.Lifelog.MediaMementoWebService;

using Microsoft.AspNetCore.Mvc;

using System;
using System.Text.Json;
using System.Threading.Tasks;

using back_end;
using Peace.Lifelog.Security;
using DomainModels;
using Peace.Lifelog.Logging;
using Peace.Lifelog.MediaMementoService;

[ApiController]
[Route("mediaMemento")]
public class MediaMementoController : ControllerBase
{
    private readonly IMediaMementoService _mediaMementoService;
    private readonly ILogging _logger;
    private JWTService jwtService;

    public MediaMementoController(IMediaMementoService mediaMementoService, ILogging logger)
    {
        _mediaMementoService = mediaMementoService;
        this.jwtService = new JWTService();
        _logger = logger;
    }

    [HttpPost("UploadMedia")]
    public async Task<IActionResult> UploadMediaMemento([FromBody] UploadMediaMementoRequest payload)
    {
        try
        {
            Console.WriteLine("Payload: " + payload.LliId + " " + payload.Binary.Length);
            if (Request.Headers == null)
            {
                return StatusCode(401);
            }
            Console.WriteLine("Request.Headers: " + Request.Headers["Token"]);
            var jwtToken = JsonSerializer.Deserialize<Jwt>(Request.Headers["Token"]!);
            
            if (jwtToken == null)
            {
                return StatusCode(401);
            }

            Console.WriteLine("JWT Token: " + jwtToken.Payload.UserHash);

            var userHash = jwtToken.Payload.UserHash;

            if (userHash == null)
            {
                return StatusCode(401);
            }
            Console.WriteLine("UserHash: " + userHash);

            if (!jwtService.IsJwtValid(jwtToken))
            {
                return StatusCode(401);
            }

            var response = await _mediaMementoService.UploadMediaMemento(userHash, payload.LliId, payload.Binary);

            Console.WriteLine("Response: " + response);
    
            return Ok(response);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Exception: " + ex.Message);
            _ = await _logger.CreateLog("Logs", "MapsController", "ERROR", "System", ex.Message);
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [HttpPost("DeleteMedia")]
    public async Task<IActionResult> DeleteMediaMemento([FromBody] DeleteMediaMementoRequest payload)
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

            var response = await _mediaMementoService.DeleteMediaMemento(payload.LliId);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _ = await _logger.CreateLog("Logs", "MapsController", "ERROR", "System", ex.Message);
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [HttpPost("UploadMediaMementosFromCSV")]
    public async Task<IActionResult> UploadMediaMementosFromCSV([FromBody] UploadMediaMementosFromCSVRequest payload)
    {
        try
        {
            Console.WriteLine("Payload: " + payload.CSVMatrix.Count);
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

            Console.WriteLine("before call");
            Console.WriteLine(payload.CSVMatrix);
            var response = await _mediaMementoService.UploadMediaMementosFromCSV(userHash, payload.CSVMatrix);
            Console.WriteLine("after call", response.HasError, response.ErrorMessage);
            return Ok(response);
        }
        catch (Exception ex)
        {
            Console.WriteLine(payload.CSVMatrix);
            Console.WriteLine("Exception: " + ex.Message);
            _ = await _logger.CreateLog("Logs", "MapsController", "ERROR", "System", ex.Message);
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

}
