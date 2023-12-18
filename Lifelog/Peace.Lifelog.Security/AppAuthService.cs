namespace Peace.Lifelog.Security;

using Peace.Lifelog.DataAccess;
using Peace.Lifelog.Logging;

public class AppAuthService : IAuthenticator, IAuthorizor
{
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

        if (String.IsNullOrEmpty(authRequest.Claims.Type) || String.IsNullOrEmpty(authRequest.Claims.Value))
        {
            throw new ArgumentNullException($"{nameof(authRequest.Claims)} must not be null");
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
            $"SELECT {authRequest.Claims.Type} "
            + $"FROM {authRequest.ModelName} "
            + $"WHERE {authRequest.UserId.Type} = \"{authRequest.UserId.Value}\""
            +   $"AND {authRequest.Proof.Type} = \"{authRequest.Proof.Value}\"";

            var readResponse = await readDataOnlyDAO.ReadData(getClaimsSql);

            // Step 2: Populate app principal object
            var claims = new Dictionary<string, string>() {};

            foreach (List<Object> readResponseData in readResponse.Output)
            {
                claims.Add(authRequest.Claims.Type, readResponseData[0].ToString());
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

