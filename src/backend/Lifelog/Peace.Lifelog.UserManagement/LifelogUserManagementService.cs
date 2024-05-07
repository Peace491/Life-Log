using DomainModels;
using Peace.Lifelog.DataAccess;
using Peace.Lifelog.Security;
using Peace.Lifelog.Infrastructure;
using Peace.Lifelog.UserManagementTest;
using Newtonsoft.Json;
using Peace.Lifelog.Email;


namespace Peace.Lifelog.UserManagement;

public class LifelogUserManagementService : ILifelogUserManagementService
{
    private readonly AppUserManagementService appUserManagementService;
    private readonly SaltService saltService;
    private readonly CreateDataOnlyDAO createDataOnlyDAO;
    private readonly IUserManagmentRepo userManagementRepo;
    public LifelogUserManagementService(IUserManagmentRepo userManagementRepo, AppUserManagementService appUserManagementService, SaltService saltService, CreateDataOnlyDAO createDataOnlyDAO)
    {
        this.userManagementRepo = userManagementRepo;
        this.appUserManagementService = appUserManagementService;
        this.saltService = saltService;
        this.createDataOnlyDAO = createDataOnlyDAO;
    }    private readonly ReadDataOnlyDAO readDataOnlyDAO= new ReadDataOnlyDAO();
    private readonly DeleteDataOnlyDAO deleteDataOnlyDAO = new DeleteDataOnlyDAO();


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
        var createLifelogAuthenticationResponse = await createLifelogAuthenticationInDB(lifelogAccountRequest, lifelogProfileRequest);

        if (createLifelogAuthenticationResponse.HasError == true)
        {
            // TODO: HANDLE ERROR
            response.HasError = true;
            response.ErrorMessage = "Failed to create LifelogAuthentication";
            return response;
        }

        // Populate RecSummary table
        var createRecSummaryResponse = await createRecSummaryInDB(userHash);

