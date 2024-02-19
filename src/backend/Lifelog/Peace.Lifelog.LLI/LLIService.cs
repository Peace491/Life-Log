using DomainModels;
using Peace.Lifelog.DataAccess;
using Peace.Lifelog.Logging;

namespace Peace.Lifelog.LLI;

public class LLIService : ICreateLLI, IReadLLI, IUpdateLLI, IDeleteLLI
{
    public async Task<Response> CreateLLI(LLI lli)
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

        var sql = "INSERT INTO LLI (UserHash, Title, Category, Description, Status, Visibility, Deadline, Cost, RecurrenceStatus, RecurrenceFrequency) VALUES ("
        + $"\"{lli.UserHash}\", "
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

    public Task<Response> GetAllLLIFromUser(string userHash, int pageNumber = 0)
    {
        throw new NotImplementedException();
    }

    public Task<Response> GetSingleLLIFromUser(string userHash, LLI lli)
    {
        throw new NotImplementedException();
    }

    public Task<Response> UpdateLLI(string userHash, LLI oldLLI, LLI newLLI)
    {
        throw new NotImplementedException();
    }

    public Task<Response> DeleteLLI(string userHash, LLI lli)
    {
        throw new NotImplementedException();
    }
}
