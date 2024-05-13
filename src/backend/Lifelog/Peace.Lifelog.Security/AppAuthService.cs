namespace Peace.Lifelog.Security;

using Peace.Lifelog.DataAccess;
using Peace.Lifelog.Logging;

/// <summary>
/// AppAuthService contains methods for authentication and authorization of users
/// </summary>
public class AppAuthService : IAuthenticator, IAuthorizor
{
    /// <summary>
    /// Attempts to authenticate a user into a generic system utilizing a relational SQL database
    /// Upon Successful authenticaiton, returns claims
    /// Method is logged
    /// </summary>
    /// <param name="authRequest"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public async Task<AppPrincipal>? AuthenticateUser(IAuthenticationRequest authRequest)
    {
        // Early exit
        #region  Validate arguments (value of parameter)
        if (authRequest is null)
        {
            throw new ArgumentNullException(nameof(authRequest));
        }

        if (String.IsNullOrEmpty(authRequest.UserId.Type) || String.IsNullOrEmpty(authRequest.UserId.Value))
        {
            throw new ArgumentNullException($"{nameof(authRequest.UserId)} must not be null");
        }
        
        if (String.IsNullOrEmpty(authRequest.Proof.Type) || String.IsNullOrEmpty(authRequest.Proof.Value))
        {
            throw new ArgumentNullException($"{nameof(authRequest.Proof)} must not be null");
        }

        if (String.IsNullOrEmpty(authRequest.Claims.Type))
        {
            throw new ArgumentNullException($"{nameof(authRequest.Claims)} must not be null");
        }

        #endregion

        AppPrincipal? appPrincipal = null;

        bool hasError = false;
        string? errorMessage = null;

        var readDataOnlyDAO = new ReadDataOnlyDAO();

        try // Library should protect against failure 
        {
            // Step 1: Validate auth request (Go to database to see if it match up)
            var getClaimsSql = 
            $"SELECT {authRequest.Claims.Type} "
            + $"FROM {authRequest.ModelName} "
            + $"WHERE {authRequest.UserId.Type} = \"{authRequest.UserId.Value}\""
            +   $"AND {authRequest.Proof.Type} = \"{authRequest.Proof.Value}\"";

            var readResponse = await readDataOnlyDAO.ReadData(getClaimsSql);

            // Step 2: Populate app principal object
            var claims = new Dictionary<string, string>() {};

            if (readResponse.Output != null)
            {
                foreach (List<Object> readResponseData in readResponse.Output)
                {
                    claims.Add(authRequest.Claims.Type, readResponseData[0].ToString()!);
                }
            }
            else
            {
                throw new NullReferenceException();
            }
            

            appPrincipal = new AppPrincipal()
            {
                UserId = authRequest.UserId.Value,
                Claims = claims
            };
        } 
        catch (Exception ex)
        {
            hasError = true;
            errorMessage = ex.GetBaseException().Message; // First error that trigger
        }

        var createDataOnlyDAO = new CreateDataOnlyDAO();
        var logTarget = new LogTarget(createDataOnlyDAO, readDataOnlyDAO);
        var logging = new Logging(logTarget);

        if (hasError) {
            var _ = logging.CreateLog("Logs", "System", "ERROR", "Persistent Data Store", errorMessage);
        }
        else {
            var _ = logging.CreateLog("Logs", "System", "Info", "Persistent Data Store", $"{authRequest.UserId} successfully authenticates");
        }
        
        return appPrincipal!;

    }

    /// <summary>
    /// Checks a user's claims agains the required claims to access something in an application, to determine if a user is authorized for that action.
    /// Method is logged
    /// </summary>
    /// <param name="currentPrincipal"></param>
    /// <param name="requiredClaims"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
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
            if (currentPrincipal.Claims != null && !currentPrincipal.Claims.Contains(claim)) 
            {
                isAuthorize = false;
            }
        }

        var createDataOnlyDAO = new CreateDataOnlyDAO();
        var readDataOnlyDAO = new ReadDataOnlyDAO();
        var logTarget = new LogTarget(createDataOnlyDAO, readDataOnlyDAO);
        var logging = new Logging(logTarget);

        if (isAuthorize)
        {
            var _ = logging.CreateLog("Logs", "System", "Info", "Business", $"{currentPrincipal.UserId} successfully authorized");
        }
        else 
        {
            var _ = logging.CreateLog("Logs", "System", "ERROR", "Business", $"{currentPrincipal.UserId} failed to authorized");
        }

        return isAuthorize;
        
    }

}

