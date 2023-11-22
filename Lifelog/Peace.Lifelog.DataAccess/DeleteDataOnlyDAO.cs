namespace Peace.Lifelog.DataAccess;

using DomainModels;

using MySql.Data.MySqlClient;

public class DeleteDataOnlyDAO : IDeleteDataOnlyDAO {
    private readonly string connectionString = "Server = localhost; Database = LifelogDB; User ID = DeleteUser; Password = password;";

    public MySqlConnection ConnectToDb()
    {
        return new MySqlConnection(connectionString);
    }

    public Response DeleteData(string sql)
    {
        var response = new Response();

        var logTransaction = new LogTransaction();

        if (sql == "")
        {
            response.HasError = true;
            response.ErrorMessage = "Empty Input";

            var logTransactionResponse = logTransaction.CreateDataAccessTransactionLog("ERROR", "Delete Data count input is empty");

            response.logId = logTransactionResponse.logId;

            return response;
        }
        
        try 
        {
            var connection = ConnectToDb();
            
            connection.Open();
            
            using (var command = new MySqlCommand())
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
            
            response.HasError = false;

            var logTransactionResponse = logTransaction.CreateDataAccessTransactionLog("ERROR", "Data Delete Successful");

            response.logId = logTransactionResponse.logId;

        } 
        catch (Exception error)
        {
            response.HasError = true;
            response.ErrorMessage = error.Message;

            var logTransactionResponse = logTransaction.CreateDataAccessTransactionLog("ERROR", "Data Delete Failed");

            response.logId = logTransactionResponse.logId;
        }

        return response;
    }
}