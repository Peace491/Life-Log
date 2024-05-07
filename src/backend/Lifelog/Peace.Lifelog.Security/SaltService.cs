namespace Peace.Lifelog.Security;
using DomainModels;
using System.Security.Cryptography;
public class SaltService : ISaltService
{
    private static int saltLength = 32;
    public Response getSalt()
    {
        var response = new Response();
        try
        {
#pragma warning disable SYSLIB0023 // Type or member is obsolete
            var rng = new RNGCryptoServiceProvider();
#pragma warning restore SYSLIB0023 // Type or member is obsolete
            byte[] saltArray = new byte[saltLength];

            rng.GetBytes(saltArray);

            // Convert the byte array to a hexadecimal string representation
            string saltHex = BitConverter.ToString(saltArray).Replace("-", "");

            // Truncate the hexadecimal string to the desired length
            string salt = saltHex.Substring(0, 32);

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
