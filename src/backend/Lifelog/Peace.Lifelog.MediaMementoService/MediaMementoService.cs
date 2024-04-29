using System.Diagnostics;
using System.Numerics;
using DomainModels;
using Org.BouncyCastle.Bcpg.Attr;
using Org.BouncyCastle.Security;
using Peace.Lifelog.Infrastructure;
using Peace.Lifelog.Logging;
namespace Peace.Lifelog.MediaMementoService;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Diagnostics;
using Peace.Lifelog.Security;

public class MediaMementoService : IMediaMementoService
{
    private List<string> authorizedRoles = new List<string>() { "Normal", "Admin", "Root" };
    private readonly IMediaMementoRepo _mediaMementoRepository;
    private readonly ILogging _logger;
    private readonly ILifelogAuthService _lifelogAuthService;
    private int fiftyMB = 52428800;
    private int oneGB = 1073741824;
    private Response validMediaResponse = new Response { HasError = false, ErrorMessage = null };

    public MediaMementoService(IMediaMementoRepo mediaMementoRepository, ILogging logger, ILifelogAuthService lifelogAuthService)
    {
        _mediaMementoRepository = mediaMementoRepository;
        _logger = logger;
        _lifelogAuthService = lifelogAuthService;
    }

    public async Task<Response> UploadMediaMemento(string userhash, int lliId, byte[] binary, AppPrincipal? appPrincipal)
    {
        try
        {
            if (appPrincipal == null)
            {
                return new Response { HasError = true, ErrorMessage = "AppPrincipal is null." };
            }
            if (IsUserAuthorizedForMedia(appPrincipal) == false)
            {
                return new Response { HasError = true, ErrorMessage = "User is not authorized for this operation." };
            }
            if(ValidateFileSize(binary.Length) == false)
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

    public async Task<Response> DeleteMediaMemento(int lliId, AppPrincipal? appPrincipal)
    {
        try
        {
            if (appPrincipal == null)
            {
                return new Response { HasError = true, ErrorMessage = "AppPrincipal is null." };
            }
            if (IsUserAuthorizedForMedia(appPrincipal) == false)
            {
                return new Response { HasError = true, ErrorMessage = "User is not authorized for this operation." };
            }
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

    public async Task<Response> GetAllUserImages(string userhash, AppPrincipal? appPrincipal)
    {
        try
        {
            if (appPrincipal == null)
            {
                return new Response { HasError = true, ErrorMessage = "AppPrincipal is null." };
            }
            if (IsUserAuthorizedForMedia(appPrincipal) == false)
            {
                return new Response { HasError = true, ErrorMessage = "User is not authorized for this operation." };
            }
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
    public async Task<Response> UploadMediaMementosFromCSV(string userHash, List<List<string>> CSVMatrix, AppPrincipal? appPrincipal)
    {
        try
        {
            if (appPrincipal == null)
            {
                return new Response { HasError = true, ErrorMessage = "AppPrincipal is null." };
            }
            if (IsUserAuthorizedForMedia(appPrincipal) == false)
            {
                return new Response { HasError = true, ErrorMessage = "User is not authorized for this operation." };
            }
            if (ContainsSQLInjection(CSVMatrix))
            {
                return new Response { HasError = true, ErrorMessage = "SQL injection detected." };
            }
            int totalLength = ValidateTotalLengthOfSecondColumns(CSVMatrix);
            if (totalLength == -1)
            {
                return new Response { HasError = true, ErrorMessage = "A files size is greater than 50 mb or empty." };
            }
            var allUserImages = await _mediaMementoRepository.GetAllUserImages(userHash);
            
            if (ValidateUserHasStorageSpace(allUserImages, totalLength) == false)
            {
                return new Response { HasError = true, ErrorMessage = "User does not have enough storage space, Storing this would store more than 1 gb." };
            }



            Stopwatch timer = new Stopwatch();
            timer.Start();
            var response = await _mediaMementoRepository.UploadMediaMementosFromCSV(CSVMatrix);
            timer.Stop();

            if (TimeOperation(timer) == false)
            {
                return new Response { HasError = true, ErrorMessage = "Upload media mementos from csv operation took longer than 3 seconds." };
            }

            if (response == null)
            {
                return new Response { HasError = true, ErrorMessage = "Couldn't upload media mementos from csv." };
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
    public bool ValidateFileSize(int binary)
    {
        if (binary > 0 && binary < fiftyMB)
        {
            return true;
        }
        return false;
    }
    private bool ValidateUserHasStorageSpace(Response response, int CurrentFileSize)
    {
        int total = CurrentFileSize;
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
    private bool IsUserAuthorizedForMedia(AppPrincipal appPrincipal)
    {

        return _lifelogAuthService.IsAuthorized(appPrincipal, authorizedRoles);
    }
    public static bool ContainsSQLInjection(List<List<string>> csvContent)
    {
        // Define a regular expression to detect suspicious SQL characters and keywords
        string pattern = @"('|;|--|\b(ALTER|CREATE|DELETE|DROP|EXEC|EXECUTE|INSERT|MERGE|SELECT|UPDATE|UNION|ALTER|GRANT|REVOKE)\b)";
        Regex sqlCheckRegex = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);

        foreach (var row in csvContent)
        {
            foreach (var column in row)
            {
                if (sqlCheckRegex.IsMatch(column))
                {
                    Console.WriteLine($"Potential SQL injection found in data: {column}");
                    return true; // Return true if a potential SQL injection attempt is detected
                }
            }
        }

        return false; // No SQL injection pattern found
    }
    private int ValidateTotalLengthOfSecondColumns(List<List<string>> csvContent)
    {
        int totalLength = 0;
        bool isFirstLine = true; // Flag to skip the first line

        foreach (var row in csvContent)
        {
            if (isFirstLine)
            {
                isFirstLine = false; // Skip the first line
                continue;
            }

            // Ensure the row has at least two columns
            if (row.Count >= 2 && !string.IsNullOrEmpty(row[1]))
            {
                // Get the length of the second column, trim for safety
                int lengthOfSecondColumn = row[1].Trim().Length;

                // Check if the file size is valid
                if (!ValidateFileSize(lengthOfSecondColumn))
                {
                    return -1; // Return -1 if any file size is invalid
                }

                // Add the length of the second column
                totalLength += lengthOfSecondColumn;
            }
        }
        return totalLength;
    }

    private bool ValidateOperationResponse(Response response)
    {
        // Check fields of example response objects against actual object
        if (response.HasError == validMediaResponse.HasError && response.ErrorMessage == validMediaResponse.ErrorMessage)
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
