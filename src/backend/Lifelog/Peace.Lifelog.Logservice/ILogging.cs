namespace Peace.Lifelog.Logging;

using DomainModels;

public interface ILogging
{
    Task<Response> CreateLog(string table, string userHash, string level, string category, string? message);
}