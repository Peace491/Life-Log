namespace Peace.Lifelog.Infrastructure;
using Peace.Lifelog.DataAccess;
using Peace.Lifelog.Logging;
using DomainModels;
using System.Text.RegularExpressions;
using Google.Protobuf.WellKnownTypes;

public class UserManagmentRepo : IUserManagmentRepo
{
    private readonly ICreateDataOnlyDAO createDataOnlyDAO;
    private readonly IReadDataOnlyDAO readDataOnlyDAO;
    private readonly IDeleteDataOnlyDAO deleteDataOnlyDAO;
    private readonly ILogging logger;

    public UserManagmentRepo(ICreateDataOnlyDAO createDataOnlyDAO, IReadDataOnlyDAO readDataOnlyDAO, IDeleteDataOnlyDAO deleteDataOnlyDAO, ILogging logger)
    {
        this.createDataOnlyDAO = createDataOnlyDAO;
        this.readDataOnlyDAO = readDataOnlyDAO;
        this.deleteDataOnlyDAO = deleteDataOnlyDAO;
        this.logger = logger;
    }

    #region User Management Repository Methods
    public async Task<Response> GetUserHashFromUserId(string userId)
    {
        Response response = new Response();
        try
        {
            // check for sql injection in userId input
            if(ContainsSQLInjection(userId))
            {
                throw new Exception("SQL Injection detected in get userhash from userid");
            }

            // substitute userId in query
            string sql = $"SELECT UserHash FROM LifelogAccount WHERE UserId=\"{userId}\"";

            // execute 
            response = await readDataOnlyDAO.ReadData(sql, null);
        }
        catch (Exception ex)
        {
            _ = await logger.CreateLog("logs", userId, "Server", "Error", ex.Message);
        }
        return response;
    }
    public async Task<Response> GetUserIdFromUserHash(string userHash)
    {
        Response response = new Response();
        try
        {
            // check for sql injection in userhash input
            if(ContainsSQLInjection(userHash))
            {
                throw new Exception("SQL Injection detected in get userid from userhash");
            }

            // substitute userhash in query
            string sql = $"SELECT UserId FROM LifelogUserHash WHERE UserHash=\"{userHash}\"";

            // execute 
            response = await readDataOnlyDAO.ReadData(sql, null);
        }
        catch (Exception ex)
        {
            _ = await logger.CreateLog("logs", userHash, "Server", "Error", ex.Message);
        }
        return response;
    }

    public async Task<Response> CreateRecSummaryForUser(string userHash)
    {
        Response response = new Response();
        try
        {
            // check for sql injection in userhash input
            if(ContainsSQLInjection(userHash))
            {
                throw new Exception("SQL Injection detected in create rec summary for user");
            }

            // substitute userhash in query
            string sql = $"INSERT INTO `RecSummary` (`UserHash`, `Category1`, `Category2`, `SystemMostPopular`) VALUES ('{userHash}', 'Mental Health', 'Physical Health', (SELECT Category1 FROM (SELECT Category1 FROM RecSummary WHERE UserHash = 'system') AS derivedTable));";

            // execute 
            response = await createDataOnlyDAO.CreateData(sql);
        }
        catch (Exception ex)
        {
            _ = await logger.CreateLog("logs", userHash, "Server", "Error", ex.Message);
        }
        return response;
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

    #endregion

    #region Helper Methods
    private static bool ContainsSQLInjection(string input)
    {
        // Regular expression to check for sql injection
        string pattern = @"('|;|--|\b(ALTER|CREATE|DELETE|DROP|EXEC|EXECUTE|INSERT|MERGE|SELECT|UPDATE|UNION|ALTER|GRANT|REVOKE)\b)";
        Regex sqlCheckRegex = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);

        // Check if the input string matches the SQL injection pattern
        return sqlCheckRegex.IsMatch(input);
    }
    #endregion
}
