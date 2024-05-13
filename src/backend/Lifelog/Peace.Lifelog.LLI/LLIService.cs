using DomainModels;
using Peace.Lifelog.DataAccess;
using Peace.Lifelog.Infrastructure;
using System.Diagnostics;

namespace Peace.Lifelog.LLI;

public class LLIService : ICreateLLI, IReadLLI, IUpdateLLI, IDeleteLLI
{
    private static int WARNING_TIME_LIMIT_IN_SECOND = 3;
    private static int ERROR_TIME_LIMIT_IN_SECOND = 5;
    private static int MAX_TITLE_LENGTH_IN_CHAR = 50;
    private static int MAX_DESC_LENGTH_IN_CHAR = 200;
    private static int EARLIEST_DEADLINE_YEAR = 1960;
    private static int LATEST_DEADLINE_YEAR = 2100;
    private static int LOWEST_COST = 0;
    private static List<int> UAD_PERIOD = new List<int>() { 6, 12, 24 };
    private ILLIRepo lliRepo;
    private Logging.Logging logging;

    public LLIService(ILLIRepo lliRepo, Logging.Logging logging)
    {
        this.lliRepo = lliRepo;
        this.logging = logging;
    }

    public async Task<Response> CreateLLI(string userHash, LLI lli)
    {
        var createLLIResponse = new Response();

        #region Input Validation
        if (lli.Title == null || lli.Category1 == null || lli.Recurrence.Status == null || lli.Recurrence.Frequency == null)
        {
            createLLIResponse.HasError = true;
            createLLIResponse.ErrorMessage = "The non-nullable LLI input is null";
            var errorMessage = "The non-nullable LLI input is null";
            var logResponse = this.logging.CreateLog("Logs", userHash, "Warning", "Persistent Data Store", errorMessage);
            return createLLIResponse;
        }

        if (userHash == string.Empty)
        {
            createLLIResponse.HasError = true;
            createLLIResponse.ErrorMessage = "User Hash must not be empty";
            var errorMessage = "The LLI User Hash is invalid";
            var logResponse = this.logging.CreateLog("Logs", userHash, "Warning", "Persistent Data Store", errorMessage);
            return createLLIResponse;
        }

        if (lli.Title == string.Empty || lli.Title.Length > MAX_TITLE_LENGTH_IN_CHAR)
        {
            createLLIResponse.HasError = true;
            createLLIResponse.ErrorMessage = "The LLI title is invalid";
            var errorMessage = "The LLI title is invalid";
            var logResponse = this.logging.CreateLog("Logs", userHash, "Warning", "Persistent Data Store", errorMessage);
            return createLLIResponse;
        }

        if (
            (lli.Category1 != null && !LLICategory.IsValidCategory(lli.Category1))
            || (lli.Category2 != null && !LLICategory.IsValidCategory(lli.Category2))
            || (lli.Category3 != null && !LLICategory.IsValidCategory(lli.Category3))
        )
        {
            createLLIResponse.HasError = true;
            createLLIResponse.ErrorMessage = "LLI categories must not be null or empty";
            var errorMessage = "The LLI category is invalid";
            var logResponse = this.logging.CreateLog("Logs", userHash, "Warning", "Persistent Data Store", errorMessage);
            return createLLIResponse;
        }

        if (lli.Description is not null && lli.Description.Length > MAX_DESC_LENGTH_IN_CHAR)
        {
            createLLIResponse.HasError = true;
            createLLIResponse.ErrorMessage = "LLI Description is too long";
            var errorMessage = "The LLI description is invalid";
            var logResponse = this.logging.CreateLog("Logs", userHash, "Warning", "Persistent Data Store", errorMessage);
            return createLLIResponse;
        }

        if (!LLIStatus.IsValidStatus(lli.Status))
        {
            createLLIResponse.HasError = true;
            createLLIResponse.ErrorMessage = "LLI status is invalid";
            var errorMessage = "The LLI status is invalid";
            var logResponse = this.logging.CreateLog("Logs", userHash, "Warning", "Persistent Data Store", errorMessage);
            return createLLIResponse;
        }

        if (!LLIVisibility.IsValidVisibility(lli.Visibility))
        {
            createLLIResponse.HasError = true;
            createLLIResponse.ErrorMessage = "LLI visibility is invalid";
            var errorMessage = "The LLI visibility is invalid";
            var logResponse = this.logging.CreateLog("Logs", userHash, "Warning", "Persistent Data Store", errorMessage);
            return createLLIResponse;
        }

        if (lli.Deadline != string.Empty)
        {
            var deadlineYear = Convert.ToInt32(lli.Deadline.Substring(0, 4));

            if (deadlineYear < EARLIEST_DEADLINE_YEAR || deadlineYear > LATEST_DEADLINE_YEAR)
            {
                createLLIResponse.HasError = true;
                createLLIResponse.ErrorMessage = "LLI Deadline is out of range";
                var errorMessage = "The LLI deadline is invalid";
                var logResponse = this.logging.CreateLog("Logs", userHash, "Warning", "Persistent Data Store", errorMessage);
                return createLLIResponse;
            }
        }

        if (lli.Cost is not null && lli.Cost < LOWEST_COST)
        {
            createLLIResponse.HasError = true;
            createLLIResponse.ErrorMessage = "LLI Cost must not be negative";
            var errorMessage = "The LLI cost is invalid";
            var logResponse = this.logging.CreateLog("Logs", userHash, "Warning", "Persistent Data Store", errorMessage);
            return createLLIResponse;
        }

        if (!LLIRecurrenceStatus.IsValidRecurrenceStatus(lli.Recurrence.Status))
        {
            createLLIResponse.HasError = true;
            createLLIResponse.ErrorMessage = "LLI recurrence status is invalid";
            var errorMessage = "The LLI recurrence status is invalid";
            var logResponse = this.logging.CreateLog("Logs", userHash, "Warning", "Persistent Data Store", errorMessage);
            return createLLIResponse;
        }

        if (lli.Recurrence.Status == LLIRecurrenceStatus.On && !LLIRecurrenceFrequency.IsValidRecurrenceFrequency(lli.Recurrence.Frequency))
        {
            createLLIResponse.HasError = true;
            createLLIResponse.ErrorMessage = "LLI recurrence frequency is invalid";
            var errorMessage = "The LLI recurrence frequency is invalid";
            var logResponse = this.logging.CreateLog("Logs", userHash, "Warning", "Persistent Data Store", errorMessage);
            return createLLIResponse;
        }
        #endregion

        #region Check for completion within the last year
        var completionWithinLastYearCheckResponse = await checkIfLLIHasBeenCompletedWithinTheLastYear(userHash, lli);
        if (completionWithinLastYearCheckResponse.HasError)
        {
            createLLIResponse.ErrorMessage = "LLI has been completed within the last year";
            var errorMessage = "The LLI title and category matches the title and category of a LLI that the user has completed within one year prior to the current date.";
            var logResponse = this.logging.CreateLog("Logs", userHash, "Warning", "Persistent Data Store", errorMessage);
            return createLLIResponse;
        }
        #endregion

        #region Create LLI in DB
        lli.CompletionDate = "";
        if (lli.Status == LLIStatus.Completed)
        {
            lli.CompletionDate = DateTime.Today.ToString("yyyy-MM-dd");
        }

        var timer = new Stopwatch();
        timer.Start();
        
        LLIDB lliDB = new LLIDB() 
        {
            LLIID = lli.LLIID,
            Title = lli.Title,
            Description = lli.Description!,
            Category1 = lli.Category1,
            Category2 = lli.Category2,
            Category3 = lli.Category3,
            Status = lli.Status,
            Visibility = lli.Visibility,
            Deadline = lli.Deadline,
            Cost = lli.Cost,
            RecurrenceStatus = lli.Recurrence.Status,
            RecurrenceFrequency = lli.Recurrence.Frequency,
            CompletionDate = lli.CompletionDate
        };

        createLLIResponse = await lliRepo.CreateLLI(userHash ,lliDB);

        // Get LLI Id
        var lliid = "";

        if (createLLIResponse.Output != null)
        {
            int i = 0;
            foreach (Int64 id in createLLIResponse.Output)
            {
                if (i == 0) lliid = id.ToString();
                break;
            }
        }
        else if (createLLIResponse.Output == null && createLLIResponse.HasError == false)
        {
            createLLIResponse.HasError = true;
            createLLIResponse.ErrorMessage = "The user does not exist";
            var errorMessage = "The was an error with LLI creation";
            var logResponse = this.logging.CreateLog("Logs", userHash, "Warning", "Persistent Data Store", errorMessage);
            return createLLIResponse;
        }
        else
        {
            createLLIResponse.ErrorMessage = "LLI fields are invalid";
            var errorMessage = "LLI fields are invalid";
            var logResponse = this.logging.CreateLog("Logs", userHash, "ERROR", "Persistent Data Store", errorMessage);
            return createLLIResponse;
        }


        timer.Stop();

        #endregion

        #region Log


        var successLogResponse = this.logging.CreateLog("Logs", userHash, "Info", "Persistent Data Store", "The LLI is successfully created");


        if (timer.Elapsed.TotalSeconds > WARNING_TIME_LIMIT_IN_SECOND && timer.Elapsed.TotalSeconds < ERROR_TIME_LIMIT_IN_SECOND)
        {
            var errorMessage = "Operation exceeded time frame";
            var logResponse = this.logging.CreateLog("Logs", userHash, "Warning", "Persistent Data Store", errorMessage);
        }
        else if (timer.Elapsed.TotalSeconds > ERROR_TIME_LIMIT_IN_SECOND)
        {
            var errorMessage = "Operation took too long";
            var logResponse = this.logging.CreateLog("Logs", userHash, "ERROR", "Persistent Data Store", errorMessage);
        }
        #endregion

        return createLLIResponse;

    }

