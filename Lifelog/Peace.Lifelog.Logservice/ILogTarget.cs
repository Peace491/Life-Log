namespace Peace.Lifelog.Logging;

using DomainModels;

public interface ILogTarget
{
    Task<Response> WriteLog(string level, string category, string? message);
}