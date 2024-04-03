using DomainModels;

namespace Peace.Lifelog.Security;

public interface ILifelogAuthService
{
    public Task<AppPrincipal>? AuthenticateLifelogUser(string UserHash, string OTPHash);

    public bool IsAuthorized(AppPrincipal currentPrincipal, List<string> authorizedRoles);


}
