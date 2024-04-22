using System.Diagnostics;
using DomainModels;
using Peace.Lifelog.Infrastructure;
using Peace.Lifelog.Logging;
namespace Peace.Lifelog.MediaMementoService;

public class MediaMementoService : IMediaMementoService
{
    private readonly IMediaMementoRepo _mediaMementoRepository;
    private readonly ILogging _logger;
    private string fiftyMB = "52428800";
    private string oneGB = "1073741824";
    private Response validDeleteMediaResponse = new Response { HasError = false, ErrorMessage = null };
    private Response validUploadMediaResponse = new Response { HasError = false, ErrorMessage = null };

    public MediaMementoService(IMediaMementoRepo mediaMementoRepository, ILogging logger)
    {
        _mediaMementoRepository = mediaMementoRepository;
        _logger = logger;
    }

    public async Task<Response> UploadMediaMemento(string userhash, int lliId, byte[] binary)
    {
        try
        {
            var response = await _mediaMementoRepository.UploadMediaMemento(lliId, binary);

            if (response == null)
            {
                return new Response { HasError = true, ErrorMessage = "Couldn't upload media memento." };
            }

            if (response.HasError)
            {
                return new Response { HasError = true, ErrorMessage = "An error occurred while processing your request." };
            }

            return new Response { HasError = false, ErrorMessage = "Media memento uploaded successfully." };
        }
        catch (Exception ex)
        {
            _ = await _logger.CreateLog("Logs", "MediaMementoService", "ERROR", "System", ex.Message);
            return new Response { HasError = true, ErrorMessage = "An error occurred while processing your request." };
        }
    }

    public async Task<Response> DeleteMediaMemento(int lliId)
    {
        try
        {
            var response = await _mediaMementoRepository.DeleteMediaMemento(lliId);

            if (response == null)
            {
                return new Response { HasError = true, ErrorMessage = "Couldn't delete media memento." };
            }

            if (response.HasError)
            {
                return new Response { HasError = true, ErrorMessage = "An error occurred while processing your request." };
            }

            return new Response { HasError = false, ErrorMessage = "Media memento deleted successfully." };
        }
        catch (Exception ex)
        {
            _ = await _logger.CreateLog("Logs", "MediaMementoService", "ERROR", "System", ex.Message);
            return new Response { HasError = true, ErrorMessage = "An error occurred while processing your request." };
        }
    }

    // Helper funcs
    public bool ValidateFileSize(string binary)
    {
        if (binary.Length > 0 && binary.Length < 52428800)
        {
            return true;
        }
        return false;
    }
    private bool ValidateUserHasStorageSpace(string userhash)
    {
        throw new NotImplementedException();
    }
    private bool ValidateUploadMediaResponse(Response response)
    {
        // Check fields of example response objects against actual object
        throw new NotImplementedException();
    }
    private bool ValidateDeleteMediaResponse(Response response)
    {
        throw new NotImplementedException();
    }
    private bool TimeOperation(Stopwatch timer)
    {
        if (timer.Elapsed.TotalSeconds < 3)
        {
            return true;
        }
        return false;
    }
}
