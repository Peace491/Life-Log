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
        //message is string
        var validLevels = ["Info", "Debug", "Warning", "ERROR"]

        // for each valid log leve, check that the inputed level is one of the valid ones.
        // repeat this for category, and message.
        // message has the character limit of mysql text, of 65,535. message is nullable

        if(level != )

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