        if (createRecSummaryResponse.HasError == true)
        {
            // TODO: HANDLE ERROR
            response.HasError = true;
            response.ErrorMessage = "Failed to create RecSummary table entry";
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

    public async Task<Response> GetRecoveryAccountRequests(AppPrincipal principal)
    {
        var response = new Response();

        if (principal.Claims == null)
        {
            response.HasError = true;
            response.ErrorMessage = "Must provide principal";
            return response;
        }
        var userRole = principal.Claims["Role"];
        if (userRole != "Admin" && userRole != "Root")
        {
            response.HasError = true;
            response.ErrorMessage = "Unauthorized Request";
            return response;
        }

        var sql = "";

        if (userRole == "Root")
        {
            sql = "SELECT LifelogAccount.UserId FROM LifelogAccount INNER JOIN LifelogAccountRecoveryRequest WHERE LifelogAccount.UserId = LifelogAccountRecoveryRequest.UserId";
        }
        else
        {
            sql = "SELECT LifelogAccount.UserId FROM LifelogAccount INNER JOIN LifelogAccountRecoveryRequest "
            + "WHERE LifelogAccount.UserId = LifelogAccountRecoveryRequest.UserId "
            + "AND (LifelogAccount.Role != 'Admin' AND LifelogAccount.Role != 'Root') ";
        }

        response = await readDataOnlyDAO.ReadData(sql);

        if (response.HasError == true)
        {
            response.ErrorMessage = "Failed to get Lifelog User Recovery Requests.";
            return response;
        }

        response.HasError = false;
        return response;
    }

    public async Task<Response> RecoverLifelogUser(AppPrincipal principal, LifelogAccountRequest lifelogAccountRequest)
    {
        var response = new Response();

        if (principal.Claims == null)
        {
            response.HasError = true;
            response.ErrorMessage = "Must provide principal";
            return response;
        }
        var userRole = principal.Claims["Role"];
        if (userRole != "Admin" && userRole != "Root")
        {
            response.HasError = true;
            response.ErrorMessage = "Unauthorized Request";
            return response;
        }

        var recoverLifelogAccountResponse = await recoverLifelogAccountfromDB(lifelogAccountRequest);

        var deleteRecoveryRequestSql = $"DELETE FROM LifelogAccountRecoveryRequest WHERE UserId = \"{lifelogAccountRequest.UserId.Value}\"";

        var deleteResponse = await deleteDataOnlyDAO.DeleteData(deleteRecoveryRequestSql);

        if (recoverLifelogAccountResponse.HasError == true || deleteResponse.HasError == true)
        {
            response.HasError = true;
            response.ErrorMessage = "Failed to recover Lifelog User Account.";
            return response;
        }

        response.HasError = false;
        response.Output = recoverLifelogAccountResponse.Output;
        return response;
    }

    public async Task<Response> CreateRecoveryAccountRequest(LifelogAccountRequest lifelogAccountRequest)
    {
        var response = new Response();

        if (lifelogAccountRequest.UserId.Type == null || lifelogAccountRequest.UserId.Value == null)
        {
            response.HasError = true;
            response.ErrorMessage = "Must provide UserId";
            return response;
        }

        string sql = $"INSERT INTO LifelogAccountRecoveryRequest (UserId) VALUES (\"{lifelogAccountRequest.UserId.Value}\")";

        response = await createDataOnlyDAO.CreateData(sql);

        if (response.HasError == true)
        {
            response.ErrorMessage = "Failed to create Account Recovery Request";
            return response;
        }

        response.HasError = false;

        return response;
    }
    public async Task<Response> ViewPersonalIdentifiableInformation(string userHash)
    {
        Console.WriteLine("ViewPersonalIdentifiableInformation");
        var response = await userManagementRepo.ViewPersonalIdentifiableInformation(userHash);

        string fpath = await ComposeLogsToFileAsync(response);

        Console.WriteLine("Logs written to: " + fpath);

        var EmailService = new EmailService();
        var emailResponse = await EmailService.SendPIIEmail(userHash, fpath);
        // TODO: send email with attachment, and log operation
        return response;
    }
    // Helper functions
    // Some should be moved to infrastructure
    #region Helper Functions
    public async Task<string> ComposeLogsToFileAsync(Response response)
    {
        string directory = AppDomain.CurrentDomain.BaseDirectory;
        string filePath = Path.Combine(directory, "logs.txt");

        using (StreamWriter writer = File.CreateText(filePath))
        {
            if (response.Output != null)
            {
                foreach (var outputItem in response.Output)
                {
                    await writer.WriteLineAsync(JsonConvert.SerializeObject(outputItem));
                }
            }
        }

        return filePath;
    }
    
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

    private string createSalt()
    {

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

    private async Task<Response> createLifelogUserRoleInDB(LifelogAccountRequest lifelogAccountRequest)
    {
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

    private async Task<Response> createLifelogUserOTPInDB(LifelogProfileRequest lifelogProfileRequest)
    {
        string sql = $"INSERT INTO LifelogUserOTP ({lifelogProfileRequest.UserId.Type}) VALUES (\"{lifelogProfileRequest.UserId.Value}\")";
        var createLifelogUserOTPInDBResponse = await createDataOnlyDAO.CreateData(sql);

        return createLifelogUserOTPInDBResponse;
    }

    private async Task<Response> createLifelogAuthenticationInDB(LifelogAccountRequest lifelogAccountRequest, LifelogProfileRequest lifelogProfileRequest)
    {
        string sql = $"INSERT INTO LifelogAuthentication ({lifelogAccountRequest.UserId.Type}, {lifelogProfileRequest.UserId.Type}, {lifelogAccountRequest.Role.Type})"
         + $"VALUES (\"{lifelogAccountRequest.UserId.Value}\", \"{lifelogProfileRequest.UserId.Value}\", \"{lifelogAccountRequest.Role.Value}\")";
        var createLifelogAuthenticationInDBResponse = await createDataOnlyDAO.CreateData(sql);

        return createLifelogAuthenticationInDBResponse;
    }

    private async Task<Response> createRecSummaryInDB(string userHash)
    {
        // string sql = $"INSERT INTO RecSummary ({lifelogAccountRequest.UserId.Type}, {lifelogProfileRequest.UserId.Type}, {lifelogAccountRequest.Role.Type}, IsUserFormCompleted)" 
        //  + $"VALUES (\"{lifelogAccountRequest.UserId.Value}\", \"{lifelogProfileRequest.UserId.Value}\", \"{lifelogAccountRequest.Role.Value}\", 0)";
        string sql = $"INSERT INTO `RecSummary` (`UserHash`, `Category1`, `Category2`, `SystemMostPopular`) VALUES ('{userHash}', 'Mental Health', 'Physical Health', (SELECT Category1 FROM (SELECT Category1 FROM RecSummary WHERE UserHash = 'system') AS derivedTable));";

        var createRecSummaryInDBResponse = await createDataOnlyDAO.CreateData(sql);

        return createRecSummaryInDBResponse;
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

    public async Task<string> getUserHashFromUserId(string userId)
    {
        if (userId == string.Empty) return "";

        string userHash = "";

        var readDataOnlyDAO = new ReadDataOnlyDAO();

        string sql = $"SELECT UserHash FROM LifelogAccount WHERE UserId=\"{userId}\"";

        var response = await readDataOnlyDAO.ReadData(sql);

        if (response.Output != null)
        {
            foreach (List<Object> output in response.Output)
            {
                foreach (string userHashOutput in output)
                {
                    userHash = userHashOutput;
                }
            }
        }

        return userHash;
    }

    public async Task<string> GetUserIdFromUserHash(string userHash)
    {
        if (userHash == string.Empty) return "";

        string userId = "";

        var readDataOnlyDAO = new ReadDataOnlyDAO();

        string sql = $"SELECT UserId FROM LifelogUserHash WHERE UserHash=\"{userHash}\"";

        var response = await readDataOnlyDAO.ReadData(sql);

        if (response.Output != null)
        {
            foreach (List<Object> output in response.Output)
            {
                foreach (string userIdOutput in output)
                {
                    userId = userIdOutput;
                }
            }
        }

        return userId;
    }

    #endregion
}
