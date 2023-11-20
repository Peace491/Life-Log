namespace Peace.Lifelog.Logging;

using DomainModels;

public class Logging : ILogging
{
    private readonly ILogRepo _logRepo;
    public Logging(ILogRepo logRepo) => _logRepo = logRepo; // Composition Root -> Entry Point

    // Not the best approach
    // Error
    public Response CreateLog(string level, string category, string? message)
    {
        // TODO: Business Logic Here

        var logResponse = _logRepo.CreateLog(level, category, message);

        return logResponse;
    }

    public Response ReadLog(string level, string category, string? message)
    {
        // TODO: Business Login Here

        var readResponse = _logRepo.ReadLog(level, category, message);

        return readResponse;
    }
}