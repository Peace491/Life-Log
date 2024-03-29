﻿using back_end;

namespace Peace.Lifelog.Security;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using System.Text.Json;
using System.Security.Cryptography;
using System.Text;

public class JWTService
{
    public Jwt createJWT(HttpRequest request, AppPrincipal appPrincipal, string userHash) {
        var header = new JwtHeader();
        var payload = new JwtPayload()
        {
            Iss = request.Host.Host,
            Sub = "myApp",
            Aud = "myApp",
            Iat = DateTime.UtcNow.Ticks,
            Exp = DateTime.UtcNow.AddMinutes(20).Ticks,
            UserHash = userHash,
            Claims = appPrincipal.Claims
        };


        var serializerOptions = new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };

        var encodedHeader = Base64UrlEncode(JsonSerializer.Serialize(header, serializerOptions));
        var encodedPayload = Base64UrlEncode(JsonSerializer.Serialize(payload, serializerOptions));


        using(var hash = new HMACSHA256(Encoding.UTF8.GetBytes("simple-key")))
        {
            // String to Byte[]
            var signatureInput = $"{encodedHeader}.{encodedPayload}";
            var signatureInputBytes = Encoding.UTF8.GetBytes(signatureInput); 

            // Byte[] to String
            var signatureDigestBytes = hash.ComputeHash(signatureInputBytes);
            var encodedSignature = WebEncoders.Base64UrlEncode(signatureDigestBytes);

            var jwt = new Jwt()
            {
                Header = header,
                Payload = payload,
                Signature = encodedSignature
            };

            return jwt;
        }

    }

    private static string Base64UrlEncode(string input) 
    {
        var bytes = Encoding.UTF8.GetBytes(input);

        return WebEncoders.Base64UrlEncode(bytes); 
    }

}