    public async Task<Response> GetAllLLIFromUser(string userHash)
    {
        var readLLIResponse = new Response();

        #region Input Validation
        if (userHash == string.Empty)
        {
            readLLIResponse.HasError = true;
            readLLIResponse.ErrorMessage = "UserHash can not be empty";
            return readLLIResponse;
        }
        #endregion

        #region Update LLI According to recurrence status

        var updateResponse = await this.lliRepo.UpdateLLIRecurrenceStatus(userHash);

        #endregion
        #region Read LLI In DB
        var timer = new Stopwatch();
        timer.Start();

        readLLIResponse = await this.lliRepo.ReadAllLLI(userHash);

        if (readLLIResponse.Output == null)
        {
            var message = "There is no lli associated with the account";
            var logResponse = this.logging.CreateLog("Logs", userHash, "ERROR", "Persistent Data Store", message);
            return readLLIResponse;
        }

        timer.Stop();
        #endregion

        #region Log

        if (readLLIResponse.HasError)
        {
            readLLIResponse.ErrorMessage = "LLI fields are invalid";

            var errorMessage = "The LLI Management view failed to load";
            var logResponse = this.logging.CreateLog("Logs", userHash, "ERROR", "Persistent Data Store", errorMessage);
        }
        else
        {
            var logResponse = this.logging.CreateLog("Logs", userHash, "Info", "Persistent Data Store", $"{userHash} get all LLI");
        }

        if (timer.Elapsed.TotalSeconds > WARNING_TIME_LIMIT_IN_SECOND && timer.Elapsed.TotalSeconds < ERROR_TIME_LIMIT_IN_SECOND)
        {
            var errorMessage = "Operation exceeded time frame";
            var logResponse = this.logging.CreateLog("Logs", userHash, "Warning", "Persistent Data Store", errorMessage);
        }
        else if (timer.Elapsed.TotalSeconds > ERROR_TIME_LIMIT_IN_SECOND)
        {
            var errorMessage = "Operation took too long";
            var logResponse = this.logging.CreateLog("Logs", userHash, "ERROR", "Persistent Data Store", errorMessage);
        }
        #endregion

        var lliOutput = ConvertDatabaseResponseOutputToLLIObjectList(readLLIResponse);

        readLLIResponse.Output = lliOutput;

        return readLLIResponse;
    }

