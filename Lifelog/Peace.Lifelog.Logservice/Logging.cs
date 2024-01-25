namespace Peace.Lifelog.Logging;

using System.Data;
using System.Threading.Tasks;
using DomainModels;
using MySqlX.XDevAPI.Relational;
using Peace.Lifelog.DataAccess;

public class Logging : ILogging
{
    private readonly ILogTarget _logTarget;
    public Logging(ILogTarget logTarget) => _logTarget = logTarget; // Composition Root -> Entry Point
    public async Task<Response> CreateLog(string table, string userHash, string level, string category, string? message)
    {
        int MAXIMUM_MESSAGE_LENGTH = 2000;
        int HASH_LENGTH = 64;
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

        if (userHash.Length != HASH_LENGTH)
        {
            response.HasError = true;
            response.ErrorMessage = $"'{userHash.Length}' is not the correct length, indicating an invalid hash";
        }

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

        response = await _logTarget.WriteLog(table, userHash, level, category, message);

        return response;
    }

}