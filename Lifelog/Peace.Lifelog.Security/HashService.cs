using DomainModels;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Policy;

namespace Peace.Lifelog.Security;

public class HashService : IHasher
{
    // Task<Response>
    public string Hasher(string plaintext)
    {
        var response = new Response();
        try
        {
            byte[] salt = new byte[0];
            // TODO
            string hash = Convert.ToBase64String(KeyDerivation.Pbkdf2(plaintext,
                                                                        salt: salt,
                                                                        prf: KeyDerivationPrf.HMACSHA256,
                                                                        iterationCount: 100000,
                                                                        numBytesRequested: 256 / 8));
            // hash operation will be async, could have high cost on system resources.
            // -> after reserach, it cant be async?????
            // return response;
            return hash;
        }
        catch
        {
            string hash = String.Empty;
            return hash;
        }
    }

}
