namespace Peace.Lifelog.DataAccess;

using DomainModels;

using MySql.Data.MySqlClient;

public interface ILogTransaction
{
    MySqlConnection ConnectToDb();

    Response CreateDataAccessTransactionLog(string level, string message);

    Response DeleteDataAccessTransactionLog(long logId);    
}
