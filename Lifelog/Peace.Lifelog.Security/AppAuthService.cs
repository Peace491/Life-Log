namespace Peace.Lifelog.Security;

public class AppAuthService
{
    AppPrincipal AuthenticateUser(AuthenticationRequest authRequest)
    {
        throw new NotImplementedException();

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