    public async Task<Response> GetMostCommonLLICategory(int period)
    {
        var response = await lliRepo.ReadMostCommonLLICategory();

        return response;
    }

    public async Task<Response> GetLLICount()
    {
        var response = new Response();
        response.HasError = false;
        try
        {
            var output = new List<Object>();

            foreach (int period in UAD_PERIOD)
            {
                var readResponse = await lliRepo.ReadNumberOfLLI(period);

                if (readResponse.Output != null)
                {
                    output.Add(readResponse.Output);
                }
            }

            response.Output = output;

        }
        catch (Exception error)
        {
            response.HasError = true;
            response.ErrorMessage = error.Message;
            response.Output = null;
            return response;
        }

        return response;
    }

    public async Task<Response> GetMostExpensiveLLI()
    {
        var response = new Response();
        response.HasError = false;
        try
        {
            var output = new List<Object>();

            foreach (int period in UAD_PERIOD)
            {
                var readResponse = await lliRepo.ReadMostExpensiveLLI(period);

                if (readResponse.Output != null)
                {
                    output.Add(readResponse.Output);
                }
            }

            response.Output = output;

        }
        catch (Exception error)
        {
            response.HasError = true;
            response.ErrorMessage = error.Message;
            response.Output = null;
            return response;
        }

        return response;
    }

