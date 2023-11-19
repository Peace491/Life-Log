namespace Peace.Lifelog.Logging;

public class Logging : ILogging
{
    private readonly ILogRepo _logRepo;
    public Logging(ILogRepo logRepo) => _logRepo = logRepo; // Composition Root -> Entry Point

    // Not the best approach
    // Error
    public bool Log(string level, string category, string? message)
    {
        _logRepo.CreateLog(level, category, message);
        return true;
    }
}