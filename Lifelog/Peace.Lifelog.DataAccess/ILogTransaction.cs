namespace Peace.Lifelog.DataAccess;

using DomainModels;

public interface ILogTransaction : ISqlDAO
{
    Task<Response> CreateDataAccessTransactionLog(string level, string message);

    Task<Response> DeleteDataAccessTransactionLog(long logId);    
}
