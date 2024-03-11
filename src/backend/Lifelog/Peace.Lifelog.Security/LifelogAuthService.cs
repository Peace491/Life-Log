namespace Peace.Lifelog.Security;

public class LifelogAuthService
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

}
