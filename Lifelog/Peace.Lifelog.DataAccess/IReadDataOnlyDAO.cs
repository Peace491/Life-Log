namespace Peace.Lifelog.DataAccess;

using DomainModels;

using MySql.Data.MySqlClient;

public interface IReadDataOnlyDAO {
    MySqlConnection ConnectToDb();

    Response ReadData(string sql, int count = 10, int currentPage = 0);
}
