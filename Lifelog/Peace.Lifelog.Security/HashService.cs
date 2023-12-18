using DomainModels;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Policy;

namespace Peace.Lifelog.Security;

public class HashService : IHasher
{
    /// <summary>
    /// Hash plaintext
    /// </summary>
    /// <param name="plaintext"></param>
    /// <returns></returns>
    public Response Hasher(string plaintext)
    {
        var response = new Response();
        try
        {
            // salt protocol will be to concat the salt onto the input string.
            // ^ hasher will not know abt protocols of fields being hashed this way.
            byte[] salt = new byte[0];
            string hash = Convert.ToBase64String(KeyDerivation.Pbkdf2(plaintext,
                                                                        salt: salt,
                                                                        prf: KeyDerivationPrf.HMACSHA256,
                                                                        iterationCount: 100000,
                                                                        numBytesRequested: 256 / 8));
            response.HasError = false;
            response.Output = [hash];
            return response;
        }
        catch (ArgumentNullException)
        {
            response.HasError = true;
            response.ErrorMessage = "Password is null";
            return response;
        }
    }

}
