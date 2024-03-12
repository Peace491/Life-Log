namespace Peace.Lifelog.Security;

public interface IAuthorizer
{
    bool IsAuthorize(AppPrincipal currentPrincipal, IDictionary<string, string> requiredClaims);
}