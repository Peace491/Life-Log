using DomainModels;
using Org.BouncyCastle.Asn1;
using Peace.Lifelog.DataAccess;
using Peace.Lifelog.Security;
using Peace.Lifelog.UserManagementTest;

namespace Peace.Lifelog.UserManagement;

public class LifelogUserManagementService : ICreateLifelogUser, IDeleteLifelogUser, IModifyLifelogUser, IRecoverLifelogUser
{
    private readonly AppUserManagementService appUserManagementService = new AppUserManagementService();
    private readonly SaltService saltService = new SaltService();

    private readonly CreateDataOnlyDAO createDataOnlyDAO = new CreateDataOnlyDAO();


    public async Task<Response> CreateLifelogUser(LifelogAccountRequest lifelogAccountRequest, LifelogProfileRequest lifelogProfileRequest)
    {
        var response = new Response();

        var salt = createSalt();

        // Populate the creation date for user account
        lifelogAccountRequest.CreationDate = ("CreationDate", DateTime.Today.ToString("yyyy-MM-dd"));

        // Create the user hash string from the user id
        var userHash = createUserHashWithGivenId(lifelogAccountRequest.UserId.Value, salt);

        // Create UserHashRequest object
        var lifelogUserHashRequest = new LifelogUserHashRequest();
        lifelogUserHashRequest.UserId = ("UserId", lifelogAccountRequest.UserId.Value);
        lifelogUserHashRequest.UserHash = ("UserHash", userHash);

        // Populate user hash field in account request and profile request
        lifelogAccountRequest.UserHash = ("UserHash", userHash);
        lifelogProfileRequest.UserId = ("UserHash", userHash); // UserId is the literal user identification. It is not the column name. With user profile, we are identifying the user using UserHash

        // Populate the creation date for user account
        lifelogAccountRequest.CreationDate = ("CreationDate", DateTime.Today.ToString("yyyy-MM-dd"));
        lifelogAccountRequest.Salt = ("Salt", salt);

        // Populate user account table
        var createLifelogAccountResponse = await createLifelogAccountInDB(lifelogAccountRequest);

        if (createLifelogAccountResponse.HasError == true)
        {
            // TODO: HANDLE ERROR
            response.HasError = true;
            response.ErrorMessage = "Failed to create Account table entry";
            return response;
        }

        // Populate user role table
        var createLifelogUserRoleResponse = await createLifelogUserRoleInDB(lifelogAccountRequest);

        if (createLifelogUserRoleResponse.HasError == true)
        {
            // TODO: HANDLE ERROR
            response.HasError = true;
            response.ErrorMessage = "Failed to create LifelogUserRole table entry";
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

        // Populate OTP table
        var createLifelogUserOTPResponse = await createLifelogUserOTPInDB(lifelogProfileRequest);

        if (createLifelogUserOTPResponse.HasError == true)
        {
            // TODO: HANDLE ERROR
            response.HasError = true;
            response.ErrorMessage = "Failed to create LifelogUserOTP";
            return response;
        }

         // Populate Authentication table
        var createLifelogAuthenticationResponse = await createLifelogAuthenticationInDB(lifelogAccountRequest ,lifelogProfileRequest);

        if (createLifelogAuthenticationResponse.HasError == true)
        {
            // TODO: HANDLE ERROR
            response.HasError = true;
            response.ErrorMessage = "Failed to create LifelogUserOTP";
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

        response.HasError = false;
        response.Output = deleteLifelogAccountResponse.Output;
        return response;
    }

    public async Task<Response> ModifyLifelogUser(LifelogProfileRequest lifelogProfileRequest)
    {
        var response = new Response();

        var modifyLifelogProfileResponse = await modifyLifelogProfileInDB(lifelogProfileRequest);

        if (modifyLifelogProfileResponse.HasError == true)
        {
            response.HasError = true;
            response.ErrorMessage = "Failed to modify Lifelog user profile.";
            return response;
        }

        response.HasError = false;
        response.Output = modifyLifelogProfileResponse.Output;
        return response;
    }
    public async Task<Response> RecoverLifelogUser(LifelogAccountRequest lifelogAccountRequest)
    {
        var response = new Response();

        var recoverLifelogAccountResponse = await recoverLifelogAccountfromDB(lifelogAccountRequest);

        if (recoverLifelogAccountResponse.HasError == true)
        {
            response.HasError = true;
            response.ErrorMessage = "Failed to recover Lifelog User Account.";
            return response;
        }

        response.HasError = false;
        response.Output = recoverLifelogAccountResponse.Output;
        return response;
    }
    // Helper functions
    #region Helper Functions
    private string createUserHashWithGivenId(string userId, string salt)
    {
        // Create Lifelog User Hash
        var userHash = "";
        var hashService = new HashService();

        var hashResponse = hashService.Hasher(userId + salt);

        if (hashResponse.Output is not null)
        {
            foreach (String hashOutput in hashResponse.Output)
            {

                userHash = hashOutput;
            }
        }

        return userHash;

    }

    private string createSalt() {
        
        var salt = "";
        var saltResponse = saltService.getSalt();

        if (saltResponse.Output != null)
        {
            foreach (string output in saltResponse.Output)
            {
                salt = output;
            }
        }

        return salt;
    }

    private async Task<Response> createLifelogAccountInDB(LifelogAccountRequest lifelogAccountRequest)
    {
        Response createAccountResponse = await appUserManagementService.CreateAccount(lifelogAccountRequest);

        return createAccountResponse;

    }

    private async Task<Response> createLifelogUserRoleInDB(LifelogAccountRequest lifelogAccountRequest) {
        string sql = $"INSERT INTO LifelogUserRole ({lifelogAccountRequest.UserId.Type}, {lifelogAccountRequest.Role.Type})" 
         + $"VALUES (\"{lifelogAccountRequest.UserId.Value}\", \"{lifelogAccountRequest.Role.Value}\")";
        var createLifelogUserRoleInDBResponse = await createDataOnlyDAO.CreateData(sql);

        return createLifelogUserRoleInDBResponse;
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

    private async Task<Response> createLifelogUserOTPInDB(LifelogProfileRequest lifelogProfileRequest) {
        string sql = $"INSERT INTO LifelogUserOTP ({lifelogProfileRequest.UserId.Type}) VALUES (\"{lifelogProfileRequest.UserId.Value}\")";
        var createLifelogUserOTPInDBResponse = await createDataOnlyDAO.CreateData(sql);

        return createLifelogUserOTPInDBResponse;
    }

    private async Task<Response> createLifelogAuthenticationInDB(LifelogAccountRequest lifelogAccountRequest, LifelogProfileRequest lifelogProfileRequest)
    {
        string sql = $"INSERT INTO LifelogAuthentication ({lifelogProfileRequest.UserId.Type}, {lifelogAccountRequest.Role.Type})" 
         + $"VALUES (\"{lifelogProfileRequest.UserId.Value}\", \"{lifelogAccountRequest.Role.Value}\")";
        var createLifelogAuthenticationInDBResponse = await createDataOnlyDAO.CreateData(sql);

        return createLifelogAuthenticationInDBResponse;
    }

    private async Task<Response> deleteLifelogAccountInDB(LifelogAccountRequest lifelogAccountRequest)
    {
        Response deleteAccountResponse = await appUserManagementService.DeleteAccount(lifelogAccountRequest);

        return deleteAccountResponse;

    }

    private async Task<Response> modifyLifelogProfileInDB(LifelogProfileRequest lifelogProfileRequest)
    {
        Response modifyLifelogProfileResponse = await appUserManagementService.ModifyProfile(lifelogProfileRequest);

        return modifyLifelogProfileResponse;
    }

    private async Task<Response> recoverLifelogAccountfromDB(IStatusAccountRequest userAccountRequest)
    {
        Response recoverLifelogAccountResponse = await appUserManagementService.RecoverStatusAccount(userAccountRequest);

        return recoverLifelogAccountResponse;
    }

    public async Task<string> getUserHashFromUserId(string userId) {
        if (userId == string.Empty) return "";

        string userHash = "";

        var readDataOnlyDAO = new ReadDataOnlyDAO();

        string sql = $"SELECT UserHash FROM LifelogAccount WHERE UserId=\"{userId}\"";

        var response = await readDataOnlyDAO.ReadData(sql);

        if (response.Output != null) {
            foreach (List<Object> output in response.Output) {
                foreach (string userHashOutput in output) {
                    userHash = userHashOutput;
                }
            }
        }

        return userHash;
    }

    #endregion
}
