namespace Peace.Lifelog.Security;

using back_end;
using Microsoft.AspNetCore.Http;
public interface IJWTService
{
    Jwt createJWT(HttpRequest request, AppPrincipal appPrincipal, string userHash);
    bool IsJwtValid(Jwt jwt);
    int ProcessToken(HttpRequest request);
}
