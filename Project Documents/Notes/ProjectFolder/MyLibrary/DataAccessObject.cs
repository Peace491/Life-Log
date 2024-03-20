namespace MyLibrary;

// using Microsoft.Data.SqlClient;

public class Result 
{
    public bool? HasError {get; set;}

    public string? ErrorMessage { get; set; }

    public bool? HasResult { get; set; }
}
public class DataAccessObject // Any class that interact with data store
{
    private int _count = 0;

    // public Result CreateLog(){
    //     //
    // }

    public Result CreateUser (string firstName, string lastName) 
    {
        var result = new Result();

        try 
        {
            // var connectionString = "";
            // using(var connection = new SqlConnection(connectionString))
            // {
            //     connection.Open();
            //     var sql = $"INSERT INTO users VALUES({firstName}, {lastName})";
            //     using(var command = new SqlCommand(sql, connection))
            //     {
            //         command.ExecuteNonQuery();
            //     }
            // }

            result.HasError = false;
        } 
        catch (Exception ex)
        {
            result.ErrorMessage = ex.Message;
            result.HasError = true;
            //logging
        }
        return result;
    }

    public int Users
    {
        get 
        {
            return _count;
        }
    }

}

// public class Logger()
// {
//     public Result Log(string logLevel, string logCategory, string context)
//     {
//         var dao = new DataAccessObject();

//         return dao.CreateLog(DateTime.UtcNow, logLevel, logCategory, context);
//     }
// }
