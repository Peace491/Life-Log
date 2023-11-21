namespace Peace.Lifelog.DataAccess;

using DomainModels;

using MySql.Data.MySqlClient;

public interface ICreateDataOnlyDAO {

    MySqlConnection ConnectToDb();
    
    Response CreateData(string sql);
}
