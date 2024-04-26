namespace Peace.Lifelog.MediaMementoWebService;

using Microsoft.AspNetCore.Mvc;

using System;
using System.Text.Json;
using System.Threading.Tasks;

using DomainModels;
using Peace.Lifelog.Logging;
using Peace.Lifelog.MediaMementoService;

[ApiController]
[Route("mediaMemento")]
public class MediaMementoController : ControllerBase
{
    private readonly IMediaMementoService _mediaMementoService;
    private readonly ILogging _logger;

    public MediaMementoController(IMediaMementoService mediaMementoService, ILogging logger)
    {
        _mediaMementoService = mediaMementoService;
        _logger = logger;
    }

    [HttpPost("UploadMedia")]
    public async Task<IActionResult> UploadMediaMemento([FromBody] UploadMediaMementoRequest payload)
    {
        var response = await _mediaMementoService.UploadMediaMemento(payload.UserHash, payload.LliId, payload.Binary);
        return Ok(JsonSerializer.Serialize<Response>(response));
    }

    [HttpPost("DeleteMedia")]
    public async Task<IActionResult> UploadMediaMemento([FromBody] DeleteMediaMementoRequest payload)
    {
        var response = await _mediaMementoService.DeleteMediaMemento(payload.LliId);
        return Ok(JsonSerializer.Serialize<Response>(response));
    }

}
