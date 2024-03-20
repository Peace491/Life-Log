namespace Peace.Lifelog.Security;

using DomainModels;
using Peace.Lifelog.DataAccess;

public class OTPService: IOTPService
{
    private Random random = new Random();
    public async Task<Response> generateOneTimePassword(string userHash)
    {
        var response = new Response();
        var updateDataOnlyDAO = new UpdateDataOnlyDAO();
        // Hashed otp or not?
        
        response.HasError = false;
        try
        {
            const string validChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

            char [] oneTimePassword = new char[8];
            
            for (int i = 0; i < oneTimePassword.Length; i++)
            {
                oneTimePassword[i] = validChars[random.Next(0, validChars.Length)];
            }

            string str = new string(oneTimePassword);
            
            string updateSQL = $"UPDATE LifelogUserOTP " +
                                $"SET OTPHash = \"{str}\", Timestamp = NOW() " +
                                $"WHERE UserHash = \"{userHash}\"";

            string updateAuthenticationSQL = $"UPDATE LifelogAuthentication " +
                                $"SET OTPHash = \"{str}\" " +
                                $"WHERE UserHash = \"{userHash}\"";
            
            var temp = await updateDataOnlyDAO.UpdateData(updateSQL);
            var updateAuthenticationResponse = await updateDataOnlyDAO.UpdateData(updateAuthenticationSQL);
            
            response.Output = [str];
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