    public async Task<Response> UpdateLLI(string userHash, LLI lli)
    {
        var updateLLIResponse = new Response();

        #region Input Validation
        if (userHash == string.Empty)
        {
            updateLLIResponse.HasError = true;
            updateLLIResponse.ErrorMessage = "User Hash must not be empty";
            var errorMessage = "The LLI User Hash is invalid";
            var logResponse = this.logging.CreateLog("Logs", userHash, "Warning", "Persistent Data Store", errorMessage);
            return updateLLIResponse;
        }

        if (lli.Title.Length > MAX_TITLE_LENGTH_IN_CHAR)
        {
            updateLLIResponse.HasError = true;
            updateLLIResponse.ErrorMessage = "The LLI title is invalid";
            var errorMessage = "The LLI title is invalid";
            var logResponse = this.logging.CreateLog("Logs", userHash, "Warning", "Persistent Data Store", errorMessage);
            return updateLLIResponse;
        }

        if (
            (lli.Category1 != null && !LLICategory.IsValidCategory(lli.Category1))
            || (lli.Category2 != null && !LLICategory.IsValidCategory(lli.Category2))
            || (lli.Category3 != null && !LLICategory.IsValidCategory(lli.Category3))
        )
        {
            updateLLIResponse.HasError = true;
            updateLLIResponse.ErrorMessage = "LLI categories must not be null or empty";
            var errorMessage = "The LLI category is invalid";
            var logResponse = this.logging.CreateLog("Logs", userHash, "Warning", "Persistent Data Store", errorMessage);
            return updateLLIResponse;
        }
        if (lli.Description is not null && lli.Description.Length > MAX_DESC_LENGTH_IN_CHAR)
        {
            updateLLIResponse.HasError = true;
            updateLLIResponse.ErrorMessage = "LLI Description is too long";
            var errorMessage = "The LLI description is invalid";
            var logResponse = this.logging.CreateLog("Logs", userHash, "Warning", "Persistent Data Store", errorMessage);
            return updateLLIResponse;
        }

        if (lli.Status != null && !LLIStatus.IsValidStatus(lli.Status))
        {
            updateLLIResponse.HasError = true;
            updateLLIResponse.ErrorMessage = "LLI status is invalid";
            var errorMessage = "The LLI status is invalid";
            var logResponse = this.logging.CreateLog("Logs", userHash, "Warning", "Persistent Data Store", errorMessage);
            return updateLLIResponse;
        }

        if (lli.Visibility != null && !LLIVisibility.IsValidVisibility(lli.Visibility))
        {
            updateLLIResponse.HasError = true;
            updateLLIResponse.ErrorMessage = "LLI visibility is invalid";
            var errorMessage = "The LLI visibility is invalid";
            var logResponse = this.logging.CreateLog("Logs", userHash, "Warning", "Persistent Data Store", errorMessage);
            return updateLLIResponse;
        }

        if (lli.Deadline != string.Empty)
        {
            var deadlineYear = Convert.ToInt32(lli.Deadline.Substring(0, 4));

            if (deadlineYear < EARLIEST_DEADLINE_YEAR || deadlineYear > LATEST_DEADLINE_YEAR)
            {
                updateLLIResponse.HasError = true;
                updateLLIResponse.ErrorMessage = "LLI Deadline is out of range";
                var errorMessage = "The LLI deadline is invalid";
                var logResponse = this.logging.CreateLog("Logs", userHash, "Warning", "Persistent Data Store", errorMessage);
                return updateLLIResponse;
            }
        }

        if (lli.Cost is not null && lli.Cost < LOWEST_COST)
        {
            updateLLIResponse.HasError = true;
            updateLLIResponse.ErrorMessage = "LLI Cost must not be negative";
            var errorMessage = "The LLI cost is invalid";
            var logResponse = this.logging.CreateLog("Logs", userHash, "Warning", "Persistent Data Store", errorMessage);
            return updateLLIResponse;
        }

        if (lli.Recurrence != null && !LLIRecurrenceStatus.IsValidRecurrenceStatus(lli.Recurrence.Status))
        {
            updateLLIResponse.HasError = true;
            updateLLIResponse.ErrorMessage = "LLI recurrence status is invalid";
            var errorMessage = "The LLI recurrence status is invalid";
            var logResponse = this.logging.CreateLog("Logs", userHash, "Warning", "Persistent Data Store", errorMessage);
            return updateLLIResponse;
        }

        if (lli.Recurrence != null && lli.Recurrence.Status == LLIRecurrenceStatus.On && !LLIRecurrenceFrequency.IsValidRecurrenceFrequency(lli.Recurrence.Frequency))
        {
            updateLLIResponse.HasError = true;
            updateLLIResponse.ErrorMessage = "LLI recurrence frequency is invalid";
            var errorMessage = "The LLI recurrence frequency is invalid";
            var logResponse = this.logging.CreateLog("Logs", userHash, "Warning", "Persistent Data Store", errorMessage);
            return updateLLIResponse;
        }
        #endregion

        #region Check for completion within the last year
        if (lli.Title != null && lli.Title != string.Empty)
        {
            var completionWithinLastYearCheckResponse = await checkIfLLIHasBeenCompletedWithinTheLastYear(userHash, lli);
            if (completionWithinLastYearCheckResponse.HasError)
            {
                updateLLIResponse.ErrorMessage = "LLI has been completed within the last year";
                var errorMessage = "The LLI title and category matches the title and category of a LLI that the user has completed within one year prior to the current date.";
                var logResponse = this.logging.CreateLog("Logs", userHash, "Warning", "Persistent Data Store", errorMessage);
                return updateLLIResponse;
            }
        }

        #endregion

        #region Update LLI in DB
        // Update LLI
        var timer = new Stopwatch();
        timer.Start();

        LLIDB lliDB = new LLIDB() 
        {
            LLIID = lli.LLIID,
            Title = lli.Title!,
            Description = lli.Description!,
            Category1 = lli.Category1,
            Category2 = lli.Category2,
            Category3 = lli.Category3,
            Status = lli.Status!,
            Visibility = lli.Visibility!,
            Deadline = lli.Deadline,
            Cost = lli.Cost,
            RecurrenceStatus = lli.Recurrence!.Status,
            RecurrenceFrequency = lli.Recurrence.Frequency,
            CompletionDate = lli.CompletionDate
        };

        updateLLIResponse = await lliRepo.UpdateLLI(userHash, lliDB);

        if (updateLLIResponse.HasError)
        {
            updateLLIResponse.ErrorMessage = "LLI fields are invalid";

            var errorMessage = updateLLIResponse.ErrorMessage;
            var logResponse = this.logging.CreateLog("Logs", userHash, "ERROR", "Persistent Data Store", errorMessage);
        }

        timer.Stop();
        #endregion

        #region Log
        // Log LLI Creation

        var successlogResponse = this.logging.CreateLog("Logs", userHash, "Info", "Persistent Data Store", "The LLI is successfully edited");


        if (timer.Elapsed.TotalSeconds > WARNING_TIME_LIMIT_IN_SECOND && timer.Elapsed.TotalSeconds < ERROR_TIME_LIMIT_IN_SECOND)
        {
            var errorMessage = "Operation exceeded time frame";
            var logResponse = this.logging.CreateLog("Logs", userHash, "Warning", "Persistent Data Store", errorMessage);
        }
        else if (timer.Elapsed.TotalSeconds > ERROR_TIME_LIMIT_IN_SECOND)
        {
            var errorMessage = "Operation took too long";
            var logResponse = this.logging.CreateLog("Logs", userHash, "ERROR", "Persistent Data Store", errorMessage);
        }
        #endregion

        return updateLLIResponse;
    }

