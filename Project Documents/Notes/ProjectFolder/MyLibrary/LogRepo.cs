using MyLibrary;

public class LogRepo
{
    private readonly ISqlDAO _sqlDAO;

    public LogRepo(ISqlDAO sqlDAO)
    {
        _sqlDAO = sqlDAO;
    }

    public bool Log(string logLevel, string category, string message)
    {
        _sqlDAO.ExecuteSQL($"Insert into dbo.Logs values ({DateTime.UtcNow}, {logLevel}, {category}, {message})");
        return true;
    }
}

