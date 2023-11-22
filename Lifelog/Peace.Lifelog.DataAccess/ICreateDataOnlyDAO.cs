namespace Peace.Lifelog.DataAccess;

using DomainModels;

using MySql.Data.MySqlClient;

public interface ICreateDataOnlyDAO {
    MySqlConnection ConnectToDb();

    Task<Response> CreateData(string sql);
}
