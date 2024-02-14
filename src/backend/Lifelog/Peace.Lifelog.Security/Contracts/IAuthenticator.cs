namespace Peace.Lifelog.Security;
/// <summary>
/// IAuthenticator Interface
/// </summary>
public interface IAuthenticator
{
    /// <summary>
    /// Authenticates a user using an authentication request
    /// </summary>
    /// <param name="authRequest"></param>
    /// <returns></returns>
    Task<AppPrincipal>? AuthenticateUser(IAuthenticationRequest authRequest);
}
