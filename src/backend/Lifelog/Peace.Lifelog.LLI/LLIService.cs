using System.Reflection.Metadata.Ecma335;
using System.Runtime.Serialization;
using DomainModels;
using Peace.Lifelog.DataAccess;
using Peace.Lifelog.Logging;

namespace Peace.Lifelog.LLI;

public class LLIService : ICreateLLI, IReadLLI, IUpdateLLI, IDeleteLLI
{
    public async Task<Response> CreateLLI(string userHash, LLI lli)
    {
        var response = new Response();

        #region Input Validation
        if (userHash == string.Empty)
        {
            response.HasError = true;
            response.ErrorMessage = "User Hash must not be empty";
            return response;
        }

        if (lli.Title.Length > 50)
        {
            response.HasError = true;
            response.ErrorMessage = "LLI Title is too long";
            return response;
        }

        if (lli.Description is not null && lli.Description.Length > 200)
        {
            response.HasError = true;
            response.ErrorMessage = "LLI Description is too long";
            return response;
        }

        if (lli.Deadline != string.Empty)
        {
            var deadlineYear = Convert.ToInt32(lli.Deadline.Substring(0, 4));

            if (deadlineYear < 1960 || deadlineYear > 2100) 
            {
                response.HasError = true;
                response.ErrorMessage = "LLI Deadline is out of range";
                return response;
            }
        }
        
        if (lli.Cost is not null && lli.Cost < 0)
        {
            response.HasError = true;
            response.ErrorMessage = "LLI Cost must not be negative";
            return response;
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

                    var lliCompletionDate = attribute.ToString()!.Substring(0, 9);

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
                        response.ErrorMessage = "LLI has been completed within the last year";
                        return response;
                    }
                    
                }
                
            }
        }
        #endregion

        #region Create LLI in DB
        var sql = "INSERT INTO LLI (UserHash, Title, Category, Description, Status, Visibility, Deadline, Cost, RecurrenceStatus, RecurrenceFrequency, CreationDate, CompletionDate) VALUES ("
        + $"\"{userHash}\", "
        + $"\"{lli.Title}\", "
        + $"\"{lli.Category}\", "
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

        response = await createDataOnlyDAO.CreateData(sql);
        #endregion

        #region Log
        var logTarget = new LogTarget(createDataOnlyDAO);
        var logging = new Logging.Logging(logTarget);

        if (response.HasError) {
            response.ErrorMessage = "LLI fields are invalid";

            var errorMessage = response.ErrorMessage;
            var logResponse = logging.CreateLog("Logs", userHash, "ERROR", "Persistent Data Store", errorMessage);
        }
        else {
            var logResponse =  logging.CreateLog("Logs", userHash, "Info", "Persistent Data Store", $"{lli.UserHash} created a LLI");
        }
        #endregion

        return response;

    }

    public async Task<Response> GetAllLLIFromUser(string userHash)
    {
        var response = new Response();

        #region Input Validation
        if (userHash == string.Empty) {
            response.HasError = true;
            response.ErrorMessage = "UserHash can not be empty";
            return response;
        }
        #endregion

        var sql = $"SELECT * FROM LLI WHERE userHash = \"{userHash}\"";

        var readDataOnlyDAO = new ReadDataOnlyDAO();

        response = await readDataOnlyDAO.ReadData(sql);

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
            var logResponse =  logging.CreateLog("Logs", userHash, "Info", "Persistent Data Store", $"{userHash} get all LLI");
        }
        #endregion

        var lliOutput = ConvertDatabaseResponseOutputToLLIObjectList(response);

        response.Output = lliOutput;

        return response;

        
    }

    public Task<Response> GetSingleLLIFromUser(string userHash, LLI lli)
    {
        throw new NotImplementedException();
    }

    public async Task<Response> UpdateLLI(string userHash, LLI lli)
    {
        var response = new Response();

        #region Input Validation
        if (userHash == string.Empty)
        {
            response.HasError = true;
            response.ErrorMessage = "User Hash must not be empty";
            return response;
        }

        if (lli.Title.Length > 50)
        {
            response.HasError = true;
            response.ErrorMessage = "LLI Title is too long";
            return response;
        }

        if (lli.Description is not null && lli.Description.Length > 200)
        {
            response.HasError = true;
            response.ErrorMessage = "LLI Description is too long";
            return response;
        }

        if (lli.Deadline != string.Empty)
        {
            var deadlineYear = Convert.ToInt32(lli.Deadline.Substring(0, 4));

            if (deadlineYear < 1960 || deadlineYear > 2100) 
            {
                response.HasError = true;
                response.ErrorMessage = "LLI Deadline is out of range";
                return response;
            }
        }

        if (lli.Cost is not null && lli.Cost < 0)
        {
            response.HasError = true;
            response.ErrorMessage = "LLI Cost must not be negative";
            return response;
        }
        #endregion

        string sql = "UPDATE LLI SET "
        + (lli.Title != string.Empty && lli.Title != string.Empty ? $"Title = \"{lli.Title}\"," : "")
        + (lli.Category != null && lli.Category != string.Empty ? $"Category = \"{lli.Category}\"," : "")
        + (lli.Description != null && lli.Description != string.Empty? $"Description = \"{lli.Description}\"," : "")
        + (lli.Status != null && lli.Status != string.Empty ? $"Status = \"{lli.Status}\"," : "")
        + (lli.Visibility != null && lli.Visibility != string.Empty ? $"Visibility = \"{lli.Visibility}\"," : "")
        + (lli.Deadline != string.Empty && lli.Deadline != string.Empty ? $"Deadline = \"{lli.Deadline}\"," : "")
        + (lli.Cost != null ? $"Cost = {lli.Cost}," : "")
        + (lli.Recurrence.Status != null && lli.Recurrence.Status != string.Empty ? $"RecurrenceStatus = \"{lli.Recurrence.Status}\"," : "")
        + (lli.Recurrence.Frequency != null && lli.Recurrence.Frequency != string.Empty ? $"RecurrenceFrequency = \"{lli.Recurrence.Frequency}\"," : "")
        + (lli.CompletionDate != null && lli.CompletionDate != string.Empty ? $"CompletionDate = \"{lli.CompletionDate}\"," : "");

        sql = sql.Remove(sql.Length - 1);

        sql += $" WHERE LLIId = \"{lli.LLIID}\";";

        var updateDataOnlyDAO = new UpdateDataOnlyDAO();
        response = await updateDataOnlyDAO.UpdateData(sql);

        // Log LLI Creation
        var createDataOnlyDAO = new CreateDataOnlyDAO();
        var logTarget = new LogTarget(createDataOnlyDAO);
        var logging = new Logging.Logging(logTarget);

        if (response.HasError) {
            response.ErrorMessage = "LLI fields are invalid";

            var errorMessage = response.ErrorMessage;
            var logResponse = logging.CreateLog("Logs", userHash, "ERROR", "Persistent Data Store", errorMessage);
        }
        else {
            var logResponse =  logging.CreateLog("Logs", userHash, "Info", "Persistent Data Store", $"{lli.UserHash} updated LLI with id {lli.LLIID}");
        }

        return response;
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
    private List<Object>? ConvertDatabaseResponseOutputToLLIObjectList(Response response)
    {
        List<Object> lliList = new List<Object>();

        if (response.Output == null){
            return null;
        }

        foreach (List<Object> LLI in response.Output)
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
                        lli.Category = attribute.ToString() ?? "";
                        break;
                    case 4:
                        lli.Description = attribute.ToString() ?? "";
                        break;
                    case 5:
                        lli.Status = attribute.ToString() ?? "";
                        break;
                    case 6:
                        lli.Visibility = attribute.ToString() ?? "";
                        break;
                    case 7:
                        lli.Deadline = attribute.ToString() ?? "";
                        break;
                    case 8:
                        lli.Cost = Convert.ToInt32(attribute);
                        break;
                    case 9:
                        lli.Recurrence.Status = attribute.ToString() ?? "";
                        break;
                    case 10:
                        lli.Recurrence.Frequency = attribute.ToString() ?? "";
                        break;
                    case 11:
                        lli.CreationDate = attribute.ToString() ?? "";
                        break;
                    case 12:
                        lli.CompletionDate = attribute.ToString() ?? "";
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
