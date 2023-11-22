namespace Peace.Lifelog.DataAccess;

using DomainModels;

using MySql.Data.MySqlClient;

public interface ILogTransaction
{
    MySqlConnection ConnectToDb();

    Task<Response> CreateDataAccessTransactionLog(string level, string message);

    Task<Response> DeleteDataAccessTransactionLog(long logId);    
}
