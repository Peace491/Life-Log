using System.Security.Principal;

namespace Peace.Lifelog.Security;

public class AuthenticationResponse
{
    public bool HasError => Principal is null ? true : false;

    public bool CanRetry { get; set; }

    public AppPrincipal? Principal { get; set; }
}
