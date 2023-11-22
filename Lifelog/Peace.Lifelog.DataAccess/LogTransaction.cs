namespace Peace.Lifelog.DataAccess;

using DomainModels;

using MySql.Data.MySqlClient;

public class LogTransaction : ILogTransaction
{
    private readonly string connectionString = "Server = localhost; Database = LifelogDB; User ID = LogTransUser; Password = password;";

    public MySqlConnection ConnectToDb()
    {
        return new MySqlConnection(connectionString);
    }

    public Response CreateDataAccessTransactionLog(string level, string message)
    {
        var response = new Response();

        try 
        {
            var connection = ConnectToDb();

            connection.Open();
            
            using (var command = new MySqlCommand())
            {
                // Set the connection for the command
                command.Connection = connection;

                // Define the SQL command
                string sql = $"INSERT INTO Logs (LogTimestamp, LogLevel, LogCategory, LogMessage) VALUES (NOW(), '{level}', 'Persistent Data Store', '{message}')";
                command.CommandText = sql;

                // Execute the SQL command
                var dbResponse = command.ExecuteNonQuery();

                response.Output = [dbResponse];

                response.LogId = command.LastInsertedId;
            }

            connection.Close();

            response.HasError = false;

        } 
        catch (Exception error)
        {
            response.HasError = true;
            response.ErrorMessage = error.Message;
        }

        return response;
    }

    public Response DeleteDataAccessTransactionLog(long logId)
    {
        var response = new Response();

        try 
        {
            var connection = ConnectToDb();

            connection.Open();
            
            using (var command = new MySqlCommand())
            {
                // Set the connection for the command
                command.Connection = connection;

                // Define the SQL command
                string sql = $"DELETE FROM Logs WHERE LogID = {logId}";
                command.CommandText = sql;

                // Execute the SQL command
                var dbResponse = command.ExecuteNonQuery();

                response.Output = [dbResponse];

                response.LogId = command.LastInsertedId;
            }

            connection.Close();

            response.HasError = false;

        } 
        catch (Exception error)
        {
            response.HasError = true;
            response.ErrorMessage = error.Message;
        }

        return response;
    }
}
