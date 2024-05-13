namespace Peace.Lifelog.RegistrationService;

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
using Peace.Lifelog.Infrastructure;


public class RegistrationService
{
    private readonly ILifelogUserManagementService lifelogUserManagementService;
    private readonly ILogging logger;
    public RegistrationService(ILifelogUserManagementService lifelogUserManagementService, ILogging logger)
    {
        this.lifelogUserManagementService = lifelogUserManagementService;
        this.logger = logger;
    }

    public async Task<Response> CheckInputValidation(string email, string DOB, string zipCode)
    {
        var response = new Response();
        string logHash = "System";

        bool createDateTrue = DateTime.TryParseExact(DOB, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out DateTime date);

        DateTime lowerBound = new DateTime(1970, 1, 1);
        DateTime upperBound = DateTime.Now.Date;

        // if (DOBYear < lowerBoundYear && DOBYear > upperBoundYear)
        if (date < lowerBound || date > upperBound)
        {

            /*Log Level: Warning
            Log Category: Data
            Log Message: �The birth date is invalid.�*/
            response.HasError = true;
            response.ErrorMessage = "The birth date is invalid.";

            await logger.CreateLog("Logs", logHash, "Warning", "Data", "User registration failed: " + response.ErrorMessage);

            return response;
        }




        // #3
        string threeCharsPattern = @"^.{1,3}@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";

        if (Regex.IsMatch(email, threeCharsPattern))
        {
            /*Log Level: Warning
            Log Category: Data
            Log Message: �The email is not in the correct format.�*/
            response.HasError = true;
            response.ErrorMessage = "The email is too short.";

            await logger.CreateLog("Logs", logHash, "Warning", "Data", "User registration failed: " + response.ErrorMessage);

            return response;
        }


        // #2
        string alphanumericCharsPattern = @"^[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";

        if (!Regex.IsMatch(email, alphanumericCharsPattern))
        {

            /* Log Level: Warning
             Log Category: Data
             Log Message: �The email is too short.�*/
            response.HasError = true;
            response.ErrorMessage = "The email is not alphanumeric.";

            await logger.CreateLog("Logs", logHash, "Warning", "Data", "User registration failed: " + response.ErrorMessage);

            return response;
        }

        // valid email using regex #1
        string validEmailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";

        if (!Regex.IsMatch(email, validEmailPattern))
        {

            /*// log: Log Level: Warning
            Log Category: Data
            Log Message: �The email is not in the correct format.�*/
            response.HasError = true;
            response.ErrorMessage = "The email is not in the correct format.";

            await logger.CreateLog("Logs", logHash, "Warning", "Data", "User registration failed: " + response.ErrorMessage);

            return response;
        }

        // return response 
        response.HasError = false;
        return response;

    }

    public async Task<Response> RegisterNormalUser(string email, string DOB, string zipCode, string ip)
    {
        var response = await RegisterUser(email, DOB, zipCode, "Normal", ip);
        return response;

    }

    public async Task<Response> RegisterAdminUser(string email, string DOB, string zipCode)
    {
        var response = await RegisterUser(email, DOB, zipCode, "Admin", "");
        return response;

    }


    public async Task<Response> RegisterUser(string email, string DOB, string zipCode, string userRole, string ip)
    {
        var response = new Response();
        string userID = email;


        // uses usermanagementtest
        var accountRequest = new LifelogAccountRequest();
        var profileRequest = new LifelogProfileRequest();

        accountRequest.UserId = ("UserId", userID);
        accountRequest.Role = ("Role", userRole);

        profileRequest.DOB = ("DOB", DOB);
        profileRequest.ZipCode = ("ZipCode", zipCode);

        
        var createLifelogUserResponse = await lifelogUserManagementService.CreateLifelogUser(accountRequest, profileRequest);

        if (createLifelogUserResponse.HasError == true)
        {
            response.HasError = true;
            response.ErrorMessage = createLifelogUserResponse.ErrorMessage;

            await logger.CreateLog("Logs", "System", "Warning", "Data", ip + " registration failed: " + response.ErrorMessage);

            return response;
        }

        // add success log 
        /*Timestamp
        Log Level: Info
        Log Category: Persistent Data Store
        Log Message: �User registration successful.�*/
        response.HasError = createLifelogUserResponse.HasError;
        response.Output = createLifelogUserResponse.Output;

        string userHash = "";
        if (createLifelogUserResponse.Output is not null)
        {
            foreach (string output in createLifelogUserResponse.Output)
            {
                userHash = output;
            }
        }

        // Function to clear if first login is still zero after 2 mins
        _ = ClearFirstLogin(accountRequest, profileRequest);

        await logger.CreateLog("Logs", userHash, "Info", "Persistent Data Store", "User registration successful");

        return response;
    }

    // Helper function to clear if first login doesnt happen
    public async Task<Response> ClearFirstLogin(LifelogAccountRequest acc, LifelogProfileRequest prof)
    {
        return await lifelogUserManagementService.CheckSuccessfulReg(acc, prof);
    }
    
}