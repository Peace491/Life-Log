using System.Reflection.Metadata.Ecma335;
using DomainModels;
using Peace.Lifelog.DataAccess;
using Peace.Lifelog.Logging;

namespace Peace.Lifelog.LLI;

public class LLIService : ICreateLLI, IReadLLI, IUpdateLLI, IDeleteLLI
{
    public async Task<Response> CreateLLI(string userHash, LLI lli)
    {
        var response = new Response();

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

        var sql = "INSERT INTO LLI (UserHash, Title, Category, Description, Status, Visibility, Deadline, Cost, RecurrenceStatus, RecurrenceFrequency) VALUES ("
        + $"\"{userHash}\", "
        + $"\"{lli.Title}\", "
        + $"\"{lli.Category}\", "
        + $"\"{lli.Description}\", "
        + $"\"{lli.Status}\", "
        + $"\"{lli.Visibility}\", "
        + $"\"{lli.Deadline}\", "
        + $"{lli.Cost}, "
        + $"\"{lli.Recurrence.Status}\", "
        + $"\"{lli.Recurrence.Frequency}\""
        + ");";

        // Create LLI in DB
        var createDataOnlyDAO = new CreateDataOnlyDAO();

        response = await createDataOnlyDAO.CreateData(sql);

        // // Log LLI Creation
        // var logTarget = new LogTarget(createDataOnlyDAO);
        // var logging = new Logging.Logging(logTarget);

        // if (response.HasError) {
        //     var errorMessage = response.ErrorMessage;
        //     logging.CreateLog("Logs", "ERROR", "Persistent Data Store", errorMessage);
        // }
        // else {
        //     logging.CreateLog("Logs", "Info", "Persistent Data Store", $"{lli.UserHash} created a LLI");
        // }

        return response;

    }

    public async Task<Response> GetAllLLIFromUser(string userHash, int pageNumber = 0)
    {
        var response = new Response();

        if (userHash == string.Empty) {
            response.HasError = true;
            response.ErrorMessage = "Must provide a user hash";
            return response;
        }

        var sql = $"SELECT * FROM LLI WHERE userHash = \"{userHash}\"";

        var readDataOnlyDAO = new ReadDataOnlyDAO();

        if (pageNumber == 0)
        {
            response = await readDataOnlyDAO.ReadData(sql);
        }
        else {
            response = await readDataOnlyDAO.ReadData(sql, 10, pageNumber);
        }

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

        string sql = "UPDATE LLI SET "
        + (lli.Title != string.Empty && lli.Title != string.Empty ? $"Title = \"{lli.Title}\"," : "")
        + (lli.Category != null && lli.Category != string.Empty ? $"Category = \"{lli.Category}\"," : "")
        + (lli.Description != null && lli.Description != string.Empty? $"Description = \"{lli.Description}\"," : "")
        + (lli.Status != null && lli.Status != string.Empty ? $"Status = \"{lli.Status}\"," : "")
        + (lli.Visibility != null && lli.Visibility != string.Empty ? $"Visibility = \"{lli.Visibility}\"," : "")
        + (lli.Deadline != string.Empty && lli.Deadline != string.Empty ? $"Deadline = \"{lli.Deadline}\"," : "")
        + (lli.Cost != null ? $"Cost = {lli.Cost}," : "")
        + (lli.Recurrence.Status != null && lli.Recurrence.Status != string.Empty ? $"RecurrenceStatus = \"{lli.Recurrence.Status}\"," : "")
        + (lli.Recurrence.Frequency != null && lli.Recurrence.Frequency != string.Empty ? $"RecurrenceFrequency = \"{lli.Recurrence.Frequency}\"," : "");

        sql = sql.Remove(sql.Length - 1);

        sql += $" WHERE LLIId = \"{lli.LLIID}\";";

        var updateDataOnlyDAO = new UpdateDataOnlyDAO();
        response = await updateDataOnlyDAO.UpdateData(sql);

        return response;
    }

    public async Task<Response> DeleteLLI(string userHash, LLI lli)
    {
        var response = new Response();

        if (userHash == string.Empty) {
            response.HasError = true;
            response.ErrorMessage = "Must provide a user hash";
            return response;
        }

        var sql = $"DELETE FROM LLI WHERE userHash = \"{userHash}\" AND LLIId = \"{lli.LLIID}\";";

        var deleteDataOnlyDAO = new DeleteDataOnlyDAO();
        response = await deleteDataOnlyDAO.DeleteData(sql);

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
