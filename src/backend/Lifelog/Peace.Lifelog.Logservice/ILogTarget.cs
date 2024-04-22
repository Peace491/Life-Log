namespace Peace.Lifelog.Logging;

using DomainModels;

public interface ILogTarget
{
    Task<Response> WriteLog(string table, string userHash, string level, string category, string? message);

    Task<Response> ReadTopNMostVisitedPage(string table, int numOfPage, int period);

    Task<Response> ReadLoginLogsCount(string table, string type);

    Task<Response> ReadRegLogsCount(string table, string type);
    Task<Response> ReadTopNLongestPageVisit(string table, int numOfPage, int period);
}