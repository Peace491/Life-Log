namespace Peace.Lifelog.Security;

public interface IAuthenticator
{
    AppPrincipal AuthenticateUser(AuthenticationRequest authRequest);
}
