namespace Peace.Lifelog.Security;

public class LifelogAuthService : ILifelogAuthService
{
    public Task<AppPrincipal>? AuthenticateLifelogUser(string UserHash, string OTPHash)
    {
        if (UserHash == string.Empty) {
            return null;
        }
        if (OTPHash == string.Empty) {
            return null;
        }

        var lifelogAuthenticationRequest = new LifelogAuthenticationRequest();
        lifelogAuthenticationRequest.UserId = ("UserHash", UserHash);
        lifelogAuthenticationRequest.Proof = ("OTPHash", OTPHash);
        lifelogAuthenticationRequest.Claims = ("Role", "");

        var appAuthService = new AppAuthService();

        var principal = appAuthService.AuthenticateUser(lifelogAuthenticationRequest);

        return principal;
    }

    public bool IsAuthorized(AppPrincipal currentPrincipal, List<string> authorizedRoles) {
        var appAuthService = new AppAuthService();
        

        bool isAuthorize = false;

        if (currentPrincipal.Claims == null || !currentPrincipal.Claims.ContainsKey("RoleName")) {
            return false;
        }

        foreach (string role in authorizedRoles) {
            if (currentPrincipal.Claims["RoleName"] == role) {
                var requiredClaims = new Dictionary<string, string>() {{"RoleName", role}};
                isAuthorize = appAuthService.IsAuthorize(currentPrincipal, requiredClaims);
            }
        }

        return isAuthorize;

    }

}
