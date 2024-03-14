namespace Peace.Lifelog.Security;

using System.Security.Claims;
using System.Security.Principal;

public class AppPrincipal : ClaimsPrincipal
{
    public string UserId { get; set; }

    public IDictionary<string, string> Claims {get; set;}
}
