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
    public async Task UserFormRepoCreateUserShould_CreateUserFormForUserInDB()
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

        var readUserFormDataSql = $"SELECT * FROM UserForm WHERE UserHash=\"{USER_HASH}\"";

        // Act
        var response = await userFormRepo!.CreateUserFormInDB(USER_HASH, mentalHealthRating, physicalHealthRating, outdoorRating, sportRating, artRating, hobbyRating, thrillRating, travelRating, volunteeringRating, foodRating);

        // Assert
        Assert.True(response.HasError == false);

        var readUserFormResponse = await readDataOnlyDAO.ReadData(readUserFormDataSql);
        var readAuthenticationResponse = await userFormRepo.ReadUserFormCompletionStatusInDB(USER_HASH);

        Assert.True(readUserFormResponse.Output != null);
        Assert.True(readAuthenticationResponse.Output != null);
        foreach (List<Object> output in readAuthenticationResponse.Output)
        {
            foreach (bool completionStatus in output)
            {
                Assert.True(completionStatus == true);
            }
        }
    }

    [Fact]
    public async Task UserFormRepoCreateUserShould_ThrowErrorIfTheUserDoesNotExist()
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

        var readUserFormDataSql = $"SELECT * FROM UserForm WHERE UserHash=\"{USER_HASH}\"";

        // Act
        var response = await userFormRepo!.CreateUserFormInDB("", mentalHealthRating, physicalHealthRating, outdoorRating, sportRating, artRating, hobbyRating, thrillRating, travelRating, volunteeringRating, foodRating);

        // Assert
        Assert.True(response.HasError);
        var readUserFormResponse = await readDataOnlyDAO.ReadData(readUserFormDataSql);

        Assert.True(readUserFormResponse.Output == null);
    }

    [Fact]
    public async Task UserFormRepoGetUserFormRankingShould_GetUserFormRankingFromDB()
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

        var createResponse = await userFormRepo!.CreateUserFormInDB(USER_HASH, mentalHealthRating, physicalHealthRating, outdoorRating, sportRating, artRating, hobbyRating, thrillRating, travelRating, volunteeringRating, foodRating);

        // Act
        var response = await userFormRepo.ReadUserFormCategoriesRankingInDB(USER_HASH);

        // Assert
        Assert.True(response.Output != null);
        foreach (List<Object> rankings in response.Output)
        {
            // Because the ranking is a straight forward 1 to 10, each ranking should increment according to the loop
            int currRanking = 1;

            foreach (var ranking in rankings)
            {
                Assert.True(currRanking == Convert.ToInt32(ranking));
                currRanking++;
            }

        }
    }

    [Fact]
    public async Task UserFormRepoGetUserFormRankingShould_ReturnNullIfTheUserDoesNotExist()
    {
        // Arrange

        // Act
        var response = await userFormRepo.ReadUserFormCategoriesRankingInDB("");

        // Assert
        Assert.True(response.Output == null);

    }

    [Fact]
    public async Task UserFormReadUserFormCompletionStatusShould_Return1_IfTheUserFormIsCompleted()
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

        var createResponse = await userFormRepo!.CreateUserFormInDB(USER_HASH, mentalHealthRating, physicalHealthRating, outdoorRating, sportRating, artRating, hobbyRating, thrillRating, travelRating, volunteeringRating, foodRating);

        // Act
        var response = await userFormRepo.ReadUserFormCompletionStatusInDB(USER_HASH);

        // Assert
        Assert.True(response.Output != null);

        foreach (List<Object> output in response.Output)
        {
            foreach (bool completionStatus in output)
            {
                Assert.True(completionStatus);
            }
        }
    }

    [Fact]
    public async Task UserFormReadUserFormCompletionStatusShould_Return0_IfTheUserFormIsNotCompleted()
    {
        // Arrange
        var userId = "TestNotCompletedUserForm";
        var userHash = "";
        var lifelogUserManagementService = new LifelogUserManagementService();

        var testLifelogAccountRequest = new LifelogAccountRequest();
        testLifelogAccountRequest.UserId = ("UserId", userId);
        testLifelogAccountRequest.Role = ("Role", ROLE);

        var testLifelogProfileRequest = new LifelogProfileRequest();
        testLifelogProfileRequest.DOB = ("DOB", DOB);
        testLifelogProfileRequest.ZipCode = ("ZipCode", ZIP_CODE);

        var createAccountResponse = await lifelogUserManagementService.CreateLifelogUser(testLifelogAccountRequest, testLifelogProfileRequest);

        if (createAccountResponse.Output is not null)
        {
            foreach (string output in createAccountResponse.Output)
            {
                userHash = output;
            }
        }

        // Act
        var response = await userFormRepo.ReadUserFormCompletionStatusInDB(userHash);

        // Assert
        Assert.True(response.Output != null);

        foreach (List<Object> output in response.Output)
        {
            foreach (bool completionStatus in output)
            {
                Assert.False(completionStatus);
            }
        }

        // Cleanup
        await lifelogUserManagementService.DeleteLifelogUser(testLifelogAccountRequest, testLifelogProfileRequest);
    }

    [Fact]
    public async Task UserFormRepoUpdateUserFormShould_UpdateUserFormForUserInDB()
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
        Assert.True(updateResponse.Output != null);
    }

    [Fact]
    public async Task UserFormRepoUpdateUserFormShould_ReturnNullIfTheUserDoesNotExist()
    {
        // Arrange

        // Act
        var updateResponse = await userFormRepo.UpdateUserFormInDB("", mentalHealthRating: 2, physicalHealthRating: 1);

        // Assert
        Assert.True(updateResponse.Output != null);
        foreach(Int32 affectedRow in updateResponse.Output)
        {
                Assert.True(affectedRow == 0);
            
        }
        
    }
}