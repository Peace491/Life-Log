namespace RegistrationService;

using DomainModels;
using Peace.Lifelog.UserManagement;
using Peace.Lifelog.Logging;
using System;
using System.Text.RegularExpressions;
using MimeKit;
using MailKit.Net.Smtp;

public class RegistrationService()
{


    public async Task<Response> RegisterUser(string email, string DOB, string zipCode, string userRole)
    {
        var response = new Response();
        
        // valid email using regex
        string validEmailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";

        if(!Regex.IsMatch(email, validEmailPattern)){

            response.HasError = true;
            response.ErrorMessage = "The email is not in the correct format.";
            return response;
        }

        string threeCharsPattern = @"^.{1,3}@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";

        if(!Regex.IsMatch(email, threeCharsPattern)){

            response.HasError = true;
            response.ErrorMessage = "The email is too short.";
            return response;
        }

        string alphanumericCharsPattern = @"^[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";

        if(!Regex.IsMatch(email, alphanumericCharsPattern)){

            response.HasError = true;
            response.ErrorMessage = "The email is not alphanumeric.";
            return response;
        }

        bool createDateTrue = DateTime.TryParseExact(DOB, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out DateTime date)

        DateTime lowerBound = new DateTime(1970, 1, 1);
        DateTime upperBound = DateTime.Now.Date;

        if(date < lowerBound && date > upperBound){
            response.HasError = true;
            response.ErrorMessage = "The birth date is invalid.";
            return response;
        }

        // implement check if any zipcode outside of LA county then return response has error




        if(email == "" || DOB == "" || zipCode == ""){
            response.HasError = true;
            response.ErrorMessage = "The non-nullable option is null.";
            return response;
        }

        // send a confirmation email to the user using mailkit, after user confirms then create user


        // create the Normal user


    }


    public async void SendEmail(string email){

        string lifelogEmail = "peacesuperuser1@gmail.com";   // add lifelog email and pass
        string lifelogPassword = "Peacesuperuser1$";

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("Lifelog", lifelogEmail));
        message.To.Add(new MailboxAddress("", email));
        message.Subject = "Your OTP for Lifelog!"

        //  Create a OTP for our account
        string registrationOTP = "";

        var body = new BodyBuilder();
        body.HtmlBody = $"<p>Thank you for registering! Please use this OTP {registrationOTP} to confirm your registration.</p>";

        message.Body = body.ToMessageBody();

        using (var emailClient = new SmtpClient()){
            emailClient.Connect("smtp.gmail.com", 465, SecureSocketOptions.SslOnConnect);

            client.Authenticate (lifelogEmail, lifelogPassword);

            client.Send(message);

            client.Disconnect (true);
        }

    }







}