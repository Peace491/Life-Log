namespace Peace.Lifelog.Security;
using DomainModels;
using Microsoft.VisualBasic;
using Org.BouncyCastle.Crypto.Signers;
using System.ComponentModel;
using System.Security.Cryptography;
public class SaltService
{
    private static int saltLength = 32;
    public Response getSalt()
    {
        var response = new Response();
        try 
        {
            var rng = new RNGCryptoServiceProvider();
            byte[] saltArray = new byte[saltLength];

            rng.GetBytes(saltArray);
            var salt = Convert.ToBase64String(saltArray);
            
            response.HasError = false;
            response.Output = [salt];
            return response;
        } 
        catch (Exception)
        {
            response.HasError = true;
            response.ErrorMessage = "getSalt encountered an issue";
            return response;
        }

    }
}
