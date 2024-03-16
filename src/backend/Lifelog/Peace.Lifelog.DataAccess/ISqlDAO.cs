namespace Peace.Lifelog.DataAccess;

using MySql.Data.MySqlClient;

public interface ISqlDAO
{
    MySqlConnection ConnectToDb();
}
