namespace Peace.Lifelog.DataAccess;

using DomainModels;

using MySql.Data.MySqlClient;

public interface IUpdateDataOnlyDAO {
    MySqlConnection ConnectToDb();
    Response UpdateData(string sql);
}
