namespace Peace.Lifelog.Security;

public interface IAuthorizor
{
    bool IsAuthorize(AppPrincipal currentPrincipal, IDictionary<string, string> requiredClaims);
}
