using Peace.Lifelog.Security;

namespace Peace.Lifelog.SecurityTest;

public class TestAuthenticationRequest : BaseAuthenticationRequest
{
    public override string ModelName { get; } = "TestAppAuthService";
}
