namespace Peace.Lifelog.DataAccess;

using DomainModels;

using MySql.Data.MySqlClient;

public class CreateDataOnlyDAO : ICreateDataOnlyDAO
{
    private readonly string connectionString = "Server = localhost; Database = LifelogDB; User ID = CreateUser; Password = password;";

    public  MySqlConnection ConnectToDb()
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

            var logTransactionResponse = await logTransaction.CreateDataAccessTransactionLog("ERROR", "Create Data input is empty");

            response.LogId = logTransactionResponse.LogId;

            return response;
        }

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
                command.CommandText = sql;

                // Define the transaction
                command.Transaction = transaction;

                // Execute the SQL command
                var dbResponse = command.ExecuteNonQuery();

                response.Output = [command.LastInsertedId, dbResponse];
            }

            transaction.Commit();

            connection.Close();

            var logTransactionResponse = await logTransaction.CreateDataAccessTransactionLog("Info", "Data Create is successful");

            response.LogId = logTransactionResponse.LogId;

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

            var logTransactionResponse = await logTransaction.CreateDataAccessTransactionLog("Info", "Data Create failed");

            response.LogId = logTransactionResponse.LogId;
        }

        return response;
    }
}
