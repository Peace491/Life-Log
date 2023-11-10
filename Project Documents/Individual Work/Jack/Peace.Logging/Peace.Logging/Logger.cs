using Peace.Logging.DataAccess;

public class Logger
{
    // 
    public static Tuple<DateTime, string> GetLogDetails()
    {
        string msg = "Go team Peace!";
        return Tuple.Create(DateTime.Now, msg);
    }

    public void NewLog()
    {
        DataAccess dataAccess = new DataAccess();

        // the log's responability is to generate the log info
        Tuple<DateTime, string> logInfo = GetLogDetails();

        // use dataAccess to save the log
        dataAccess.SaveLog(logInfo.Item1, logInfo.Item2);
    }
}
