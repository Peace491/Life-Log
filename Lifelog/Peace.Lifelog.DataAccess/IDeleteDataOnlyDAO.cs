namespace Peace.Lifelog.DataAccess;

using DomainModels;

using MySql.Data.MySqlClient;

public interface IDeleteDataOnlyDAO {
    MySqlConnection ConnectToDb();
    Response DeleteData(string sql);
}