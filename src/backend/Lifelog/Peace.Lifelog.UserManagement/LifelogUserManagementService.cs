using DomainModels;
using Peace.Lifelog.DataAccess;
using Peace.Lifelog.Security;
using Peace.Lifelog.UserManagementTest;

namespace Peace.Lifelog.UserManagement;

public class LifelogUserManagementService : ICreateLifelogUser, IDeleteLifelogUser
{
    private readonly AppUserManagementService appUserManagementService = new AppUserManagementService();
    private readonly SaltService saltService = new SaltService();

    private readonly CreateDataOnlyDAO createDataOnlyDAO = new CreateDataOnlyDAO();


    public async Task<Response> CreateLifelogUser(LifelogAccountRequest lifelogAccountRequest, LifelogProfileRequest lifelogProfileRequest)
    {
        var response = new Response();

        var saltResponse = saltService.getSalt();

        foreach (string output in saltResponse.Output)
        {
            lifelogAccountRequest.Salt = ("Salt", output);
        }

        // Populate the creation date for user account
        lifelogAccountRequest.CreationDate = ("CreationDate", DateTime.Today.ToString("yyyy-MM-dd"));
        lifelogAccountRequest.Salt = ("Salt", "Bad Salt"); // TODO: Implement Salt function

        // Create the user hash string from the user id
        var userHash = createUserHashWithGivenId(lifelogAccountRequest.UserId.Value + lifelogAccountRequest.Salt.Value);

        // Create UserHashRequest object
        var lifelogUserHashRequest = new LifelogUserHashRequest();
        lifelogUserHashRequest.UserId = ("UserId", lifelogAccountRequest.UserId.Value);
        lifelogUserHashRequest.UserHash = ("UserHash", userHash);

        // Populate user hash field in account request and profile request
        lifelogAccountRequest.UserHash = ("UserHash", userHash);
        lifelogProfileRequest.UserId = ("UserHash", userHash); // UserId is the literal user identification. It is not the column name. With user profile, we are identifying the user using UserHash

        // Populate the creation date for user account
        lifelogAccountRequest.CreationDate = ("CreationDate", DateTime.Today.ToString("yyyy-MM-dd"));
        lifelogAccountRequest.Salt = ("Salt", "Bad Salt"); // TODO: Implement Salt function

        // Populate user account table
        var createLifelogAccountResponse = await createLifelogAccountInDB(lifelogAccountRequest);

        if (createLifelogAccountResponse.HasError == true)
        {
            // TODO: HANDLE ERROR
            response.HasError = true;
            response.ErrorMessage = "Failed to create Account table entry";
            return response;
        }

        // Populate user hash table
        var createUserHashResponse = await createUserHashInDB(lifelogUserHashRequest);

        if (createUserHashResponse.HasError == true)
        {
            // TODO: HANDLE ERROR
            response.HasError = true;
            response.ErrorMessage = "Failed to create UserHash table entry";
            return response;
        }

        // Populate user profile table
        var createLifelogProfileResponse = await createLifelogProfileInDB(lifelogProfileRequest);

        if (createLifelogProfileResponse.HasError == true)
        {
            // TODO: HANDLE ERROR
            response.HasError = true;
            response.ErrorMessage = "Failed to create LifelogProfle";
            return response;
        }


        // TODO: handle success outcome
        response.HasError = false;
        response.Output = [userHash];
        return response;
    }

    public async Task<Response> DeleteLifelogUser(LifelogAccountRequest lifelogAccountRequest, LifelogProfileRequest lifelogProfileRequest)
    {
        var response = new Response();

        var deleteLifelogAccountResponse = await deleteLifelogAccountInDB(lifelogAccountRequest);

        if (deleteLifelogAccountResponse.HasError == true)
        {
            // TODO: HANDLE ERROR
            response.HasError = true;
            response.ErrorMessage = "Failed to delete Account table entry";
            return response;
        }


        // TODO: handle success outcome
        response.HasError = false;
        response.Output = deleteLifelogAccountResponse.Output;
        return response;
    }

    // Helper functions
    private string createUserHashWithGivenId(string userId)
    {
        // Create Lifelog User Hash
        var userHash = "";
        var hashService = new HashService();

        // TODO: POPULATE 'accountRequest' salt

        // TODO: IMPLEMENT SALT FUNC


        var hashResponse = hashService.Hasher(userId + /*accountRequest.Salt*/ "badsalt");

        if (hashResponse.Output is not null)
        {
            foreach (String hashOutput in hashResponse.Output)
            {

                userHash = hashOutput;
            }
        }

        return userHash;

    }

    private async Task<Response> createLifelogAccountInDB(LifelogAccountRequest lifelogAccountRequest)
    {
        Response createAccountResponse = await appUserManagementService.CreateAccount(lifelogAccountRequest);

        return createAccountResponse;

    }

    private async Task<Response> createUserHashInDB(LifelogUserHashRequest lifelogUserHashRequest)
    {
        Response createUserHashResponse = await appUserManagementService.CreateUserHash(lifelogUserHashRequest);

        return createUserHashResponse;
    }

    private async Task<Response> createLifelogProfileInDB(LifelogProfileRequest lifelogProfileRequest)
    {
        Response createLifelogProfileResponse = await appUserManagementService.CreateProfile(lifelogProfileRequest);

        return createLifelogProfileResponse;
    }

    private async Task<Response> deleteLifelogAccountInDB(LifelogAccountRequest lifelogAccountRequest)
    {
        Response deleteAccountResponse = await appUserManagementService.DeleteAccount(lifelogAccountRequest);

        return deleteAccountResponse;

    }
}
