using System.Diagnostics;
using System.Numerics;
using DomainModels;
using Org.BouncyCastle.Bcpg.Attr;
using Org.BouncyCastle.Security;
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
                return new Response { HasError = true, ErrorMessage = "File size is greater than 50 mb or empty." };
            }

            var allUserImages = await _mediaMementoRepository.GetAllUserImages(userhash);

            if(ValidateUserHasStorageSpace(allUserImages, binary.Length) == false)
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

            if (ValidateOperationResponse(response) == false)
            {
                return new Response { HasError = true, ErrorMessage = "Invalid response object." };
            }

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

            if (response.Output == null)
            {
                return new Response { HasError = true, ErrorMessage = "No media memento found to delete from." };
            }

            foreach (int result in response.Output)
            {
                if (result == 0)
                {
                    return new Response { HasError = true, ErrorMessage = "No media memento found to delete from." };
                }
            }
            

            return response;
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
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var response = await _mediaMementoRepository.GetAllUserImages(userhash);
            timer.Stop();

            if (TimeOperation(timer) == false)
            {
                return new Response { HasError = true, ErrorMessage = "Get all user images operation took longer than 3 seconds." };
            }

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
    private static bool IsJpeg(byte[] byteArray)
    {
        return byteArray.Length >= 3 &&
            byteArray[0] == 0xFF &&
            byteArray[1] == 0xD8 &&
            byteArray[2] == 0xFF;
    }

    private static bool IsPng(byte[] byteArray)
    {
        return byteArray.Length >= 8 &&
            byteArray[0] == 0x89 &&
            byteArray[1] == 0x50 &&
            byteArray[2] == 0x4E &&
            byteArray[3] == 0x47 &&
            byteArray[4] == 0x0D &&
            byteArray[5] == 0x0A &&
            byteArray[6] == 0x1A &&
            byteArray[7] == 0x0A;
    }
    public bool ValidateFileSize(byte[] binary)
    {
        if (binary.Length > 0 && binary.Length < fiftyMB)
        {
            return true;
        }
        return false;
    }
    private bool ValidateUserHasStorageSpace(Response response, int binaryLength)
    {
        int total = binaryLength;
        if (response.Output != null)
        {
            foreach (List<Object> image in response.Output)
            {
                byte[] temp = (byte[])image[0];
                total += temp.Length;
            }
        }
        if (total < oneGB)
        {
            return true;
        }
        return false;
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
