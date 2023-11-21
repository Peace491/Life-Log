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
        // level is debug, succes, erorr etc
        //message is str

        var logResponse = _logRepo.CreateLog(createOnlyDAO, level, category, message);

        return logResponse;
    }

    public Response ReadLog(ReadDataOnlyDAO readOnlyDAO, string level, string category, string? message)
    {
        // TODO: Business Login Here

        var readResponse = _logRepo.ReadLog(readOnlyDAO, level, category, message);

        return readResponse;
    }
}