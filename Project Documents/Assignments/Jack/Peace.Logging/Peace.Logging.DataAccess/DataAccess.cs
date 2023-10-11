namespace Peace.Logging.DataAccess;

using System;
using MySql.Data.MySqlClient;

public class DataAccess
{
    // Data Access responsability is to access datastore and insert/save info
    // In this case log info
    public void SaveLog(DateTime timeStamp, string logMessage)
    {
        // connection string to connect
        string connectionString = "Server=localhost;Database=Peace;Uid=root;Pwd=JackPickle123;";

        using MySqlConnection connection = new MySqlConnection(connectionString);
        connection.Open();

        // log insertion query
        string insertQuery = "INSERT INTO logs (log_timestamp, log_message) VALUES (@timeStamp, @logMessage)";

        using MySqlCommand command = new MySqlCommand(insertQuery, connection);
        command.Parameters.AddWithValue("@timeStamp", timeStamp);
        command.Parameters.AddWithValue("@logMessage", logMessage);

        command.ExecuteNonQuery();

        connection.Close();
    }
}
