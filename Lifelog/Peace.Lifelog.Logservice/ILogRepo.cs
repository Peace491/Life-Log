namespace Peace.Lifelog.Logging;

using DomainModels;

public interface ILogRepo
{
    Response CreateLog(string level, string category, string? message); // sql to create a log

    Response ReadLog(string level, string category, string? message);

    Response DeleteLog(string level, string category, string? message); 
}