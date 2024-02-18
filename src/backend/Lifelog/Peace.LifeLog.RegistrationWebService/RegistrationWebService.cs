namespace RegistrationWebService;

using DomainModels;
using Peace.Lifelog.UserManagement;
using Peace.Lifelog.Logging;
using System;
using System.Text.RegularExpressions;

public class RegistrationWebService()
{


    public async Task<Response> RegisterUser(string email, string DOB, string zipCode, string userRole)
    {
        var response = new Response();
        
        // valid email using regex
        string regexPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";

        if(Regex.IsMatch(email, regexPattern)){

            response.HasError = true;
            response.ErrorMessage = "LLI Title is too long";
            return response;
        }



    }







}