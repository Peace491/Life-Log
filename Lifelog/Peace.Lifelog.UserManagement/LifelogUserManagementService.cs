using DomainModels;
using Peace.Lifelog.UserManagementTest;
using Peace.Lifelog.Security;
using Peace.Lifelog.DataAccess;

namespace Peace.Lifelog.UserManagement;

public class LifelogUserManagementService : ICreateLifelogUser
{
    public async Task<Response> CreateLifelogUser(LifelogAccountRequest accountRequest, LifelogProfileRequest profileRequest)
    {
        var userHash = "";    
        var response = new Response();    

        // Create Lifelog User Hash
        var hashService = new HashService();
        var appUserManagementService = new AppUserManagementService();
        var createDataOnlyDAO = new CreateDataOnlyDAO();

        // TODO: POPULATE 'accountRequest' salt

        // TODO: IMPLEMENT SALT FUNC


        var hashResponse = hashService.Hasher(accountRequest.UserId + /*accountRequest.Salt*/ "badsalt"); 

        if (hashResponse.Output is not null)
        {
            foreach (String hashOutput in hashResponse.Output)
            {
                accountRequest.UserHash = ("UserHash", hashOutput);
                userHash = hashOutput;
            }
        }

        Response createAccountResponse = await appUserManagementService.CreateAccount(accountRequest);
        
        if (createAccountResponse.HasError == true) 
        {
            // TODO: HANDLE ERROR
            response.HasError = false;
            response.ErrorMessage = "Failed to create Account table entry";
            return response;
        }

        // Define SQL
        // TODO: store SQL elsewhere on proj in  playbook
        var createUserHashSql = $"INSERT INTO LifelogUserHash (UserId, UserHash) VALUES (\"{accountRequest.UserId}\", \"{accountRequest.UserHash}\");";
        var createLifelogProfileSql = $"INSERT INTO LifelogProfile VALUES (\"{userHash}\", \"{profileRequest.DOB}\", \"{profileRequest.ZipCode}\");";  

        // Populate user hash and user profile tables
        Response createUserHashResponse = await createDataOnlyDAO.CreateData(createUserHashSql);

        if (createUserHashResponse.HasError == true) 
        {
            // TODO: HANDLE ERROR
            response.HasError = false;
            response.ErrorMessage = "Failed to create UserHash table entry";
            return response;
        }

        Response createLifelogProfileResponse = await createDataOnlyDAO.CreateData(createLifelogProfileSql); 

        if (createLifelogProfileResponse.HasError == true) 
        {
            // TODO: HANDLE ERROR
            response.HasError = false;
            response.ErrorMessage = "Failed to create LifelogProfle";
            return response;
        }
        
        // TODO: handle success outcome
        response.HasError = false;
        response.Output = ["This worked"];
        return response;
    }
}