    public async Task<Response> DeleteLLI(string userHash, LLI lli)
    {
        var response = new Response();

        #region Input Validation
        if (userHash == string.Empty)
        {
            response.HasError = true;
            response.ErrorMessage = "UserHash can not be empty";
            return response;
        }

        if (lli.LLIID == string.Empty || lli.LLIID is null)
        {
            response.HasError = true;
            response.ErrorMessage = "LLIId can not be empty";
            return response;
        }
        #endregion

        #region Delete LLI in DB
        var timer = new Stopwatch();
        timer.Start();

        var deleteResponse = await lliRepo.DeleteLLI(userHash, lli.LLIID);
        timer.Stop();

        if (deleteResponse.Output != null)
        {
            foreach (int rowsAffected in deleteResponse.Output)
            {
                if (rowsAffected == 0)
                {
                    response.HasError = true;
                    response.ErrorMessage = "Failed to delete LLI";
                    return response;
                }
            }
        }

        response = deleteResponse;
        #endregion

        #region Log

        if (response.HasError)
        {
            response.ErrorMessage = "LLI fields are invalid";

            var errorMessage = "The LLI failed to be deleted from the persistent data store";
            var logResponse = this.logging.CreateLog("Logs", userHash, "ERROR", "Persistent Data Store", errorMessage);
        }
        else
        {
            var logResponse = this.logging.CreateLog("Logs", userHash, "Info", "Persistent Data Store", $"The LLI is successfully deleted");
        }

        if (timer.Elapsed.TotalSeconds > WARNING_TIME_LIMIT_IN_SECOND && timer.Elapsed.TotalSeconds < ERROR_TIME_LIMIT_IN_SECOND)
        {
            var errorMessage = "Operation exceeded time frame";
            var logResponse = this.logging.CreateLog("Logs", userHash, "Warning", "Persistent Data Store", errorMessage);
        }
        else if (timer.Elapsed.TotalSeconds > ERROR_TIME_LIMIT_IN_SECOND)
        {
            var errorMessage = "Operation took too long";
            var logResponse = this.logging.CreateLog("Logs", userHash, "ERROR", "Persistent Data Store", errorMessage);
        }
        #endregion

        return response;
    }

