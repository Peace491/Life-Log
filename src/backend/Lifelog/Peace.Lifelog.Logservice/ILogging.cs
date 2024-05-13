namespace Peace.Lifelog.Logging;

using DomainModels;

public interface ILogging
{
    Task<Response> CreateLog(string table, string userHash, string level, string category, string? message);

    Task<Response> ReadMostVisitedPage(string table, int numOfPage);

    Task<Response> ReadLoginLogsCount(string table, string type);
    Task<Response> ReadRegLogsCount(string table, string type);
    Task<Response> ReadLongestPageVisit(string table, int numOfPage);
}