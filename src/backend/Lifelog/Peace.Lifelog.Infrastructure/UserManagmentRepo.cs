namespace Peace.Lifelog.Infrastructure;
using Peace.Lifelog.DataAccess;
using Peace.Lifelog.Logging;
using DomainModels;
using System.Text.RegularExpressions;
using Org.BouncyCastle.Asn1.Cms;

public class UserManagmentRepo : IUserManagmentRepo
{
    private readonly ICreateDataOnlyDAO createDataOnlyDAO;
    private readonly IReadDataOnlyDAO readDataOnlyDAO;
    private readonly IUpdateDataOnlyDAO updateDataOnlyDAO;
    private readonly IDeleteDataOnlyDAO deleteDataOnlyDAO;
    private readonly ILogging logger;

    public UserManagmentRepo(ICreateDataOnlyDAO createDataOnlyDAO, IReadDataOnlyDAO readDataOnlyDAO, IUpdateDataOnlyDAO updateDataOnlyDAO, IDeleteDataOnlyDAO deleteDataOnlyDAO, ILogging logger)
    {
        this.createDataOnlyDAO = createDataOnlyDAO;
        this.readDataOnlyDAO = readDataOnlyDAO;
        this.updateDataOnlyDAO = updateDataOnlyDAO;
        this.deleteDataOnlyDAO = deleteDataOnlyDAO;
        this.logger = logger;
    }

    #region User Management Repository Methods Userstory 1 & 2
    public async Task<Response> GetAllNormalUsers()
    {
        Response response = new Response();
        try
        {
            string sql = "SELECT UserId FROM LifelogUserRole WHERE Role = 'Normal'";

            // execute 
            response = await readDataOnlyDAO.ReadData(sql, null);
        }
        catch (Exception ex)
        {
            _ = await logger.CreateLog("logs", "root", "Server", "Error", ex.Message);
        }
        return response;
    }

    public async Task<Response> GetAllNonRootUsers()
    {
        Response response = new Response();
        try
        {
            string sql = "SELECT UserId FROM LifelogUserRole WHERE Role != 'Root'";

            // execute 
            response = await readDataOnlyDAO.ReadData(sql, null);
        }
        catch (Exception ex)
        {
            _ = await logger.CreateLog("logs", "root", "Server", "Error", ex.Message);
        }
        return response;
    }

    public async Task<Response> ChangeUserRole(string userId, string role)
    {
        Response response = new Response();
        try
        {
            // check for sql injection in userId input
            if(ContainsSQLInjection(userId))
            {
                throw new Exception("SQL Injection detected in change user role");
            }

            if(role != "Normal" && role != "Admin" && role != "Root")
            {
                throw new Exception("Invalid role");
            }

            // substitute userId in querys
            string accountSql = $"UPDATE LifelogAccount SET Role = \"{role}\" WHERE UserId = \"{userId}\"";
            string userRoleSql = $"UPDATE LifelogUserRole SET Role = \"{role}\" WHERE UserId = \"{userId}\"";

            // execute 
            response = await updateDataOnlyDAO.UpdateData(userRoleSql);
            if(response.HasError == false)
            {
                response = await updateDataOnlyDAO.UpdateData(accountSql);
            }
        }
        catch (Exception ex)
        {
            _ = await logger.CreateLog("logs", userId, "Server", "Error", ex.Message);
        }
        return response;
    }

    #endregion

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
    public async Task<Response> CreateLifelogUserRoleInDB(string userId, string role)
    {
        Response response = new Response();
        try
        {
            // check for sql injection in userId input
            if(ContainsSQLInjection(userId))
            {
                throw new Exception("SQL Injection detected in create lifelog user role in db");
            }

            // substitute userId in query
            string sql = $"INSERT INTO LifelogUserRole (UserId, Role) VALUES (\"{userId}\", \"{role}\")";

            // execute 
            response = await createDataOnlyDAO.CreateData(sql);
        }
        catch (Exception ex)
        {
            _ = await logger.CreateLog("logs", userId, "Server", "Error", ex.Message);
        }
        return response;
    }
    
    public async Task<Response> CreateLifelogUserOTPInDB(string userHash)
    {
        Response response = new Response();
        try
        {
            // check for sql injection in userId input
            if(ContainsSQLInjection(userHash))
            {
                throw new Exception("SQL Injection detected in create lifelog user OTP in db");
            }

            // substitute userId in query
            string sql = $"INSERT INTO LifelogUserOTP (UserHash) VALUES (\"{userHash}\")";
            // execute 
            response = await createDataOnlyDAO.CreateData(sql);
        }
        catch (Exception ex)
        {
            _ = await logger.CreateLog("logs", userHash, "Server", "Error", ex.Message);
        }
        return response;
    }

