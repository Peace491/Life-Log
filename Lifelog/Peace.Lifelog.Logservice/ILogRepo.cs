namespace Peace.Lifelog.Logging;

using DomainModels;
using Peace.Lifelog.DataAccess;

public interface ILogRepo
{
    Response CreateLog(CreateDataOnlyDAO createOnlyDAO, string level, string category, string? message); // sql to create a log

    Response ReadLog(ReadDataOnlyDAO readOnlyDAO, string level, string category, string? message);

    Response DeleteLog(DeleteDataOnlyDAO deleteOnlyDAO, string level, string category, string? message); 
}