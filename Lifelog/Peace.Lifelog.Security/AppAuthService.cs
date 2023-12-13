namespace Peace.Lifelog.Security;

using Peace.Lifelog.DataAccess;
using Peace.Lifelog.Logging;

public class AppAuthService : IAuthenticator, IAuthorizor
{
    public async Task<AppPrincipal>? AuthenticateUser(AuthenticationRequest authRequest)
    {
        // Early exit
        #region  Validate arguments (value of parameter)
        if (authRequest is null)
        {
            throw new ArgumentNullException(nameof(authRequest));
        }

        if (string.IsNullOrWhiteSpace(authRequest.UserId))
        {
            throw new ArgumentNullException($"{nameof(authRequest.UserId)} must not be null");
        }
        
        if (string.IsNullOrWhiteSpace(authRequest.Proof))
        {
            throw new ArgumentNullException($"{nameof(authRequest.Proof)} must not be null");
        }

        if (authRequest.AuthSQLDetails is null)
        {
            throw new ArgumentNullException(nameof(authRequest.AuthSQLDetails));
        }

        #endregion

        AppPrincipal? appPrincipal = null;

        bool hasError = false;
        string? errorMessage = null;

        try // Library should protect against failure 
        {
            // Step 1: Validate auth request (Go to database to see if it match up)
            var readDataOnlyDAO = new ReadDataOnlyDAO();

            var getClaimsSql = 
            $"SELECT {authRequest.AuthSQLDetails.ClaimColumnName} "
            + $"FROM {authRequest.AuthSQLDetails.TableName} "
            + $"WHERE {authRequest.AuthSQLDetails.UserIdColumnName} = {authRequest.UserId} "
            +   $"AND {authRequest.AuthSQLDetails.ProofColumnName} = {authRequest.Proof}";

            var readResponse = await readDataOnlyDAO.ReadData(getClaimsSql);

            // Step 2: Populate app principal object
            var claims = new Dictionary<string, string>() {};

            foreach (List<Object> readResponseData in readResponse.Output)
            {
                claims.Add(authRequest.AuthSQLDetails.ClaimColumnName, readResponseData[0].ToString());
            }

            appPrincipal = new AppPrincipal()
            {
                UserId = authRequest.UserId,
                Claims = claims
            };
        } 
        catch (Exception ex)
        {
            hasError = true;
            errorMessage = ex.GetBaseException().Message; // First error that trigger
        }

        var createDataOnlyDAO = new CreateDataOnlyDAO();
        var logTarget = new LogTarget(createDataOnlyDAO);
        var logging = new Logging(logTarget);

        if (hasError) {
            logging.CreateLog("Logs", "ERROR", "Persistent Data Store", errorMessage);
        }
        else {
            logging.CreateLog("Logs", "Info", "Persistent Data Store", $"{authRequest.UserId} successfully authenticates");
        }
        
        return appPrincipal;

    }

    public bool IsAuthorize(AppPrincipal currentPrincipal, 
                            IDictionary<string, string> requiredClaims) 
    {
        // Dictionary<string, string>() {new ("RoleName", "Admin")}
        // key - RoleName, value - Admin
        if (currentPrincipal is null) 
        {
            throw new ArgumentNullException(nameof(currentPrincipal));
        }

        if (requiredClaims is null) 
        {
            throw new ArgumentNullException(nameof(requiredClaims));
        }

        var isAuthorize = true;

        foreach (var claim in requiredClaims)
        {
            if (!currentPrincipal.Claims.Contains(claim)) 
            {
                isAuthorize = false;
            }
        }

        var createDataOnlyDAO = new CreateDataOnlyDAO();
        var logTarget = new LogTarget(createDataOnlyDAO);
        var logging = new Logging(logTarget);

        if (isAuthorize)
        {
            logging.CreateLog("Logs", "Info", "Business", $"{currentPrincipal.UserId} successfully authorized");
        }
        else 
        {
            logging.CreateLog("Logs", "ERROR", "Business", $"{currentPrincipal.UserId} failed to authorized");
        }

        return isAuthorize;
        
    }

}

