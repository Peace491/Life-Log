using System.Security.Cryptography;
using System.Text;

using DomainModels;

using System.Security.Principal; // This is built in, we can use some of the objects, like Principle

namespace Peace.Lifelog.Security;

// Security library should have a random number generator. Cant be own or built in
// public class RandomValue
// {
//     // byte[256] key;

//     public static byte[] GenerateRandom(int size)
//     {
//         var rng = RandomNumberGenerator.GetBytes(size);

//         // Don't use Random, because it is pseudorandom, can be predicted

//         // return Encoding.UTF8.GetString(rng); // Still need to convert to int

//     }

// }

public class AppAuthService : IAuthenticator, IAuthorizer
{
    // public IPrincipal Authenticate(IIdentity id) {

    // }

    /// <summary>
    /// For authentication
    /// </summary>
    /// <param name="authRequest"></param>
    /// <returns>AppPrinciple is null, then it fails, otherwise it is good</returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentException"></exception>
    public AppPrincipal Authenticate(AuthenticationRequest authRequest) // We need to identify identity and principle
    {
        // Early exit
        #region  Validate arguments (value of parameter)
        if (authRequest is null)
        {
            throw new ArgumentNullException(nameof(authRequest));
        }

        if (string.IsNullOrWhiteSpace(authRequest.UserId))
        {
            throw new ArgumentException($"{nameof(authRequest.UserId)} must be valid");
        }
        
        if (string.IsNullOrWhiteSpace(authRequest.Proof))
        {
            throw new ArgumentException($"{nameof(authRequest.Proof)} must be valid");
        }

        #endregion

        AppPrincipal appPrincipal = null;

        try // Library should protect against failure 
        {
            // Step 1: Validate auth request (Go to database to see if it match up)

            // Step 2: Populate app principal object
            var claims = new Dictionary<string, string>();

            appPrincipal = new AppPrincipal()
            {
                UserId = authRequest.UserId,
                Claims = claims
            };
        } 
        catch (Exception ex)
        {
            var errorMessage = ex.GetBaseException().Message; // First error that trigger
            // logging errorMessage // Async
        }


        return appPrincipal;
    }


    // Call by every functionality
    public bool IsAuthorize(AppPrincipal currentPrincipal, 
                            IDictionary<string, string> requiredClaims) 
    {
        // Dictionary<string, string>() {new ("RoleName", "Admin")}
        // key - RoleName, value - Admin

        foreach (var claim in requiredClaims)
        {
            if (!currentPrincipal.Claims.Contains(claim)) 
            {
                return false;
            }
        }

        return true;
        
    }

}

// // example of how to use
// public class UploadService()
// {
//     private readonly AppAuthService _appAuthService;
//     public UploadService(AppAuthService authService)
//     {
//         _appAuthService = authService;
//     }
//     public void Upload()
//     {
//         var requiredClaims = new Dictionary<string, string>()
//         {
//             new KeyValuePair<string, string>(key: "RoleName", value: "Admin")
//         };

//         var authRequest = new AuthenticationRequest();
//         var principal = _appAuthService.Authenticate(authRequest);

//         if (_appAuthService.IsAuthorize(principal, requiredClaims))
//         {
//             // Upload
//             UploadFile();
//         }
//         else 
//         {

//         }
//     }

//     private void UploadFile() {

//     }
// }

