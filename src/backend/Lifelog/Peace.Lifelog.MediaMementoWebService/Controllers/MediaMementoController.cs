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
    #region Constructor
    private readonly IMediaMementoService _mediaMementoService;
    private readonly ILogging _logger;
    private JWTService jwtService;

    public MediaMementoController(IMediaMementoService mediaMementoService, ILogging logger)
    {
        _mediaMementoService = mediaMementoService;
        this.jwtService = new JWTService();
        _logger = logger;
    }
    #endregion

    #region API Entry Points
    [HttpPost("UploadMedia")]
    public async Task<IActionResult> UploadMediaMemento([FromBody] UploadMediaMementoRequest payload)
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

            var response = await _mediaMementoService.UploadMediaMemento(userHash, payload.LliId, payload.Binary, payload.AppPrincipal);

            if (response.HasError)
            {
                throw new Exception(response.ErrorMessage);
            }
    
            return Ok(response);
        }
        catch (Exception ex)
        {
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

            var response = await _mediaMementoService.DeleteMediaMemento(payload.LliId, payload.AppPrincipal);

            if (response.HasError)
            {
                return StatusCode(500, "An error occurred while processing your request.");
            }

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

            var response = await _mediaMementoService.UploadMediaMementosFromCSV(userHash, payload.CSVMatrix, payload.AppPrincipal);

            if (response.HasError)
            {
                return StatusCode(500, "An error occurred while processing your request.");
            }

            return Ok(response);
        }
        catch (Exception ex)
        {
            _ = await _logger.CreateLog("Logs", "MapsController", "ERROR", "System", ex.Message);
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }
    #endregion
}
