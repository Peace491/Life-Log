namespace Peace.Lifelog.Logging;

using DomainModels;
using Peace.Lifelog.DataAccess;

public interface ILogging
{
    Response CreateLog(CreateDataOnlyDAO createOnlyDAO, string level, string category, string? message);

    Response ReadLog(ReadDataOnlyDAO readOnlyDAO,string level, string category, string? message); // Consider changing name to ReadLogs
}