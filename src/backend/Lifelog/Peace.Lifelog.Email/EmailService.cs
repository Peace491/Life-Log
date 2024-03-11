namespace Peace.Lifelog.Email;

using MimeKit;
using MailKit.Net.Smtp;
using DomainModels;
using Peace.Lifelog.Security;
using Peace.Lifelog.DataAccess;
public class EmailService : IEmailService
{
    LifelogConfig lifelogConfig = LifelogConfig.LoadConfiguration(); 
    public async Task<Response> SendOTPEmail(string userHash)
    {
        var response = new Response();
        var readDataOnlyDao = new ReadDataOnlyDAO();
        response.HasError = false;

        var otpEmail = new MimeMessage();
        otpEmail.From.Add(new MailboxAddress("Lifelog", lifelogConfig.LifelogSystemEmail)); // Get from config
        string selectEmailSql = $"SELECT UserId FROM LifelogAccount WHERE UserHash = \"{userHash}\""; 
        var toResponse = await readDataOnlyDao.ReadData(selectEmailSql);

        string to = "";

        if (toResponse.Output != null)
        {
            foreach (List<Object> output in toResponse.Output)
            {
                to = output[0].ToString()!;
            }
        }
        

        // to is in lifelog accout or lifelog user hash table, get the email from there
        otpEmail.To.Add(new MailboxAddress("", to));
        otpEmail.Subject = "Your Lifelog OTP!";

        OTPService oTPService = new OTPService();
        var otpResponse = await oTPService.generateOneTimePassword(to);
        var body = new BodyBuilder();
        if (otpResponse.Output != null)
        {
            foreach (string otpOutput in otpResponse.Output)
            {
            body.HtmlBody = "<h1>Your Lifelog OTP is: " + otpOutput + "</h1>";
            }
        }
        
        // body.HtmlBody = "<h1>Your Lifelog OTP is: " + otpResponse.Output + "</h1>";
        otpEmail.Body = body.ToMessageBody();

        try
        {
            var emailResponse = SendEmail(otpEmail);
        }
        catch
        {
            response.HasError = true;
            response.ErrorMessage = "Error sending email";
        }
        return response;
    }

    // Created method for usage in both otp email and Lifelog Reminder Feature
    private Response SendEmail(MimeMessage email)
        
        {
        var response = new Response();
        using (var client = new SmtpClient())
        {
            try
            {
                client.Connect("smtp.gmail.com", 587, false); // Use SSL and correct port
                client.Authenticate(lifelogConfig.LifelogSystemEmail, lifelogConfig.LifelogSystemEmailAppPassword);
                client.Send(email);
                client.Disconnect(true);
                response.HasError = false;
            }
            catch (Exception ex)
            {
                response.HasError = true;
                response.ErrorMessage = ex.Message;
            }
        }
        return response;
    }
}

