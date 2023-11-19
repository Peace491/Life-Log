namespace Peace.Lifelog.Logging;

public interface ILogRepo
{
    bool CreateLog(string level, string category, string? message); // sql to create a log
}