using ZstdSharp.Unsafe;
using Peace.Lifelog.Infrastructure;
using Microsoft.AspNetCore.Authorization.Infrastructure;
namespace Peace.Lifelog.Security;

public class LifelogAuthService : ILifelogAuthService
{
    private readonly AppAuthService appAuthService;
    private readonly UserManagmentRepo userManagementRepo;

    public LifelogAuthService(AppAuthService appAuthService, UserManagmentRepo userManagementRepo)
    {
        this.appAuthService = appAuthService;
        this.userManagementRepo = userManagementRepo;
    }
    public async Task<AppPrincipal>? AuthenticateLifelogUser(string UserHash, string OTPHash)
    {
        if (UserHash == string.Empty) {
            return null!;
        }
        if (OTPHash == string.Empty) {
            return null!;
        }

        var lifelogAuthenticationRequest = new LifelogAuthenticationRequest();

        lifelogAuthenticationRequest.UserId = ("UserHash", UserHash);
        lifelogAuthenticationRequest.Proof = ("OTPHash", OTPHash);
        lifelogAuthenticationRequest.Claims = ("Role", "");

        var appAuthService = new AppAuthService();

        #pragma warning disable CS8602 // Dereference of a possibly null reference.
        AppPrincipal? principal = await appAuthService.AuthenticateUser(lifelogAuthenticationRequest)!;
        #pragma warning restore CS8602 // Dereference of a possibly null reference.

        if (principal != null) 
        {
            _ = userManagementRepo.UpdateUserFirstLogin(UserHash);
        }
        

        return principal!;
    }

    public bool IsAuthorized(AppPrincipal currentPrincipal, List<string> authorizedRoles) {
        var appAuthService = new AppAuthService();
        
        bool isAuthorize = false;

        if (currentPrincipal.Claims == null || !currentPrincipal.Claims.ContainsKey("Role")) {
            return false;
        }

        foreach (string role in authorizedRoles) {
            if (currentPrincipal.Claims["Role"] == role) {
                var requiredClaims = new Dictionary<string, string>() {{"Role", role}};
                isAuthorize = appAuthService.IsAuthorize(currentPrincipal, requiredClaims);
            }
        }

        return isAuthorize;

    }

}
