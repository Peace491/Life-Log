namespace Peace.Lifelog.Logging;

using DomainModels;
using Peace.Lifelog.DataAccess;

public interface ILogging
{
    Response CreateLog(CreateDataOnlyDAO createOnlyDAO, string level, string category, string? message);
}