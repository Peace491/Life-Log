namespace Peace.Lifelog.Logging;

using DomainModels;

public interface ILogging
{
    Response CreateLog(string level, string category, string? message);

    Response ReadLog(string level, string category, string? message); // Consider changing name to ReadLogs
}