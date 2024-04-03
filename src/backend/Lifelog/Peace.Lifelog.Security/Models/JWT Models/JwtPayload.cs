using System.Security.Claims;

namespace back_end
{

    public class JwtPayload
    {
        public string Iss {get; set;} = string.Empty;
        public string Sub {get; set;} = string.Empty;
        public string Aud {get; set;} = string.Empty;
        public string Exp {get; set;} = string.Empty;
        public string Iat {get; set;} = string.Empty;


        public long? Nbf {get; set;}

        public string? Scope {get; set;} = String.Empty;
        public string? UserHash {get; set;} = string.Empty;

        public IDictionary<string, string>? Claims {get; set;}

    }
}