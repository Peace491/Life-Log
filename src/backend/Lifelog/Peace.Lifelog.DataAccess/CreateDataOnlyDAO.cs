namespace Peace.Lifelog.DataAccess;

using System.Diagnostics;
using DomainModels;

using MySql.Data.MySqlClient;

public class CreateDataOnlyDAO : ICreateDataOnlyDAO
{
    LifelogConfig lifelogConfig = LifelogConfig.LoadConfiguration();

    public MySqlConnection ConnectToDb()
    {
        return new MySqlConnection(lifelogConfig.CreateOnlyConnectionString);
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
            using (var connection = ConnectToDb())
            {

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

                // var logTransactionResponse = await logTransaction.CreateDataAccessTransactionLog("Info", "Data Create is successful");

                // response.LogId = logTransactionResponse.LogId;

                response.HasError = false;
            }

        }
        catch (Exception error)
        {
            if (transaction != null)
            {
                transaction.Rollback();
            }

            response.HasError = true;
            response.ErrorMessage = error.Message;

            var logTransactionResponse = await logTransaction.CreateDataAccessTransactionLog("Info", error.Message);

            response.LogId = logTransactionResponse.LogId;
        }

        return response;
    }
}
