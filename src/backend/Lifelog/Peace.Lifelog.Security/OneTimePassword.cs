namespace Peace.Lifelog.Security;

using DomainModels;

public class OneTimePassword: IOneTimePassword
{

    public Response generateOneTimePassword(string userHash)
    {
        var response = new Response();
        var random = new Random();

        response.HasError = false;
        try
        {
            const string validChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

            char [] oneTimePassword = new char[8];
            
            for (int i = 0; i < oneTimePassword.Length; i++)
            {
                oneTimePassword[i] = validChars[random.Next(0, validChars.Length)];
            }

            // TODO: Write to LifelogUserOTP table, using an insert on dupe key update, with passed userHash, otp, and sql date now.
            
            response.Output = [oneTimePassword.ToString()];
            
            return response;
        }
        catch (Exception)
        {
            response.HasError = true;
            response.ErrorMessage = "Error generating one time password";
            return response;
        }
    }
}
