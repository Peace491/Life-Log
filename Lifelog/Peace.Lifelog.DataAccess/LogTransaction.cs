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

    public async Task<Response> CreateDataAccessTransactionLog(string level, string message)
    {
        var response = new Response();

        MySqlTransaction? transaction = null;

        try 
        {
            var connection = ConnectToDb();

            connection.Open();

            transaction = connection.BeginTransaction();
            
            await using (var command = new MySqlCommand())
            {
                // Set the connection for the command
                command.Connection = connection;

                // Define the SQL command
                string sql = $"INSERT INTO Logs (LogTimestamp, LogLevel, LogCategory, LogMessage) VALUES (NOW(), '{level}', 'Persistent Data Store', '{message}')";
                command.CommandText = sql;

                // Define the transaction
                command.Transaction = transaction;

                // Execute the SQL command
                var dbResponse = command.ExecuteNonQuery();

                response.Output = [dbResponse];

                response.LogId = command.LastInsertedId;
            }

            transaction.Commit();

            connection.Close();

            response.HasError = false;

        } 
        catch (Exception error)
        {
            if (transaction != null) 
            {
                transaction.Rollback();
            }

            response.HasError = true;
            response.ErrorMessage = error.Message;
        }

        return response;
    }

    public async Task<Response> DeleteDataAccessTransactionLog(long logId)
    {
        var response = new Response();

        MySqlTransaction? transaction = null;

        try 
        {
            var connection = ConnectToDb();

            connection.Open();

            transaction = connection.BeginTransaction();
            
            await using (var command = new MySqlCommand())
            {
                // Set the connection for the command
                command.Connection = connection;

                // Define the SQL command
                string sql = $"DELETE FROM Logs WHERE LogID = {logId}";
                command.CommandText = sql;

                // Define the transaction
                command.Transaction = transaction;

                // Execute the SQL command
                var dbResponse = command.ExecuteNonQuery();

                response.Output = [dbResponse];

                response.LogId = command.LastInsertedId;
            }

            transaction.Commit();

            connection.Close();

            response.HasError = false;

        } 
        catch (Exception error)
        {
            if (transaction != null) 
            {
                transaction.Rollback();
            }
            
            response.HasError = true;
            response.ErrorMessage = error.Message;
        }

        return response;
    }
}
