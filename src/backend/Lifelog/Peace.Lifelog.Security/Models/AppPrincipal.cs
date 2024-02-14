namespace Peace.Lifelog.Security;
using System.Security.Claims;

public class AppPrincipal
{
    public string UserId { get; set; }

    public IDictionary<string, string> Claims { get; set; }
}
