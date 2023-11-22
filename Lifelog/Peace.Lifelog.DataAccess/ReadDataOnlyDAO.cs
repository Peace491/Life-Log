namespace Peace.Lifelog.DataAccess;

using DomainModels;

using MySql.Data.MySqlClient;

public class ReadDataOnlyDAO : IReadDataOnlyDAO {
    private readonly string connectionString = "Server = localhost; Database = LifelogDB; User ID = ReadUser; Password = password;";

    public MySqlConnection ConnectToDb()
    {
        return new MySqlConnection(connectionString);
    }

    public async Task<Response> ReadData(string sql, int count = 10, int currentPage = 0) 
    {
        var response = new Response();

        var logTransaction = new LogTransaction();

        if (sql == "")
        {
            response.HasError = true;
            response.ErrorMessage = "Empty Input";

            var logTransactionResponse = logTransaction.CreateDataAccessTransactionLog("ERROR", "Read Data input is empty");

            response.LogId = logTransactionResponse.LogId;

            return response;
        }

        if (count < 0)
        {
            response.HasError = true;
            response.ErrorMessage = "Invalid Count";

            var logTransactionResponse = logTransaction.CreateDataAccessTransactionLog("ERROR", "Read Data count input is invalid");

            response.LogId = logTransactionResponse.LogId;

            return response;
        }

        if (currentPage < 0)
        {
            response.HasError = true;
            response.ErrorMessage = "Invalid Page";

            var logTransactionResponse = logTransaction.CreateDataAccessTransactionLog("ERROR", "Read Data currentPage input is invalid");

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
                command.CommandText = sql + $" LIMIT {count} OFFSET {count * currentPage}";

                // Execute the SQL command
                var dbResponse = command.ExecuteReader();

                while (dbResponse.Read())
                {
                    if (response.Output == null)
                    {
                        response.Output = new List<Object>();
                    }
                    
                    List<object> row = new List<object>();

                    for (int i = 0; i < dbResponse.FieldCount; i++)
                    {
                        row.Add(dbResponse[i]);
                    }

                    response.Output.Add(row);
                    
                }
            }

            connection.Close();
            
            response.HasError = false;

            var logTransactionResponse = logTransaction.CreateDataAccessTransactionLog("Info", "Read Data is successful");

            response.LogId = logTransactionResponse.LogId;

        } 
        catch (Exception error)
        {
            response.HasError = true;
            response.ErrorMessage = error.Message;

            var logTransactionResponse = logTransaction.CreateDataAccessTransactionLog("ERROR", "Read Data is unsuccessful");

            response.LogId = logTransactionResponse.LogId;
        }

        return response;
    }
}
