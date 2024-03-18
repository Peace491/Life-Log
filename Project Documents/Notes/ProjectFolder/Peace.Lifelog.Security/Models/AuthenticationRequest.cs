namespace Peace.Lifelog.Security;

public class AuthenticationRequest
{
    public string UserId { get; set; } = string.Empty;

    public string Proof { get; set; } = string.Empty;

}

public class AccountDetails
{
    public string UserId { get; set; } //email

    public IDictionary<string, string> Details {get; set;} // Everything else (Zip Code, DOB, ...)
}