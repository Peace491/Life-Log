namespace Peace.Lifelog.Security;

public interface IAuthenticator
{
    Task<AppPrincipal>? AuthenticateUser(IAuthenticationRequest authRequest);
}
