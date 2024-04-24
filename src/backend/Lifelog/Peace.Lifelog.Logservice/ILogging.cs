namespace Peace.Lifelog.Logging;

using DomainModels;

public interface ILogging
{
    Task<Response> CreateLog(string table, string userHash, string level, string category, string? message);

    Task<Response> ReadTopNMostVisitedPage(string table, int numOfPage, int period);

    Task<Response> ReadLoginLogsCount(string table, string type, int period);
    Task<Response> ReadRegLogsCount(string table, string type, int period);
    Task<Response> ReadTopNLongestPageVisit(string table, int numOfPage, int period);
}