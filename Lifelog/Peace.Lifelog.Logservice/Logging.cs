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

        if (!validLogLevels.Contains(level))
        {
            // TODO invalid level response object return ehre
        }
        if (!validLogCategories.Contains(category))
        {
            // TODO invalid category response object return here
        }
        if (message != null) // Nested if is ugly
        {
            if (message.Length > MAXIMUMMESSAGELENGTH) 
            {
                // TODO invalid message response object return here
            }
        }  

        var logResponse = _logRepo.CreateLog(createOnlyDAO, level, category, message);

        return logResponse;
    }

}