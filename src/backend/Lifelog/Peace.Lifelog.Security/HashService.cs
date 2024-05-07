using DomainModels;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;


namespace Peace.Lifelog.Security;

/// <summary>
/// HashService, containing methods relating to hashing, namely "Hasher"
/// </summary>
public class HashService : IHashService
{
    /// <summary>
    /// Takes in plaintext and returns hash of the plaintext. 
    /// If salt or pepper is involved in the hashing of the input, concatenate it onto the input before passing it to Hasher.
    /// Catches null input as ArgumentNullException
    /// </summary>
    /// <param name="plaintext"></param>
    /// <returns></returns>
    public Response Hasher(string plaintext)
    {
        var response = new Response();
        try // Protect vs failure
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
            response.ErrorMessage = "plaintext input is null";
            return response;
        }
    }

}
