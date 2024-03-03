namespace RegistrationService;

using DomainModels;
using Peace.Lifelog.UserManagement;
using Peace.Lifelog.UserManagementTest;
using Peace.Lifelog.DataAccess;
using System.Diagnostics; 
using Peace.Lifelog.Logging;
using Peace.Lifelog.Security;
using System;
using System.Text.RegularExpressions;
using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;



public class RegistrationService()
{

    public async Task<Response> CheckInputValidation(string email, string DOB, string zipCode)
    {
        var response = new Response();
        
        // valid email using regex
        string validEmailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";

        if(!Regex.IsMatch(email, validEmailPattern)){

            /*// log: Log Level: Warning
            Log Category: Data
            Log Message: “The email is not in the correct format.”*/


            response.HasError = true;
            response.ErrorMessage = "The email is not in the correct format.";
            return response;
        }

        string threeCharsPattern = @"^.{1,3}@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";

        if(!Regex.IsMatch(email, threeCharsPattern)){

            /*Log Level: Warning
            Log Category: Data
            Log Message: “The email is not in the correct format.”*/


            response.HasError = true;
            response.ErrorMessage = "The email is too short.";
            return response;
        }

        string alphanumericCharsPattern = @"^[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";

        if(!Regex.IsMatch(email, alphanumericCharsPattern)){

           /* Log Level: Warning
            Log Category: Data
            Log Message: “The email is too short.”*/

            response.HasError = true;
            response.ErrorMessage = "The email is not alphanumeric.";
            return response;
        }

        bool createDateTrue = DateTime.TryParseExact(DOB, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out DateTime date);

        DateTime lowerBound = new DateTime(1970, 1, 1);
        DateTime upperBound = DateTime.Now.Date;

        if(date < lowerBound && date > upperBound){

            /*Log Level: Warning
            Log Category: Data
            Log Message: “The birth date is invalid.”*/


            response.HasError = true;
            response.ErrorMessage = "The birth date is invalid.";
            return response;
        }

        // TODO: check if any zipcode outside of LA county then return response has error
        var readOnlyDAO = new ReadDataOnlyDAO();
        var readAllZipCodeSQL = $"SELECT ZipCode FROM californiazipcode";
        int RECORD_COUNT = 311; // hard coded value 

        var readZipCodeResponse = await readOnlyDAO.ReadData(readAllZipCodeSQL, RECORD_COUNT, 0);

        foreach (List<Object> readResponseData in readZipCodeResponse.Output)
        {
            if (readResponseData[0].ToString() != zipCode)
            {
                /*Log Level: Warning
                Log Category: Data
                Log Message: “The zip code is invalid.”*/

                response.HasError = true;
                response.ErrorMessage = "The birth date is invalid.";
                return response;
            }
        }




        if (String.IsNullOrEmpty(email) || String.IsNullOrEmpty(DOB) || String.IsNullOrEmpty(zipCode)){

            /*Log Level: Warning
            Log Category: Data
            Log Message: “The non-nullable option is null.”*/


            response.HasError = true;
            response.ErrorMessage = "The non-nullable option is null.";
            return response;
        }

        // send a confirmation email to the user using mailkit, after user confirms then create user
        SendOTPEmail(email);

        // return response 
        response.HasError = false;
        return response;

    }


    public static void SendOTPEmail(string email){ // Should probably be in Security Library

        string lifelogEmail = "";   // add lifelog email and pass
        string lifelogPassword = "";

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("Lifelog", lifelogEmail));
        message.To.Add(new MailboxAddress("", email));
        message.Subject = "Your OTP for Lifelog!";

        // TODO: create OTP generator ??
        string registrationOTP = "";

        var body = new BodyBuilder();
        body.HtmlBody = $"<p>Thank you for registering! Please use this OTP {registrationOTP} to confirm your registration.</p>";

        message.Body = body.ToMessageBody();

        using (var emailClient = new SmtpClient()){
            emailClient.Connect("smtp.gmail.com", 465, SecureSocketOptions.Auto);

            emailClient.Authenticate (lifelogEmail, lifelogPassword);

            emailClient.Send(message);

            emailClient.Disconnect (true);
        }

    }

    public void RegisterNormalUser(string email, string DOB, string zipCode){
        RegisterUser(email, DOB, zipCode, "Normal");
    }

    public void RegisterAdminUser(string email, string DOB, string zipCode){
        RegisterUser(email, DOB, zipCode, "Admin");
    }


    public async void RegisterUser(string email, string DOB, string zipCode, string userRole){

        // TODO: Check OTP before doing register user

        string userID = email;  
        string role = userRole;
        
        // uses usermanagementtest
        var accountRequest = new LifelogAccountRequest();
        var profileRequest = new LifelogProfileRequest();

        accountRequest.UserId = ("UserId", userID);
        accountRequest.Role = ("Role", role);

        profileRequest.DOB = ("DOB", DOB);
        profileRequest.ZipCode = ("ZipCode", zipCode);

        var userManagementService = new LifelogUserManagementService();
        var createLifelogUserResponse = await userManagementService.CreateLifelogUser(accountRequest, profileRequest);

        // add success log 
        /*Timestamp
        Log Level: Info
        Log Category: Persistent Data Store
        Log Message: “User registration successful.”*/


    }

}