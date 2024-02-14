namespace Peace.Lifelog.Security;
using DomainModels;
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
