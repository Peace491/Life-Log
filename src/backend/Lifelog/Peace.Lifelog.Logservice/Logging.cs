namespace Peace.Lifelog.Logging;

using System.Data;
using System.Threading.Tasks;
using DomainModels;
using Peace.Lifelog.DataAccess;

public class Logging : ILogging
{
    private readonly ILogTarget _logTarget;
    public Logging(ILogTarget logTarget) => _logTarget = logTarget; // Composition Root -> Entry Point
    public async Task<Response> CreateLog(string table, string userHash, string level, string category, string? message)
    {
        int MAXIMUM_MESSAGE_LENGTH = 2000;
        int HASH_LENGTH = 44;
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

    public async Task<Response> ReadLoginLogsCount(string table, string type, int period)
    {
        var response = new Response();

        HashSet<string> LOGIN_LOG_TYPES = new HashSet<string>
        {
            "Success",
            "Failure",
        };

        if (table == null || table == string.Empty)
        {
            response.HasError = true;
            response.ErrorMessage = "Table must not be null or empty";
            return response;
        }

        if  (!LOGIN_LOG_TYPES.Contains(type)) {
            response.HasError = true;
            response.ErrorMessage = "The type must be either 'Success' or 'Failure'";
            return response;
        }

        response = await _logTarget.ReadLoginLogsCount(table, type, period);

        return response;

    }

    public async Task<Response> ReadRegLogsCount(string table, string type, int period)
    {
        var response = new Response();

        HashSet<string> REG_LOG_TYPES = new HashSet<string>
        {
            "Success",
            "Failure",
        };

        if (table == null || table == string.Empty)
        {
            response.HasError = true;
            response.ErrorMessage = "Table must not be null or empty";
            return response;
        }

        if  (!REG_LOG_TYPES.Contains(type)) {
            response.HasError = true;
            response.ErrorMessage = "The type must be either 'Success' or 'Failure'";
            return response;
        }

        response = await _logTarget.ReadRegLogsCount(table, type, period);

        return response;
    }

    public async Task<Response> ReadTopNLongestPageVisit(string table, int numOfPage, int periodInMonth)
    {
        var response = new Response();

        if (table == null || table == string.Empty)
        {
            response.HasError = true;
            response.ErrorMessage = "Table must not be null or empty";
            return response;
        }

        if (numOfPage < 1)
        {
            response.HasError = true;
            response.ErrorMessage = "Must select at least 1 log";
            return response;
        }

        if (periodInMonth < 1)
        {
            response.HasError = true;
            response.ErrorMessage = "Must select logs from at least the past 1 month";
            return response;
        }

        response = await _logTarget.ReadTopNLongestPageVisit(table, numOfPage, periodInMonth);
        return response;
    }

    public async Task<Response> ReadTopNMostVisitedPage(string table, int numOfPage, int periodInMonth)
    {
        var response = new Response();

        if (table == null || table == string.Empty)
        {
            response.HasError = true;
            response.ErrorMessage = "Table must not be null or empty";
            return response;
        }

        if (numOfPage < 1)
        {
            response.HasError = true;
            response.ErrorMessage = "Must select at least 1 log";
            return response;
        }

        if (periodInMonth < 1)
        {
            response.HasError = true;
            response.ErrorMessage = "Must select logs from at least the past 1 month";
            return response;
        }

        response = await _logTarget.ReadTopNMostVisitedPage(table, numOfPage, periodInMonth);
        return response;
    }
}