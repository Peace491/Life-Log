namespace Peace.Lifelog.Security;
using System.Security.Claims;

public class AppPrincipal
{
    public string UserId { get; set; } = string.Empty;

    public IDictionary<string, string>? Claims { get; set; }
}
