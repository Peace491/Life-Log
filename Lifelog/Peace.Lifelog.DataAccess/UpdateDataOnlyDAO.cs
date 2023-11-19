namespace Peace.Lifelog.DataAccess;

using DomainModels;

using MySql.Data.MySqlClient;

public class UpdateDataOnlyDAO : IUpdateDataOnlyDAO {
    private readonly string connectionString = "Server = localhost; Database = LifelogDB; User ID = UpdateUser; Password = password;";

    public Response UpdateData(string sql) 
    {
        var response = new Response();

        if (sql == "")
        {
            response.HasError = true;
            response.ErrorMessage = "Empty Input";
            return response;
        }
        
        try 
        {
            using(var connection = new MySqlConnection(connectionString))
            {
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
            }

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
