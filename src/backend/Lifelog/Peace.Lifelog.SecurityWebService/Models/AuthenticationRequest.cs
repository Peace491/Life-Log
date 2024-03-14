namespace Peace.Lifelog.SecurityWebService;

public class AuthenticationRequest
{
    public string UserHash { get; set; } = string.Empty;
    public string OTP { get; set; } = string.Empty;

}
