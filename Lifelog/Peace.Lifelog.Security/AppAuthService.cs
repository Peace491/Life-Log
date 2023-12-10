namespace Peace.Lifelog.Security;

using Peace.Lifelog.DataAccess;

public class AppAuthService : IAuthenticator, IAuthorizor
{
    public async Task<AppPrincipal>? AuthenticateUser(
        AuthenticationRequest authRequest, 
        string table, 
        string userIdColumnName, 
        string proofColumnName, 
        string claimColumnName)
    {
        // Early exit
        #region  Validate arguments (value of parameter)
        if (authRequest is null)
        {
            throw new ArgumentNullException(nameof(authRequest));
        }

        if (string.IsNullOrWhiteSpace(authRequest.UserId))
        {
            throw new ArgumentException($"{nameof(authRequest.UserId)} must be valid");
        }
        
        if (string.IsNullOrWhiteSpace(authRequest.Proof))
        {
            throw new ArgumentException($"{nameof(authRequest.Proof)} must be valid");
        }

        #endregion

        AppPrincipal? appPrincipal = null;

        try // Library should protect against failure 
        {
            // Step 1: Validate auth request (Go to database to see if it match up)
            var readDataOnlyDAO = new ReadDataOnlyDAO();

            var getClaimsSql = $"SELECT {claimColumnName} FROM {table} WHERE {userIdColumnName} = {authRequest.UserId} AND {proofColumnName} = {authRequest.Proof}";

            var readResponse = await readDataOnlyDAO.ReadData(getClaimsSql);

            // Step 2: Populate app principal object
            var claims = new Dictionary<string, string>() {};

            foreach (List<Object> readResponseData in readResponse.Output)
            {
                claims.Add(claimColumnName, readResponseData[0].ToString());
            }

            appPrincipal = new AppPrincipal()
            {
                UserId = authRequest.UserId,
                Claims = claims
            };
        } 
        catch (Exception ex)
        {
            var errorMessage = ex.GetBaseException().Message; // First error that trigger
            // logging errorMessage // Async
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

        foreach (var claim in requiredClaims)
        {
            if (!currentPrincipal.Claims.Contains(claim)) 
            {
                return false;
            }
        }

        return true;
        
    }

}

