using DomainModels;
using Peace.Lifelog.DataAccess;
using Peace.Lifelog.Logging;

namespace Peace.Lifelog.LLI;

public class LLIService : ICreateLLI
{
    public async Task<Response> CreateLLI(LLI lli)
    {
        var response = new Response();

        var sql = "INSERT INTO LLI (UserHash, Title, Category, Description, Status, Visibility, Deadline, Cost, ReccurenceStatus, ReccurenceFrequency) VALUES ("
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

        // Log LLI Creation
        var logTarget = new LogTarget(createDataOnlyDAO);
        var logging = new Logging.Logging(logTarget);

        // if (response.HasError) {
        //     var errorMessage = response.ErrorMessage;
        //     logging.CreateLog("Logs", "ERROR", "Persistent Data Store", errorMessage);
        // }
        // else {
        //     logging.CreateLog("Logs", "Info", "Persistent Data Store", $"{lli.UserHash} created a LLI");
        // }

        return response;

    }
}
