namespace Peace.Lifelog.SecurityWebService;

public class AuthenticationRequest
{
    public string UserId { get; set; } = string.Empty;
    public string OTP { get; set; } = string.Empty;

}
