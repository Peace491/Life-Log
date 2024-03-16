using DomainModels;
using Org.BouncyCastle.Crypto.Prng;
using Peace.Lifelog.DataAccess;
using Peace.Lifelog.Logging;

namespace Peace.Lifelog.LLI;

public class LLIService : ICreateLLI, IReadLLI, IUpdateLLI, IDeleteLLI
{
    public async Task<Response> CreateLLI(string userHash, LLI lli)
    {
        var createLLIResponse = new Response();

        #region Input Validation
        if (userHash == string.Empty)
        {
            createLLIResponse.HasError = true;
            createLLIResponse.ErrorMessage = "User Hash must not be empty";
            return createLLIResponse;
        }

        if (lli.Title.Length > 50)
        {
            createLLIResponse.HasError = true;
            createLLIResponse.ErrorMessage = "LLI Title is too long";
            return createLLIResponse;
        }

        if (lli.Description is not null && lli.Description.Length > 200)
        {
            createLLIResponse.HasError = true;
            createLLIResponse.ErrorMessage = "LLI Description is too long";
            return createLLIResponse;
        }

        if (lli.Deadline != string.Empty)
        {
            var deadlineYear = Convert.ToInt32(lli.Deadline.Substring(0, 4));

            if (deadlineYear < 1960 || deadlineYear > 2100) 
            {
                createLLIResponse.HasError = true;
                createLLIResponse.ErrorMessage = "LLI Deadline is out of range";
                return createLLIResponse;
            }
        }
        
        if (lli.Cost is not null && lli.Cost < 0)
        {
            createLLIResponse.HasError = true;
            createLLIResponse.ErrorMessage = "LLI Cost must not be negative";
            return createLLIResponse;
        }
        #endregion

        #region Check for completion within the last year
        var completionDateCheckSql = "SELECT CompletionDate "
        + $"FROM LLI WHERE UserHash=\"{userHash}\" AND Title=\"{lli.Title}\"";

        var readDataOnlyDAO = new ReadDataOnlyDAO();

        var completionDateCheckResponse = await readDataOnlyDAO.ReadData(completionDateCheckSql);

        if (completionDateCheckResponse.Output != null)
        {
            foreach(List<Object> lliOutput in completionDateCheckResponse.Output)
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
                        createLLIResponse.HasError = true;
                        createLLIResponse.ErrorMessage = "LLI has been completed within the last year";
                        return createLLIResponse;
                    }
                    
                }
                
            }
        }
        #endregion

        #region Create LLI in DB
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

        var createDataOnlyDAO = new CreateDataOnlyDAO();

        createLLIResponse = await createDataOnlyDAO.CreateData(sql);

        // Get LLI Id
        var lliid = "";
        
        if (createLLIResponse.Output != null) {
            int i = 0;
            foreach(Int64 id in createLLIResponse.Output) {
                if (i == 0) lliid = id.ToString();
                break;
            }
        }
        
        // Insert Category
        var insertCategorySQL = "INSERT INTO LLICategories VALUES ";

        foreach(string category in lli.Categories!) {
            insertCategorySQL += $"({lliid}, \"{category}\"),";
        }

        // Remove trailing comma from sql
        insertCategorySQL = insertCategorySQL.Remove(insertCategorySQL.Length - 1, 1);

        var createCategoryResponse = createDataOnlyDAO.CreateData(insertCategorySQL);

        #endregion

        #region Log
        var logTarget = new LogTarget(createDataOnlyDAO);
        var logging = new Logging.Logging(logTarget);

        if (createLLIResponse.HasError) {
            createLLIResponse.ErrorMessage = "LLI fields are invalid";

            var errorMessage = createLLIResponse.ErrorMessage;
            var logResponse = logging.CreateLog("Logs", userHash, "ERROR", "Persistent Data Store", errorMessage);
        }
        else {
            var logResponse =  logging.CreateLog("Logs", userHash, "Info", "Persistent Data Store", $"{lli.UserHash} created a LLI");
        }
        #endregion

        return createLLIResponse;

    }

    public async Task<Response> GetAllLLIFromUser(string userHash)
    {
        var readLLIResponse = new Response();

        #region Input Validation
        if (userHash == string.Empty) {
            readLLIResponse.HasError = true;
            readLLIResponse.ErrorMessage = "UserHash can not be empty";
            return readLLIResponse;
        }
        #endregion

        // Read LLI Object
        var readLLISql = $"SELECT * FROM LLI WHERE userHash = \"{userHash}\"";

        var readDataOnlyDAO = new ReadDataOnlyDAO();

        readLLIResponse = await readDataOnlyDAO.ReadData(readLLISql, count:null);

        // Read LLI Categories
        var readLLICategoriesSql = "SELECT lc.lliid, lc.category "
        + "FROM LLICategories lc INNER JOIN LLI l ON lc.lliid = l.lliid "
        + $"WHERE l.UserHash = \"{userHash}\"";

        var readLLICategoriesResponse = await readDataOnlyDAO.ReadData(readLLICategoriesSql, count:null);
        #region Log
        var createDataOnlyDAO = new CreateDataOnlyDAO();
        var logTarget = new LogTarget(createDataOnlyDAO);
        var logging = new Logging.Logging(logTarget);

        if (readLLIResponse.HasError) {
            readLLIResponse.ErrorMessage = "LLI fields are invalid";

            var errorMessage = readLLIResponse.ErrorMessage;
            var logResponse = logging.CreateLog("Logs", userHash, "ERROR", "Persistent Data Store", errorMessage);
        }
        else {
            var logResponse =  logging.CreateLog("Logs", userHash, "Info", "Persistent Data Store", $"{userHash} get all LLI");
        }
        #endregion

        var lliOutput = ConvertDatabaseResponseOutputToLLIObjectList(readLLIResponse, readLLICategoriesResponse);
        
        readLLIResponse.Output = lliOutput;

        return readLLIResponse;
    }

    public Task<Response> GetSingleLLIFromUser(string userHash, LLI lli)
    {
        throw new NotImplementedException();
    }

    public async Task<Response> UpdateLLI(string userHash, LLI lli)
    {
        var updateLLIResponse = new Response();

        #region Input Validation
        if (userHash == string.Empty)
        {
            updateLLIResponse.HasError = true;
            updateLLIResponse.ErrorMessage = "User Hash must not be empty";
            return updateLLIResponse;
        }

        if (lli.Title.Length > 50)
        {
            updateLLIResponse.HasError = true;
            updateLLIResponse.ErrorMessage = "LLI Title is too long";
            return updateLLIResponse;
        }

        if (lli.Description is not null && lli.Description.Length > 200)
        {
            updateLLIResponse.HasError = true;
            updateLLIResponse.ErrorMessage = "LLI Description is too long";
            return updateLLIResponse;
        }

        if (lli.Deadline != string.Empty)
        {
            var deadlineYear = Convert.ToInt32(lli.Deadline.Substring(0, 4));

            if (deadlineYear < 1960 || deadlineYear > 2100) 
            {
                updateLLIResponse.HasError = true;
                updateLLIResponse.ErrorMessage = "LLI Deadline is out of range";
                return updateLLIResponse;
            }
        }

        if (lli.Cost is not null && lli.Cost < 0)
        {
            updateLLIResponse.HasError = true;
            updateLLIResponse.ErrorMessage = "LLI Cost must not be negative";
            return updateLLIResponse;
        }
        #endregion

        // Update LLI
        string updateLLISql = "UPDATE LLI SET "
        + (lli.Title != string.Empty && lli.Title != string.Empty ? $"Title = \"{lli.Title}\"," : "")
        + (lli.Description != null && lli.Description != string.Empty? $"Description = \"{lli.Description}\"," : "")
        + (lli.Status != null && lli.Status != string.Empty ? $"Status = \"{lli.Status}\"," : "")
        + (lli.Visibility != null && lli.Visibility != string.Empty ? $"Visibility = \"{lli.Visibility}\"," : "")
        + (lli.Deadline != string.Empty && lli.Deadline != string.Empty ? $"Deadline = \"{lli.Deadline}\"," : "")
        + (lli.Cost != null ? $"Cost = {lli.Cost}," : "")
        + (lli.Recurrence.Status != null && lli.Recurrence.Status != string.Empty ? $"RecurrenceStatus = \"{lli.Recurrence.Status}\"," : "")
        + (lli.Recurrence.Frequency != null && lli.Recurrence.Frequency != string.Empty ? $"RecurrenceFrequency = \"{lli.Recurrence.Frequency}\"," : "")
        + (lli.CompletionDate != null && lli.CompletionDate != string.Empty ? $"CompletionDate = \"{lli.CompletionDate}\"," : "");

        updateLLISql = updateLLISql.Remove(updateLLISql.Length - 1);

        updateLLISql += $" WHERE LLIId = \"{lli.LLIID}\";";

        var updateDataOnlyDAO = new UpdateDataOnlyDAO();
        updateLLIResponse = await updateDataOnlyDAO.UpdateData(updateLLISql);

        // DAO for log and update category
        var createDataOnlyDAO = new CreateDataOnlyDAO();

        // Update Category

        if (lli.Categories != null && lli.Categories.Count != 0)
        {
            // Delete all existing lli categories of that lli
            var deleteDataOnlyDAO = new DeleteDataOnlyDAO();
            var deleteOldCategoriesSql = $"DELETE FROM LLICategories WHERE lliid=\"{lli.LLIID}\"";

            var insertNewCategoriesSql = "INSERT INTO LLICategories VALUES ";

            foreach (string category in lli.Categories) {
                insertNewCategoriesSql += $"(\"{lli.LLIID}\", \"{category}\"),";
            }

            // Remove trailing comma from sql
            insertNewCategoriesSql = insertNewCategoriesSql.Remove(insertNewCategoriesSql.Length - 1, 1);

            var deleteOldCategoriesResponse = await deleteDataOnlyDAO.DeleteData(deleteOldCategoriesSql);
            var insertNewCategoriesResponse = await createDataOnlyDAO.CreateData(insertNewCategoriesSql);
            
        }
        
        // Log LLI Creation
        var logTarget = new LogTarget(createDataOnlyDAO);
        var logging = new Logging.Logging(logTarget);

        if (updateLLIResponse.HasError) {
            updateLLIResponse.ErrorMessage = "LLI fields are invalid";

            var errorMessage = updateLLIResponse.ErrorMessage;
            var logResponse = logging.CreateLog("Logs", userHash, "ERROR", "Persistent Data Store", errorMessage);
        }
        else {
            var logResponse =  logging.CreateLog("Logs", userHash, "Info", "Persistent Data Store", $"{lli.UserHash} updated LLI with id {lli.LLIID}");
        }

        return updateLLIResponse;
    }

    public async Task<Response> DeleteLLI(string userHash, LLI lli)
    {
        var response = new Response();

        #region Input Validation
        if (userHash == string.Empty) {
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

        var sql = $"DELETE FROM LLI WHERE userHash = \"{userHash}\" AND LLIId = \"{lli.LLIID}\";";

        var deleteDataOnlyDAO = new DeleteDataOnlyDAO();
        response = await deleteDataOnlyDAO.DeleteData(sql);

        #region Log
        var createDataOnlyDAO = new CreateDataOnlyDAO();
        var logTarget = new LogTarget(createDataOnlyDAO);
        var logging = new Logging.Logging(logTarget);

        if (response.HasError) {
            response.ErrorMessage = "LLI fields are invalid";

            var errorMessage = response.ErrorMessage;
            var logResponse = logging.CreateLog("Logs", userHash, "ERROR", "Persistent Data Store", errorMessage);
        }
        else {
            var logResponse =  logging.CreateLog("Logs", userHash, "Info", "Persistent Data Store", $"{userHash} delete LLI with id={lli.LLIID}");
        }
        #endregion

        return response;
    }

    // Helper
    private List<Object>? ConvertDatabaseResponseOutputToLLIObjectList(Response readLLIResponse, Response readLLICategoriesResponse)
    {
        List<Object> lliList = new List<Object>();

        if (readLLIResponse.Output == null){
            return null;
        }

        var lliCategoriesForLLI = new Dictionary<string, List<string>>();

        if (readLLICategoriesResponse.Output != null) {
            foreach (List<Object> lliCategories in readLLICategoriesResponse.Output) {
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
            
            foreach (var attribute in LLI) {
                if (attribute is null) continue;
                
                switch(index){
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
            if(lliCategoriesForLLI.ContainsKey(lli.LLIID)) {
                lli.Categories = lliCategoriesForLLI[lli.LLIID];
            }


            lliList.Add(lli);
            
        }

        return lliList;
    }

    
}
