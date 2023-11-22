namespace Peace.Lifelog.DataAccess;

using DomainModels;

using MySql.Data.MySqlClient;

public class CreateDataOnlyDAO : ICreateDataOnlyDAO {
    private readonly string connectionString = "Server = localhost; Database = LifelogDB; User ID = CreateUser; Password = password;";

    public MySqlConnection ConnectToDb()
    {
        return new MySqlConnection(connectionString);
    }

    public async Task<Response> CreateData(string sql) 
    {
        var response = new Response();

        var logTransaction = new LogTransaction();

        if (sql == "")
        {
            response.HasError = true;
            response.ErrorMessage = "Empty Input";

            var logTransactionResponse = logTransaction.CreateDataAccessTransactionLog("ERROR", "Create Data input is empty");

            response.LogId = logTransactionResponse.LogId;

            return response;
        }
        
        try 
        {
            var connection = ConnectToDb();

            connection.Open();
            
            await using (var command = new MySqlCommand())
            {
                // Set the connection for the command
                command.Connection = connection;

                // Define the SQL command
                command.CommandText = sql;

                // Execute the SQL command
                var dbResponse = command.ExecuteNonQuery();

                response.Output = [dbResponse];
            }

            connection.Close();

            var logTransactionResponse = logTransaction.CreateDataAccessTransactionLog("Info", "Data Create is successful");

            response.LogId = logTransactionResponse.LogId;

            response.HasError = false;

        } 
        catch (Exception error)
        {
            response.HasError = true;
            response.ErrorMessage = error.Message;

            var logTransactionResponse = logTransaction.CreateDataAccessTransactionLog("Info", "Data Create failed");

            response.LogId = logTransactionResponse.LogId;
        }

        return response;
    }
}
