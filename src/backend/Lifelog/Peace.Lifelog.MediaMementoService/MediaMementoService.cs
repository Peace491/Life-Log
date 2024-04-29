using DomainModels;

using Peace.Lifelog.Infrastructure;
using Peace.Lifelog.Logging;
namespace Peace.Lifelog.MediaMementoService;
using System;

using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Diagnostics;
using Peace.Lifelog.Security;
using System.Collections.ObjectModel;

public class MediaMementoService : IMediaMementoService
{
    #region Arrangement
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
    #endregion

    #region MediaMementoService Public Methods
    public async Task<Response> UploadMediaMemento(string userhash, int lliId, byte[] binary, AppPrincipal? appPrincipal)
    {
        try
        {
            Response response = new Response();

            if (appPrincipal == null)
            {
                response.ErrorMessage = "AppPrincipal is null.";
                return response;
            }

            if (IsUserAuthorizedForMedia(appPrincipal) == false)
            {
                response.ErrorMessage = "User is not authorized for this operation.";
                return response;
            }

            if(ValidateFileSize(binary.Length) == false)
            {
                response.ErrorMessage = "File size is greater than 50 mb or empty.";
                return response;
            }

            var allUserImages = await _mediaMementoRepository.GetAllUserImages(userhash);

            if(ValidateUserHasStorageSpace(allUserImages, binary.Length) == false)
            {
                response.ErrorMessage = "User does not have enough storage space, Storing this would store more than 1 gb.";
                return response;
            }

            Stopwatch timer = new Stopwatch();
            timer.Start();
            response = await _mediaMementoRepository.UploadMediaMemento(lliId, binary);
            timer.Stop();

            if (ValidateOperationResponse(response) == false)
            {
                response.ErrorMessage = "Invalid Operation Response.";
                return response;
            }

            if (TimeOperation(timer) == false)
            {
                response.ErrorMessage = "Upload media memento operation took longer than 3 seconds.";
                return response;
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
            Response response = new Response();

            if (appPrincipal == null)
            {
                response.ErrorMessage = "AppPrincipal is null.";
                return response;
            }

            if (IsUserAuthorizedForMedia(appPrincipal) == false)
            {
                response.ErrorMessage = "User is not authorized for this operation.";
                return response;
            }

            Stopwatch timer = new Stopwatch();
            timer.Start();
            response = await _mediaMementoRepository.DeleteMediaMemento(lliId);
            timer.Stop();

            if (ValidateOperationResponse(response) == false)
            {
                response.ErrorMessage = "Invalid Operation Response.";
                return response;
            }

            if (TimeOperation(timer) == false)
            {
                response.ErrorMessage = "Upload media memento operation took longer than 3 seconds.";
                return response;
            }

            if (response.Output != null)
            {
                foreach (int result in response.Output)
                {
                    if (result == 0)
                    {
                        response.ErrorMessage = "No media memento found to delete from.";
                        response.HasError = true;
                        return response;
                    }
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
            Response response = new Response();

            if (appPrincipal == null)
            {
                response.ErrorMessage = "AppPrincipal is null.";
                return response;
            }

            if (IsUserAuthorizedForMedia(appPrincipal) == false)
            {
                response.ErrorMessage = "User is not authorized for this operation.";
                return response;
            }

            Stopwatch timer = new Stopwatch();
            timer.Start();
            response = await _mediaMementoRepository.GetAllUserImages(userhash);
            timer.Stop();

            if (ValidateOperationResponse(response) == false)
            {
                response.ErrorMessage = "Invalid Operation Response.";
                return response;
            }

            if (TimeOperation(timer) == false)
            {
                response.ErrorMessage = "Get all media memento operation took longer than 3 seconds.";
                return response;
            }

            if (response.Output != null)
            {
                foreach (List<object> result in response.Output)
                {
                    if (result.Count == 0)
                    {
                        response.ErrorMessage = "No media memento found";
                        response.HasError = true;
                        return response;
                    }
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
    public async Task<Response> UploadMediaMementosFromCSV(string userHash, List<List<string>> CSVMatrix, AppPrincipal? appPrincipal)
    {
        try
        {
            Response response = new Response();

            if (appPrincipal == null)
            {
                response.ErrorMessage = "AppPrincipal is null.";
                return response;
            }

            if (IsUserAuthorizedForMedia(appPrincipal) == false)
            {
                response.ErrorMessage = "User is not authorized for this operation.";
                return response;
            }
            
            if (ContainsSQLInjection(CSVMatrix))
            {
                response.ErrorMessage = "SQL injection detected.";
                response.HasError = true;
                return response;
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
            response = await _mediaMementoRepository.UploadMediaMementosFromCSV(CSVMatrix);
            timer.Stop();

            if (ValidateOperationResponse(response) == false)
            {
                response.ErrorMessage = "Invalid Operation Response.";
                return response;
            }

            if (TimeOperation(timer) == false)
            {
                response.ErrorMessage = "Upload media memento operation took longer than 3 seconds.";
                return response;
            }

            return response;
        }
        catch (Exception ex)
        {
            _ = await _logger.CreateLog("Logs", "MediaMementoService", "ERROR", "System", ex.Message);
            return new Response { HasError = true, ErrorMessage = "An error occurred while processing your request." };
        }
    }
    #endregion

    #region MediaMementoService Private Methods

    // Helper funcs
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
        // Regular expression to check for sql injection
        string pattern = @"('|;|--|\b(ALTER|CREATE|DELETE|DROP|EXEC|EXECUTE|INSERT|MERGE|SELECT|UPDATE|UNION|ALTER|GRANT|REVOKE)\b)";
        Regex sqlCheckRegex = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);

        foreach (var row in csvContent)
        {
            foreach (var column in row)
            {
                if (sqlCheckRegex.IsMatch(column))
                {
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
    #endregion
}