    // Helper
    private async Task<Response> checkIfLLIHasBeenCompletedWithinTheLastYear(string userHash, LLI lli)
    {
        var response = new Response();

        var completionDateCheckResponse = await lliRepo.ReadLLICompletionStatus(userHash, lli.Title, lli.LLIID);

        if (completionDateCheckResponse.Output != null)
        {
            foreach (List<Object> lliOutput in completionDateCheckResponse.Output)
            {
                foreach (var attribute in lliOutput)
                {
                    if (attribute is null || attribute.ToString() == string.Empty)
                    {
                        continue;
                    }

                    var lliCompletionDate = attribute.ToString()!.Substring(0, attribute.ToString()!.IndexOf(' '));

                    if (lliCompletionDate == string.Empty)
                    {
                        continue;
                    }

                    var completionDate = DateTime.ParseExact(lliCompletionDate, "M/d/yyyy", null);
                    var today = DateTime.Now;
                    var oneYearAgo = today.AddYears(-1);

                    if (completionDate > oneYearAgo) // LLI can not be created if it has been completed in the last year
                    {
                        response.HasError = true;
                        return response;
                    }

                }

            }
        }
        response.HasError = false;
        return response;
    }
    private List<Object>? ConvertDatabaseResponseOutputToLLIObjectList(Response readLLIResponse)
    {
        List<Object> lliList = new List<Object>();

        if (readLLIResponse.Output == null)
        {
            return null;
        }

        foreach (List<Object> LLI in readLLIResponse.Output)
        {

            var lli = new LLI();

            int index = 0;

            foreach (var attribute in LLI)
            {
                if (attribute is null) continue;

                switch (index)
                {
                    case 0:
                        lli.LLIID = attribute.ToString() ?? "";
                        break;
                    case 1:
                        lli.UserHash = attribute.ToString() ?? "";
                        break;
                    case 2:
                        lli.Title = attribute.ToString() ?? "";
                        break;
                    case 3:
                        lli.Description = attribute.ToString() ?? "";
                        break;
                    case 4:
                        lli.Status = attribute.ToString() ?? "";
                        break;
                    case 5:
                        lli.Visibility = attribute.ToString() ?? "";
                        break;
                    case 6:
                        lli.Deadline = attribute.ToString() ?? "";
                        break;
                    case 7:
                        lli.Cost = Convert.ToInt32(attribute);
                        break;
                    case 8:
                        lli.Recurrence.Status = attribute.ToString() ?? "";
                        break;
                    case 9:
                        lli.Recurrence.Frequency = attribute.ToString() ?? "";
                        break;
                    case 10:
                        lli.CreationDate = attribute.ToString() ?? "";
                        break;
                    case 11:
                        lli.CompletionDate = attribute.ToString() ?? "";
                        break;
                    case 12:
                        lli.Category1 = attribute.ToString() ?? "";
                        break;
                    case 13:
                        lli.Category2 = attribute.ToString() ?? "";
                        break;
                    case 14:
                        lli.Category3 = attribute.ToString() ?? "";
                        break;
                    case 15:
                        lli.MediaMemento = attribute as byte[] ?? null;

                        break;
                    default:
                        break;
                }
                index++;

            }

            lliList.Add(lli);

        }

        return lliList;
    }


}
