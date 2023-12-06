namespace Peace.Lifelog.Logging;

using DomainModels;

public interface ILogTarget
{
    Task<Response> WriteLog(string table, string level, string category, string? message);
}