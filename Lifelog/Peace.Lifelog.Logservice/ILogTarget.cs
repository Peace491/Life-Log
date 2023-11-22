namespace Peace.Lifelog.Logging;

using DomainModels;
using Peace.Lifelog.DataAccess;

public interface ILogTarget
{
    Task<Response> WriteLog(CreateDataOnlyDAO createOnlyDAO, string level, string category, string? message);
}