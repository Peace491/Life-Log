namespace Peace.Lifelog.Infrastructure;
using Peace.Lifelog.DataAccess;
using Peace.Lifelog.Logging;
using DomainModels;
using System.Text.RegularExpressions;

public class UserManagmentRepo : IUserManagmentRepo
{
    private readonly IReadDataOnlyDAO readDataOnlyDAO;
    private readonly IDeleteDataOnlyDAO deleteDataOnlyDAO;
    private readonly ILogging logger;

    public UserManagmentRepo(IReadDataOnlyDAO readDataOnlyDAO, IDeleteDataOnlyDAO deleteDataOnlyDAO, ILogging logger)
    {
        this.readDataOnlyDAO = readDataOnlyDAO;
        this.deleteDataOnlyDAO = deleteDataOnlyDAO;
        this.logger = logger;
    }

    public async Task<Response> DeletePersonalIdentifiableInformation(string userHash)
    {
        Response response = new Response();
        try
        {
            // check for sql injection in userhash input
            if(ContainsSQLInjection(userHash))
            {
                throw new Exception("SQL Injection detected in delete PII userhash");
            }

            // substitute userhash in query
            string deleteQuery = $"DELETE FROM LifelogDB.Logs WHERE UserHash = '{userHash}';";

            // execute 
            response = await deleteDataOnlyDAO.DeleteData(deleteQuery);
        }
        catch (Exception ex)
        {
            _ = await logger.CreateLog("logs", userHash, "Server", "Error", ex.Message);
        }
        return response;
    }

    public async Task<Response> ViewPersonalIdentifiableInformation(string userHash)
    {
        Response response = new Response();
        try
        {
            // check for sql injection in userhash input
            if(ContainsSQLInjection(userHash))
            {
                throw new Exception("SQL Injection detected in View PII userhash");
            }

            // substitute userhash in query
            string selectQuery = $"SELECT * FROM LifelogDB.Logs WHERE UserHash = '{userHash}';";

            // execute 
            response = await readDataOnlyDAO.ReadData(selectQuery, null);
        }
        catch (Exception ex)
        {
            _ = await logger.CreateLog("logs", userHash, "Server", "Error", ex.Message);
        }
        return response;
    }

    private static bool ContainsSQLInjection(string input)
    {
        // Regular expression to check for sql injection
        string pattern = @"('|;|--|\b(ALTER|CREATE|DELETE|DROP|EXEC|EXECUTE|INSERT|MERGE|SELECT|UPDATE|UNION|ALTER|GRANT|REVOKE)\b)";
        Regex sqlCheckRegex = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);

        // Check if the input string matches the SQL injection pattern
        return sqlCheckRegex.IsMatch(input);
    }
}
