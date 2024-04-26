using System.Diagnostics;
using System.Numerics;
using DomainModels;
using Org.BouncyCastle.Bcpg.Attr;
using Peace.Lifelog.Infrastructure;
using Peace.Lifelog.Logging;
namespace Peace.Lifelog.MediaMementoService;

public class MediaMementoService : IMediaMementoService
{
    private readonly IMediaMementoRepo _mediaMementoRepository;
    private readonly ILogging _logger;
    private int fiftyMB = 52428800;
    private int oneGB = 1073741824;
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
            if(ValidateFileSize(binary) == false)
            {
                return new Response { HasError = true, ErrorMessage = "File size is greater than 50 mb." };
            }

            var allUserImages = await _mediaMementoRepository.GetAllUserImages(userhash);

            if(ValidateUserHasStorageSpace(allUserImages) == false)
            {
                return new Response { HasError = true, ErrorMessage = "User does not have enough storage space, Storing this would store more than 1 gb." };
            }

            var response = await _mediaMementoRepository.UploadMediaMemento(lliId, binary);

            if (response == null)
            {
                return new Response { HasError = true, ErrorMessage = "Couldn't upload media memento." };
            }

            if (response.HasError)
            {
                return new Response { HasError = true, ErrorMessage = "An error occurred while processing your request." };
            }
            
            return response;
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
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var response = await _mediaMementoRepository.DeleteMediaMemento(lliId);
            timer.Stop();

            if (TimeOperation(timer) == false)
            {
                return new Response { HasError = true, ErrorMessage = "Delete operation took longer than 3 seconds." };
            }

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

    public async Task<Response> GetAllUserImages(string userhash)
    {
        try
        {
            var response = await _mediaMementoRepository.GetAllUserImages(userhash);

            if (response == null)
            {
                return new Response { HasError = true, ErrorMessage = "Couldn't get all user images." };
            }

            if (response.HasError)
            {
                return new Response { HasError = true, ErrorMessage = "An error occurred while processing your request." };
            }

            return response;
        }
        catch (Exception ex)
        {
            _ = await _logger.CreateLog("Logs", "MediaMementoService", "ERROR", "System", ex.Message);
            return new Response { HasError = true, ErrorMessage = "An error occurred while processing your request." };
        }
    }

    // Helper funcs
    public bool ValidateFileSize(byte[] binary)
    {
        if (binary.Length > 0 && binary.Length < fiftyMB)
        {
            return true;
        }
        return false;
    }
    private bool ValidateUserHasStorageSpace(Response response)
    {
        int total = 0;
        foreach (List<Object> image in response.Output)
        {
            byte[] temp = (byte[])image[0];
            total += temp.Length;
            // total += image.Length;
        }
        if (total < oneGB)
        {
            return true;
        }
        return false;

        throw new NotImplementedException();
    }
    private bool ValidateOperationResponse(Response response)
    {
        // Check fields of example response objects against actual object
        if (response.HasError == validDeleteMediaResponse.HasError && response.ErrorMessage == validDeleteMediaResponse.ErrorMessage)
        {
            return true;
        }
        return false;
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
