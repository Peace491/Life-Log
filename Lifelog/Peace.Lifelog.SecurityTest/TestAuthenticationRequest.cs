using Peace.Lifelog.Security;

namespace Peace.Lifelog.SecurityTest;

public class TestAuthenticationRequest : IAuthenticationRequest
{
    public string ModelName { get; } = "TestAuthentication";
    public (string Type, string Value) UserId { get; set; } 
    public (string Type, string Value) Proof { get; set; }
    public (string Type, string Value) Claims { get; set; }
}
