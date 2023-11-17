namespace Peace.Lifelog.DataAccess;

using System.Drawing.Printing;
using DomainModels;

using MySql.Data.MySqlClient;

public class ReadDataOnlyDAO : IReadDataOnlyDAO {
    private readonly string connectionString = "Server = localhost; Database = LifelogDB; User ID = ReadUser; Password = password;";

    public Response ReadData(string sql, int count = 10, int currentPage = 0) 
    {
        var response = new Response();
        
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
