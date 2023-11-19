namespace Peace.Lifelog.Logging;

public interface ILogging
{
    bool Log(string level, string category, string? message);
}