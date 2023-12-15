namespace Peace.Lifelog.Security;

public abstract class BaseAuthenticationRequest
{
    public abstract string ModelName { get; }
    public (string Type, string Value) UserId { get; set; } = ("", "");
    public (string Type, string Value) Proof { get; set; } = ("", "");
    public (string Type, string Value) Claims { get; set; } = ("", "");
}

