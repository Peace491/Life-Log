namespace Peace.Lifelog.Security;

public class AuthenticationRequest
{
    public string UserId { get; set; } = string.Empty;
    public string Proof { get; set; } = string.Empty;
    public AuthenticationSQLDetails AuthSQLDetails { get; set; }
}

