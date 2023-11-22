namespace Peace.Lifelog.Logging;

using DomainModels;
using Peace.Lifelog.DataAccess;

public class Logging : ILogging
{
    private readonly ILogRepo _logRepo;
    public Logging(ILogRepo logRepo) => _logRepo = logRepo; // Composition Root -> Entry Point

    // Not the best approach
    // Error
    public Response CreateLog(CreateDataOnlyDAO createOnlyDAO, string level, string category, string? message)
    {
        // TODO: Business Logic Here
        int MAXIMUMMESSAGELENGTH = 65535;
        HashSet<string> validLogLevels = new HashSet<string>
        {
            "Info", 
            "Debug", 
            "Warning", 
            "ERROR"
        };

        HashSet<string> validLogCategories = new HashSet<string>
        {
            "View", 
            "Business", 
            "Server", 
            "Data",
            "Persistent Data Store"
        };
        var response = new Response();

        if (!validLogLevels.Contains(level))
        {
            response.HasError = true;
            response.ErrorMessage = $"'{level} is an invalid Log Level";
            return response;
            // TODO invalid level response object return ehre
        }
        if (!validLogCategories.Contains(category))
        {
            response.HasError = true;
            response.ErrorMessage = $"'{category}' is an invalid Log Category";
            return response;
            // TODO invalid category response object return here
        }
        if (message != null) 
        {
            if (message.Length > MAXIMUMMESSAGELENGTH) 
            {
                response.HasError = true;
                response.ErrorMessage = $"'{message.Length}' is too long for a Log Message";
                return response;
                // TODO invalid message response object return here
            }
        }  

        var logResponse = _logRepo.CreateLog(createOnlyDAO, level, category, message);

        return logResponse;
    }

}