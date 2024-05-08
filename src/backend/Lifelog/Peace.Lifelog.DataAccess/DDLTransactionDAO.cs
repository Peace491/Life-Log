﻿namespace Peace.Lifelog.DataAccess;

using DomainModels;

using MySql.Data.MySqlClient;

public class DDLTransactionDAO : IDDLTransactionDAO
{
    LifelogConfig lifelogConfig = LifelogConfig.LoadConfiguration();

    public MySqlConnection ConnectToDb()
    {
        return new MySqlConnection(lifelogConfig.DDLConnectionString);
    }

    public async Task<Response> ExecuteDDLCommand(string sql)
    {
        var response = new Response();

        var logTransaction = new LogTransaction();

        if (sql == "")
        {
            response.HasError = true;
            response.ErrorMessage = "Empty Input";

            var logTransactionResponse = await logTransaction.CreateDataAccessTransactionLog("ERROR", "DDL Transaction input is empty");

            response.LogId = logTransactionResponse.LogId;

            return response;
        }

        MySqlTransaction? transaction = null;
        
        try 
        {
            var connection = ConnectToDb();
            
            connection.Open();

            transaction = connection.BeginTransaction();
            
            using (var command = new MySqlCommand())
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

            connection.Close();
            
            response.HasError = false;

            // var logTransactionResponse = await logTransaction.CreateDataAccessTransactionLog("Info", "DDL Transaction Successful");

            // response.LogId = logTransactionResponse.LogId;

        } 
        catch (Exception error)
        {
            if (transaction != null) 
            {
                transaction.Rollback();
            }
            
            response.HasError = true;
            response.ErrorMessage = error.Message;

            var logTransactionResponse = await logTransaction.CreateDataAccessTransactionLog("ERROR", "Data Transaction Failed");

            response.LogId = logTransactionResponse.LogId;
        }

        return response;
    }

}
