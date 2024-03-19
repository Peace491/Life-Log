using System.Diagnostics;
using DomainModels;
using Peace.Lifelog.DataAccess;
using Peace.Lifelog.Logging;

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
    private CreateDataOnlyDAO createDataOnlyDAO;
    private ReadDataOnlyDAO readDataOnlyDAO;
    private UpdateDataOnlyDAO updateDataOnlyDAO;
    private DeleteDataOnlyDAO deleteDataOnlyDAO;
    private Logging.Logging logging;

    public LLIService(CreateDataOnlyDAO createDataOnlyDAO, ReadDataOnlyDAO readDataOnlyDAO, UpdateDataOnlyDAO updateDataOnlyDAO, DeleteDataOnlyDAO deleteDataOnlyDAO, Logging.Logging logging) {
        this.createDataOnlyDAO = createDataOnlyDAO;
        this.readDataOnlyDAO = readDataOnlyDAO;
        this.updateDataOnlyDAO = updateDataOnlyDAO;
        this.deleteDataOnlyDAO = deleteDataOnlyDAO;
        this.logging = logging;
    }

    public async Task<Response> CreateLLI(string userHash, LLI lli)
    {
        var createLLIResponse = new Response();

        #region Input Validation
        if (lli.Title == null || lli.Categories == null || lli.Recurrence.Status == null || lli.Recurrence.Frequency == null)
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

        if (lli.Categories is null || lli.Categories.Count == 0)
        {
            createLLIResponse.HasError = true;
            createLLIResponse.ErrorMessage = "LLI categories must not be null or empty";
            var errorMessage = "The LLI category is invalid";
            var logResponse = this.logging.CreateLog("Logs", userHash, "Warning", "Persistent Data Store", errorMessage);
            return createLLIResponse;
        }

        foreach (var categories in lli.Categories)
        {
            if (!LLICategory.IsValidCategory(categories))
            {
                createLLIResponse.HasError = true;
                createLLIResponse.ErrorMessage = "A LLI category is invalid";
                var errorMessage = "The LLI category is invalid";
                var logResponse = this.logging.CreateLog("Logs", userHash, "Warning", "Persistent Data Store", errorMessage);
                return createLLIResponse;
            }
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
        var timer = new Stopwatch();
        timer.Start();

        var sql = "INSERT INTO LLI (UserHash, Title, Description, Status, Visibility, Deadline, Cost, RecurrenceStatus, RecurrenceFrequency, CreationDate, CompletionDate) VALUES ("
        + $"\"{userHash}\", "
        + $"\"{lli.Title}\", "
        + $"\"{lli.Description}\", "
        + $"\"{lli.Status}\", "
        + $"\"{lli.Visibility}\", "
        + $"\"{lli.Deadline}\", "
        + $"{lli.Cost}, "
        + $"\"{lli.Recurrence.Status}\", "
        + $"\"{lli.Recurrence.Frequency}\", "
        + $"\"{DateTime.Today.ToString("yyyy-MM-dd")}\", "
        + $"null"
        + ");";

        createLLIResponse = await this.createDataOnlyDAO.CreateData(sql);

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

        // Insert Category
        var insertCategorySQL = "INSERT INTO LLICategories (LLIId, Category) VALUES ";

        foreach (string category in lli.Categories!)
        {
            insertCategorySQL += $"({lliid}, \"{category}\"),";
        }

        // Remove trailing comma from sql
        insertCategorySQL = insertCategorySQL.Remove(insertCategorySQL.Length - 1, 1);

        var createCategoryResponse = this.createDataOnlyDAO.CreateData(insertCategorySQL);

        timer.Stop();

        #endregion

        #region Log


        if (createLLIResponse.HasError)
        {
            createLLIResponse.ErrorMessage = "LLI fields are invalid";

            var errorMessage = "LLI’s failed to save into the persistent data store.";
            var logResponse = this.logging.CreateLog("Logs", userHash, "ERROR", "Persistent Data Store", errorMessage);
        }
        else
        {
            var logResponse = this.logging.CreateLog("Logs", userHash, "Info", "Persistent Data Store", "The LLI is successfully created");
        }

        if (timer.Elapsed.TotalSeconds > WARNING_TIME_LIMIT_IN_SECOND && timer.Elapsed.TotalSeconds < ERROR_TIME_LIMIT_IN_SECOND)
        {
            var errorMessage = "Operation exceeded time frame";
            var logResponse = this.logging.CreateLog("Logs", userHash, "Warning", "Persistent Data Store", errorMessage);
        }
        else if (timer.Elapsed.TotalSeconds < ERROR_TIME_LIMIT_IN_SECOND)
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

        var updateLLISql = "UPDATE LLI "
        + "SET Status = 'Active', CompletionDate = NULL "
        + $"WHERE (RecurrenceFrequency = 'Weekly' AND UserHash=\"{userHash}\" AND CompletionDate IS NOT NULL AND DATE_ADD(CompletionDate, INTERVAL 1 WEEK) <= CURDATE()) "
        + $"OR (RecurrenceFrequency = 'Monthly' AND UserHash=\"{userHash}\" AND CompletionDate IS NOT NULL AND DATE_ADD(CompletionDate, INTERVAL 1 MONTH) <= CURDATE()) "
        + $"OR (RecurrenceFrequency = 'Yearly' AND UserHash=\"{userHash}\" AND CompletionDate IS NOT NULL AND DATE_ADD(CompletionDate, INTERVAL 1 YEAR) <= CURDATE())";

        var updateResponse = this.updateDataOnlyDAO.UpdateData(updateLLISql);

        #endregion
        #region Read LLI In DB
        var timer = new Stopwatch();
        timer.Start();
        var readLLISql = $"SELECT * FROM LLI WHERE userHash = \"{userHash}\"";

        readLLIResponse = await this.readDataOnlyDAO.ReadData(readLLISql, count: null);

        // Read LLI Categories
        var readLLICategoriesSql = "SELECT lc.lliid, lc.category "
        + "FROM LLICategories lc INNER JOIN LLI l ON lc.lliid = l.lliid "
        + $"WHERE l.UserHash = \"{userHash}\"";

        var readLLICategoriesResponse = await this.readDataOnlyDAO.ReadData(readLLICategoriesSql, count: null);

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
        else if (timer.Elapsed.TotalSeconds < ERROR_TIME_LIMIT_IN_SECOND)
        {
            var errorMessage = "Operation took too long";
            var logResponse = this.logging.CreateLog("Logs", userHash, "ERROR", "Persistent Data Store", errorMessage);
        }
        #endregion

        var lliOutput = ConvertDatabaseResponseOutputToLLIObjectList(readLLIResponse, readLLICategoriesResponse);

        readLLIResponse.Output = lliOutput;

        return readLLIResponse;
    }

    public async Task<Response> UpdateLLI(string userHash, LLI lli)
    {
        var updateLLIResponse = new Response();

        #region Input Validation
        // if (lli.Title == null || lli.Categories == null || lli.Recurrence.Status == null || lli.Recurrence.Frequency == null) {
        //     updateLLIResponse.HasError = true;
        //     updateLLIResponse.ErrorMessage = "The non-nullable LLI input is null";
        //     var errorMessage = "The non-nullable LLI input is null";
        //     var logResponse = this.logging.CreateLog("Logs", userHash, "Warning", "Persistent Data Store", errorMessage);
        //     return updateLLIResponse;
        // }

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

        if (!(lli.Categories is null) && !(lli.Categories.Count == 0))
        {
            foreach (var categories in lli.Categories)
            {
                if (!LLICategory.IsValidCategory(categories))
                {
                    updateLLIResponse.HasError = true;
                    updateLLIResponse.ErrorMessage = "A LLI Category is invalid";
                    var errorMessage = "The LLI category is invalid";
                    var logResponse = this.logging.CreateLog("Logs", userHash, "Warning", "Persistent Data Store", errorMessage);
                    return updateLLIResponse;
                }
            }
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

        if (lli.Status == LLIStatus.Completed) {
            lli.CompletionDate = DateTime.Today.ToString("yyyy-MM-dd");
        }

        string updateLLISql = "UPDATE LLI SET "
        + (lli.Title != string.Empty && lli.Title != string.Empty ? $"Title = \"{lli.Title}\"," : "")
        + (lli.Description != null && lli.Description != string.Empty ? $"Description = \"{lli.Description}\"," : "")
        + (lli.Status != null && lli.Status != string.Empty ? $"Status = \"{lli.Status}\"," : "")
        + (lli.Visibility != null && lli.Visibility != string.Empty ? $"Visibility = \"{lli.Visibility}\"," : "")
        + (lli.Deadline != string.Empty && lli.Deadline != string.Empty ? $"Deadline = \"{lli.Deadline}\"," : "")
        + (lli.Cost != null ? $"Cost = {lli.Cost}," : "")
        + (lli.Recurrence!.Status != null && lli.Recurrence.Status != string.Empty ? $"RecurrenceStatus = \"{lli.Recurrence.Status}\"," : "")
        + (lli.Recurrence.Frequency != null && lli.Recurrence.Frequency != string.Empty ? $"RecurrenceFrequency = \"{lli.Recurrence.Frequency}\"," : "")
        + (lli.CompletionDate != null && lli.CompletionDate != string.Empty ? $"CompletionDate = \"{lli.CompletionDate}\"," : "");

        updateLLISql = updateLLISql.Remove(updateLLISql.Length - 1);

        updateLLISql += $" WHERE LLIId = \"{lli.LLIID}\";";

        updateLLIResponse = await this.updateDataOnlyDAO.UpdateData(updateLLISql);

        // Update Category

        if (lli.Categories != null && lli.Categories.Count != 0)
        {
            // Delete all existing lli categories of that lli
            var deleteOldCategoriesSql = $"DELETE FROM LLICategories WHERE lliid=\"{lli.LLIID}\"";

            var insertNewCategoriesSql = "INSERT INTO LLICategories (LLIId, Category) VALUES ";

            foreach (string category in lli.Categories)
            {
                insertNewCategoriesSql += $"(\"{lli.LLIID}\", \"{category}\"),";
            }

            // Remove trailing comma from sql
            insertNewCategoriesSql = insertNewCategoriesSql.Remove(insertNewCategoriesSql.Length - 1, 1);

            var deleteOldCategoriesResponse = await this.deleteDataOnlyDAO.DeleteData(deleteOldCategoriesSql);
            var insertNewCategoriesResponse = await this.createDataOnlyDAO.CreateData(insertNewCategoriesSql);

        }

        timer.Stop();
        #endregion 

        #region Log
        // Log LLI Creation

        if (updateLLIResponse.HasError)
        {
            updateLLIResponse.ErrorMessage = "LLI fields are invalid";

            var errorMessage = updateLLIResponse.ErrorMessage;
            var logResponse = this.logging.CreateLog("Logs", userHash, "ERROR", "Persistent Data Store", errorMessage);
        }
        else
        {
            var logResponse = this.logging.CreateLog("Logs", userHash, "Info", "Persistent Data Store", "The LLI is successfully edited");
        }

        if (timer.Elapsed.TotalSeconds > WARNING_TIME_LIMIT_IN_SECOND && timer.Elapsed.TotalSeconds < ERROR_TIME_LIMIT_IN_SECOND)
        {
            var errorMessage = "Operation exceeded time frame";
            var logResponse = this.logging.CreateLog("Logs", userHash, "Warning", "Persistent Data Store", errorMessage);
        }
        else if (timer.Elapsed.TotalSeconds < ERROR_TIME_LIMIT_IN_SECOND)
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

        var sql = $"DELETE FROM LLI WHERE userHash = \"{userHash}\" AND LLIId = \"{lli.LLIID}\";";

        response = await this.deleteDataOnlyDAO.DeleteData(sql);
        timer.Stop();
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
        else if (timer.Elapsed.TotalSeconds < ERROR_TIME_LIMIT_IN_SECOND)
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

        var completionDateCheckSql = "";

        if (lli.LLIID != string.Empty) {
            completionDateCheckSql = "SELECT CompletionDate "
            + $"FROM LLI WHERE UserHash=\"{userHash}\" AND Title=\"{lli.Title}\" AND LLIId != {lli.LLIID}";
        } else {
            completionDateCheckSql = "SELECT CompletionDate "
            + $"FROM LLI WHERE UserHash=\"{userHash}\" AND Title=\"{lli.Title}\"";
        }
        

        var completionDateCheckResponse = await this.readDataOnlyDAO.ReadData(completionDateCheckSql);

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
    private List<Object>? ConvertDatabaseResponseOutputToLLIObjectList(Response readLLIResponse, Response readLLICategoriesResponse)
    {
        List<Object> lliList = new List<Object>();

        if (readLLIResponse.Output == null)
        {
            return null;
        }

        var lliCategoriesForLLI = new Dictionary<string, List<string>>();

        if (readLLICategoriesResponse.Output != null)
        {
            foreach (List<Object> lliCategories in readLLICategoriesResponse.Output)
            {
                string lliid = lliCategories[0].ToString()!;
                string lliCategory = lliCategories[1].ToString()!;

                if (!lliCategoriesForLLI.ContainsKey(lliid))
                {
                    // Key doesn't exist, create a new list and add the item to it
                    var newList = new List<string>();
                    newList.Add(lliCategory);
                    lliCategoriesForLLI.Add(lliid, newList);
                }
                else
                {
                    // Key exists, retrieve the list associated with that key and add the item to it
                    lliCategoriesForLLI[lliid].Add(lliCategory);
                }

            }
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
                    default:
                        break;
                }
                index++;

            }

            // Set Title
            if (lliCategoriesForLLI.ContainsKey(lli.LLIID))
            {
                lli.Categories = lliCategoriesForLLI[lli.LLIID];
            }


            lliList.Add(lli);

        }

        return lliList;
    }


}
