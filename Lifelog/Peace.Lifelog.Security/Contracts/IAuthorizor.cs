namespace Peace.Lifelog.Security;
/// <summary>
/// IAuthorizor Interface
/// </summary>
public interface IAuthorizor
{
    /// <summary>
    /// Checks if a user with the current claims is able to use a funcitonality with required claims.
    /// </summary>
    /// <param name="currentPrincipal"></param>
    /// <param name="requiredClaims"></param>
    /// <returns></returns>
    bool IsAuthorize(AppPrincipal currentPrincipal, IDictionary<string, string> requiredClaims);
}
