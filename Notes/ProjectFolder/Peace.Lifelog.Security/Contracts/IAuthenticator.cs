namespace Peace.Lifelog.Security;

public interface IAuthenticator
{
    AppPrincipal Authenticate(AuthenticationRequest authRequest); 
}

// Identity - who you are (Authentication)
// Principal - Who you are within a context (This is what authentication should return)