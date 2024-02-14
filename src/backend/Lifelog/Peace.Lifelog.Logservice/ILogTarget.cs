namespace Peace.Lifelog.Logging;

using DomainModels;

public interface ILogTarget
{
    Task<Response> WriteLog(string table, string userHash, string level, string category, string? message);
}