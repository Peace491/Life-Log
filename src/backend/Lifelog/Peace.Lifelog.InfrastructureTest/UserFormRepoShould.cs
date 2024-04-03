namespace Peace.Lifelog.InfrastructureTest;

using Peace.Lifelog.UserManagement;
using Peace.Lifelog.DataAccess;
using Xunit.Sdk;
using Peace.Lifelog.Infrastructure;

public class UserFormRepoShould : IAsyncLifetime, IDisposable
{
    private static ICreateDataOnlyDAO createDataOnlyDAO = new CreateDataOnlyDAO();
    private static IReadDataOnlyDAO readDataOnlyDAO = new ReadDataOnlyDAO();
    private static IUpdateDataOnlyDAO updateDataOnlyDAO = new UpdateDataOnlyDAO();
    private static UserFormRepo userFormRepo = new UserFormRepo(createDataOnlyDAO, readDataOnlyDAO, updateDataOnlyDAO);
    
    
    private const string USER_ID = "TestUserFormRepoAccount";
    private string USER_HASH = "";
    private const string ROLE = "Normal";

    private string DOB = DateTime.Today.ToString("yyyy-MM-dd");
    
    private const string ZIP_CODE = "90704";
    
    private const int MAX_TIME_IN_SECOND = 3;

    public async Task InitializeAsync()
    {   
        var appUserManagementService = new AppUserManagementService();
        var lifelogUserManagementService = new LifelogUserManagementService();

        var testLifelogAccountRequest = new LifelogAccountRequest();
        testLifelogAccountRequest.UserId = ("UserId", USER_ID);
        testLifelogAccountRequest.Role = ("Role", ROLE);

        var deleteAccountResponse = await appUserManagementService.DeleteAccount(testLifelogAccountRequest); // Make sure no test account with the same name exist

        var testLifelogProfileRequest = new LifelogProfileRequest();
        testLifelogProfileRequest.DOB = ("DOB", DOB);
        testLifelogProfileRequest.ZipCode = ("ZipCode", ZIP_CODE);


        var createAccountResponse = await lifelogUserManagementService.CreateLifelogUser(testLifelogAccountRequest, testLifelogProfileRequest);

        if (createAccountResponse.Output is not null)
        {
            foreach (string output in createAccountResponse.Output)
            {
                USER_HASH = output;
            }
        }

    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        var appUserManagementService = new AppUserManagementService();
        var testLifelogAccountRequest = new LifelogAccountRequest();
        testLifelogAccountRequest.UserId = ("UserId", USER_ID);
        var deleteAccountResponse = appUserManagementService.DeleteAccount(testLifelogAccountRequest);
    }


    [Fact]
    public async Task UserFormRepoShould_CreateUserFormForUserInDB() {
        // Arrange
        int mentalHealthRating = 1;
        int physicalHealthRating = 2;
        int outdoorRating = 3;
        int sportRating = 4;
        int artRating = 5;
        int hobbyRating = 6;
        int thrillRating = 7;
        int travelRating = 8;
        int volunteeringRating = 9;
        int foodRating = 10;

        var readUserFormDataSql = $"SELECT * FROM UserForm WHERE UserHash=\"{USER_HASH}\"";
        var readAuthenticationSql = $"SELECT IsUserFormCompleted FROM LifelogProfile WHERE UserHash=\"{USER_HASH}\"";

        // Act
        var response = await userFormRepo!.CreateUserFormInDB(USER_HASH, mentalHealthRating, physicalHealthRating, outdoorRating, sportRating, artRating, hobbyRating, thrillRating, travelRating, volunteeringRating, foodRating);

        // Assert
        Assert.True(response.HasError == false);

        var readUserFormResponse = await readDataOnlyDAO.ReadData(readUserFormDataSql);
        var readAuthenticationResponse = await readDataOnlyDAO.ReadData(readAuthenticationSql);

    }

    [Fact]
    public async Task UserFormRepoShould_UpdateUserFormForUserInDB()
    {
        // Arrange
        int mentalHealthRating = 1;
        int physicalHealthRating = 2;
        int outdoorRating = 3;
        int sportRating = 4;
        int artRating = 5;
        int hobbyRating = 6;
        int thrillRating = 7;
        int travelRating = 8;
        int volunteeringRating = 9;
        int foodRating = 10;

        var response = await userFormRepo!.CreateUserFormInDB(USER_HASH, mentalHealthRating, physicalHealthRating, outdoorRating, sportRating, artRating, hobbyRating, thrillRating, travelRating, volunteeringRating, foodRating);

        // Act
        var updateResponse = await userFormRepo.UpdateUserFormInDB(USER_HASH, mentalHealthRating: 2, physicalHealthRating: 1);

        // Assert
        Assert.True(updateResponse.HasError == false);

        
    }
}