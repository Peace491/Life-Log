namespace Peace.Lifelog.Logging;

public class LogRepo : ILogRepo
{
    public bool CreateLog(string level, string category, string? message)
    {

        var createDataOnlyDAO = new CreateDataOnlyDAO();

        createDataOnlyDAO.CreateData(/* sql */); // insert sql statement to insert into log table.
         // above statement is taking the passed info + timestamp 

    }
}