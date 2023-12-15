namespace Peace.Lifelog.Logging;

using DomainModels;

public interface ILogging
{
    Task<Response> CreateLog(string table, string level, string category, string? message);
}