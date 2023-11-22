namespace Peace.Lifelog.Logging;

using System.Threading.Tasks;
using DomainModels;
using Peace.Lifelog.DataAccess;

public class Logging : ILogging
{
    private readonly ILogTarget _logTarget;
    public Logging(ILogTarget logTarget) => _logTarget = logTarget; // Composition Root -> Entry Point

    public async Task<Response> CreateLog(CreateDataOnlyDAO createOnlyDAO, string level, string category, string? message)
    {
        int MAXIMUM_MESSAGE_LENGTH = 65535;
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
        }
        if (!validLogCategories.Contains(category))
        {
            response.HasError = true;
            response.ErrorMessage = $"'{category}' is an invalid Log Category";
            return response;
        }
        if (message != null && message.Length > MAXIMUM_MESSAGE_LENGTH) 
        {
            response.HasError = true;
            response.ErrorMessage = $"'{message.Length}' is too long for a Log Message";
            return response;
        }  

        response = await _logTarget.WriteLog(createOnlyDAO, level, category, message);

        return response;
    }

}