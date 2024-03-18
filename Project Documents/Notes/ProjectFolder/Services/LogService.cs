using System.Data;

namespace Services;

public interface ILogService
{
    bool Log(string logLevel, string category, string? message);
}

public interface ILogTarget
{
    bool Write(DateTimeOffset dataTime, string logLevel, string category, string? message);
}

public class SqlDbLogTarget : ILogTarget
{
    public bool Write(DateTimeOffset dateTime, string logLevel, string category, string? message)
    {
        using (var connection = new SqlConnection(connString))
        {
            connection.Open();
            using(var command = new SqlCommand("..."))
            {
                command.CommandType = CommandType.Text;

                var rowsAffected = command.ExecuteNonQuery();
                
                if (rowsAffected > 1)
                {
                    throw new Exception();
                }
            }
        }
    }
}

public class FileLogTarget : ILogTarget
{
    public bool Write(DateTimeOffset dateTime, string logLevel, string category, string? message)
    {
        using (var sw = new File.Create("filepath"))
        {
            //...
            sw.WriteBytes(dbBytes);
        }
    }
}

public class LogService : ILogService
{
    private readonly ILogTarget _logTarget;
    public LogService(ILogTarget logTarget) => _logTarget = logTarget; // Composition Root -> Entry Point

    // Not the best approach
    // Error
    public bool Log(string logLevel, string category, string? message)
    {
        _logRepo.Log(logLevel, category, message);
        return true;
    }
}
