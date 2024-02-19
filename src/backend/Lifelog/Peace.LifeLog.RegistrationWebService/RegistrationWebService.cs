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



    }







}