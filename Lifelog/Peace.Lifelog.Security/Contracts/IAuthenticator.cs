namespace Peace.Lifelog.Security;

public interface IAuthenticator
{
    Task<AppPrincipal>? AuthenticateUser(
        AuthenticationRequest authRequest, 
        string table, 
        string userIdColumnName, 
        string proofColumnName, 
        string claimColumnName);
}
