namespace Peace.Lifelog.Security;

public class LifelogAuthenticationRequest : IAuthenticationRequest
{
    public string ModelName { get; } = "LifelogUserOTP";

    public (string Type, string Value) UserId { get; set; }
    public (string Type, string Value) Proof { get; set; }
    public (string Type, string Value) Claims { get; set; }
}
