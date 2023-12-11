namespace Peace.Lifelog.Logging;

using DomainModels;
using Peace.Lifelog.DataAccess;

public interface ILogging
{
    Task<Response> CreateLog(string table, string level, string category, string? message);
}