    public async Task<Response> CreateLifelogAuthenticationInDB(string userId, string userHash, string role)
    {
        Response response = new Response();
        try
        {
            // check for sql injection in userId input
            if(ContainsSQLInjection(userId))
            {
                throw new Exception("SQL Injection detected in create lifelog authentication in db");
            }

            // substitute userId in query
            string sql = $"INSERT INTO LifelogAuthentication (UserId, UserHash, Role) VALUES (\"{userId}\", \"{userHash}\", \"{role}\")";

            // execute 
            response = await createDataOnlyDAO.CreateData(sql);
        }
        catch (Exception ex)
        {
            _ = await logger.CreateLog("logs", userId, "Server", "Error", ex.Message);
        }
        return response;
    }

    public async Task<Response> GetAccountRecoveryRequestRoot()
    {
        Response response = new Response();
        try
        {
            // substitute userId in query
            string sql = "SELECT LifelogAccount.UserId FROM LifelogAccount INNER JOIN LifelogAccountRecoveryRequest WHERE LifelogAccount.UserId = LifelogAccountRecoveryRequest.UserId";

            // execute 
            response = await readDataOnlyDAO.ReadData(sql, null);
        }
        catch (Exception ex)
        {
            _ = await logger.CreateLog("logs", "root", "Server", "Error", ex.Message);
        }
        return response;
    }
    public async Task<Response> GetAccountRecoveryRequestNotRoot()
    {
        Response response = new Response();
        try
        {
            // substitute userId in query
            string sql = "SELECT LifelogAccount.UserId FROM LifelogAccount INNER JOIN LifelogAccountRecoveryRequest "
            + "WHERE LifelogAccount.UserId = LifelogAccountRecoveryRequest.UserId "
            + "AND (LifelogAccount.Role != 'Admin' AND LifelogAccount.Role != 'Root') ";

            // execute 
            response = await readDataOnlyDAO.ReadData(sql, null);
        }
        catch (Exception ex)
        {
            _ = await logger.CreateLog("logs", "root", "Server", "Error", ex.Message);
        }
        return response;
    }

    public async Task<Response> CreateAccountRecoveryRequest(string userId)
    {
        Response response = new Response();
        try
        {
            // check for sql injection in userId input
            if(ContainsSQLInjection(userId))
            {
                throw new Exception("SQL Injection detected in create account recovery request");
            }

            // substitute userId in query
            string sql = $"INSERT INTO LifelogAccountRecoveryRequest (UserId) VALUES (\"{userId}\")";

            // execute 
            response = await createDataOnlyDAO.CreateData(sql);
        }
        catch (Exception ex)
        {
            _ = await logger.CreateLog("logs", userId, "Server", "Error", ex.Message);
        }
        return response;
    }
    public async Task<Response> DeleteAccountRecoveryRequest(string userId)
    {
        Response response = new Response();
        try
        {
            // check for sql injection in userId input
            if(ContainsSQLInjection(userId))
            {
                throw new Exception("SQL Injection detected in delete account recovery request");
            }

            // substitute userId in query
            string deleteQuery = $"DELETE FROM LifelogAccountRecoveryRequest WHERE UserId = \"{userId}\"";

            // execute 
            response = await deleteDataOnlyDAO.DeleteData(deleteQuery);
        }
        catch (Exception ex)
        {
            _ = await logger.CreateLog("logs", userId, "Server", "Error", ex.Message);
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

    public async Task<Response> UpdateUserFirstLogin(string userHash)
    {
        Response response = new Response();
        try
        {
            // check for sql injection in userhash input
            if(ContainsSQLInjection(userHash))
            {
                throw new Exception("SQL Injection detected in update user first login");
            }

            // substitute userhash in query
            string updateQuery = $"UPDATE LifelogAuthentication SET FirstLogin = 1 WHERE UserHash = '{userHash}';";

            // execute 
            response = await updateDataOnlyDAO.UpdateData(updateQuery);
        }
        catch (Exception ex)
        {
            _ = await logger.CreateLog("logs", userHash, "Server", "Error", ex.Message);
        }
        return response;
    }

    public async Task<Response> CheckSuccessfulReg(string userHash)
    {
        Response response = new Response();
        try
        {
            // check for sql injection in userhash input
            if(ContainsSQLInjection(userHash))
            {
                throw new Exception("SQL Injection detected in check successful reg");
            }

            // substitute userhash in query
            string selectQuery = $"SELECT FirstLogin FROM LifelogAuthentication WHERE UserHash = '{userHash}';";

            // execute 
            response = await readDataOnlyDAO.ReadData(selectQuery, null);
        }
        catch (Exception ex)
        {
            _ = await logger.CreateLog("logs", userHash, "Server", "Error", ex.Message);
        }
        return response;
    }

    public async Task<Response> UpdateUserStatus(string userId, string status)
    {
        Response response = new Response();

        try 
        {
            string sql = $"UPDATE LifelogAccount SET AccountStatus = \"{status}\" WHERE UserId = \"{userId}\"";
            response = await updateDataOnlyDAO.UpdateData(sql);
        }
         catch (Exception ex)
        {
            response.HasError = true;
            response.ErrorMessage = ex.Message;
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
