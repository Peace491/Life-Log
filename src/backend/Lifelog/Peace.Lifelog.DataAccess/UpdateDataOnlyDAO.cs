namespace Peace.Lifelog.DataAccess;

using DomainModels;

using MySql.Data.MySqlClient;

public class UpdateDataOnlyDAO : IUpdateDataOnlyDAO
{
    LifelogConfig lifelogConfig = LifelogConfig.LoadConfiguration();

    public MySqlConnection ConnectToDb()
    {
        return new MySqlConnection(lifelogConfig.UpdateOnlyConnectionString);
    }

    public async Task<Response> UpdateData(string sql)
    {
        var response = new Response();

        var logTransaction = new LogTransaction();

        if (sql == "")
        {
            response.HasError = true;
            response.ErrorMessage = "Empty Input";

            var logTransactionResponse = await logTransaction.CreateDataAccessTransactionLog("ERROR", "Update Data input is empty");

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

                    response.Output = [dbResponse];
                }

                transaction.Commit();

                response.HasError = false;

                // var logTransactionResponse = await logTransaction.CreateDataAccessTransactionLog("Info", "Update Data is successful");

                // response.LogId = logTransactionResponse.LogId;
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

            var logTransactionResponse = await logTransaction.CreateDataAccessTransactionLog("ERROR", "Update Data is unsuccessful");

            response.LogId = logTransactionResponse.LogId;
        }

        return response;
    }
}
