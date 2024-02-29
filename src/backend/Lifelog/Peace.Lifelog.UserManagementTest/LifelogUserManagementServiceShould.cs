namespace Peace.Lifelog.UserManagementTest;

using Peace.Lifelog.DataAccess;
using Peace.Lifelog.UserManagement;
using System.Diagnostics;

public class LifelogUserManagementServiceShould
{

    #region Create Life Log User Tests
    [Fact]
    public async void LifelogUserManagementServiceCreateLifelogUserShould_CreateAnAccountInTheDatabase()
    {
        //Arrange
        var timer = new Stopwatch();

        var LifelogUserManagementService = new LifelogUserManagementService();

        var readDataOnlyDAO = new ReadDataOnlyDAO();
        var deleteDataOnlyDAO = new DeleteDataOnlyDAO();

        var mockUserId = "TestUserCreation";
        var mockRole = "Normal";
        var mockMfaId = "2";

        // Creating User Profile based off User Account
        var mockDob = DateTime.Now.ToString("yyyy-MM-dd");
        var mockZipCode = "90704";


        var testLifelogAccountRequest = new LifelogAccountRequest();
        testLifelogAccountRequest.UserId = ("UserId", mockUserId);
        testLifelogAccountRequest.Role = ("Role", mockRole);

        var testLifelogProfileRequest = new LifelogProfileRequest();
        testLifelogProfileRequest.DOB = ("DOB", mockDob);
        testLifelogProfileRequest.ZipCode = ("ZipCode", mockZipCode);

        var readAccountSql = $"SELECT * FROM LifelogAccount WHERE UserId = \"{mockUserId}\"";

        // Act
        timer.Start();
        var createAccountResponse = await LifelogUserManagementService.CreateLifelogUser(testLifelogAccountRequest, testLifelogProfileRequest);
        timer.Stop();

        var readResponse = await readDataOnlyDAO.ReadData(readAccountSql);


        // Assert
        Assert.True(createAccountResponse.HasError == false);
        Assert.True(timer.Elapsed.TotalSeconds <= TestVariables.MAX_EXECUTION_TIME_IN_SECONDS);
        Assert.True(readResponse.Output.Count == 1);

        //Cleanup
        var deleteAccountResponse = await LifelogUserManagementService.DeleteLifelogUser(testLifelogAccountRequest, testLifelogProfileRequest);

    }

    [Fact]
    public async void LifelogUserManagementServiceCreateLifelogUserShould_ThrowArgumentNullErrorIfUserIdIsNull()
    {
        //Arrange
        var lifelogUserManagementService = new LifelogUserManagementService();

        var mockMfaId = "2";
        var mockRole = "Normal";

        // Creating User Profile based off User Account
        var mockDob = DateTime.Now.ToString("yyyy-MM-dd");
        var mockZipCode = "92612";

        var testLifelogAccountRequest = new LifelogAccountRequest();
        testLifelogAccountRequest.UserId = ("UserId", string.Empty);
        testLifelogAccountRequest.MfaId = ("MfaId", mockMfaId);
        testLifelogAccountRequest.Role = ("Role", mockRole);

        var testLifelogProfileRequest = new LifelogProfileRequest();
        testLifelogProfileRequest.DOB = ("DOB", mockDob);
        testLifelogProfileRequest.ZipCode = ("ZipCode", mockZipCode);

        var errorIsThrown = false;

        // Act
        try
        {
            var createAccountResponse = await lifelogUserManagementService.CreateLifelogUser(testLifelogAccountRequest, testLifelogProfileRequest);
        }
        catch (ArgumentNullException)
        {
            errorIsThrown = true;
        }

        // Assert
        Assert.True(errorIsThrown);
    }
    //TODO
    [Fact]
    public async void LifelogUserManagementServiceCreateLifelogUserShould_ThrowArgumentNullErrorIfAccountDetailsIsNull()
    {
        //Arrange
        var lifelogUserManagementService = new LifelogUserManagementService();

        var mockID = "23";
        // Creating User Profile based off User Account
        var mockDob = DateTime.Now.ToString("yyyy-MM-dd");

        var testLifelogAccountRequest = new LifelogAccountRequest();
        testLifelogAccountRequest.UserId = ("UserId", mockID);

        var testLifelogProfileRequest = new LifelogProfileRequest();
        testLifelogProfileRequest.DOB = ("DOB", mockDob);

        var errorIsThrown = false;

        // Act
        try
        {
            var createAccountResponse = await lifelogUserManagementService.CreateLifelogUser(testLifelogAccountRequest, testLifelogProfileRequest);
        }
        catch (ArgumentNullException)
        {
            errorIsThrown = true;
        }


        // Assert
        Assert.True(errorIsThrown);
    }

    [Fact]
    public async void LifelogUserManagementServiceCreateLifelogUserShould_ReturnAnErrorResponseIfTheAccountAlreadyExist()
    {
        //Arrange
        var LifelogUserManagementService = new LifelogUserManagementService();

        var mockUserId = "TestUserCreation";
        var mockRole = "Normal";
        var mockMfaId = "2";

        // Creating User Profile based off User Account
        var mockDob = DateTime.Now.ToString("yyyy-MM-dd");
        var mockZipCode = "92612";


        var testLifelogAccountRequest = new LifelogAccountRequest();
        testLifelogAccountRequest.UserId = ("UserId", mockUserId);
        testLifelogAccountRequest.MfaId = ("MfaId", mockMfaId);
        testLifelogAccountRequest.Role = ("Role", mockRole);

        var testLifelogProfileRequest = new LifelogProfileRequest();
        testLifelogProfileRequest.DOB = ("DOB", mockDob);
        testLifelogProfileRequest.ZipCode = ("ZipCode", mockZipCode);

        await LifelogUserManagementService.CreateLifelogUser(testLifelogAccountRequest, testLifelogProfileRequest);

        // Act
        var createAccountResponse = await LifelogUserManagementService.CreateLifelogUser(testLifelogAccountRequest, testLifelogProfileRequest);

        // Assert
        Assert.True(createAccountResponse.HasError == true);

        //Cleanup
        var deleteAccountResponse = await LifelogUserManagementService.DeleteLifelogUser(testLifelogAccountRequest, testLifelogProfileRequest);
    }

    [Fact]
    public async void LifelogUserManagementServiceCreateLifelogUserShould_ReturnAnErrorResponseIfTheRequestResultInInvalidSQL()
    {
        //Arrange
        var LifelogUserManagementService = new LifelogUserManagementService();

        var mockUserId = "TestInvalidSQLCreation";
        var mockRole = "Normal";
        var mockMfaId = "2";

        // Creating User Profile based off User Account
        var mockDob = DateTime.Now.ToString("yyyy-MM-dd");
        var mockZipCode = "92612";


        var testLifelogAccountRequest = new LifelogAccountRequest();
        testLifelogAccountRequest.UserId = ("WrongType", mockUserId);
        testLifelogAccountRequest.MfaId = ("MfaId", mockMfaId);
        testLifelogAccountRequest.Role = ("Role", mockRole);

        var testLifelogProfileRequest = new LifelogProfileRequest();
        testLifelogProfileRequest.DOB = ("DOB", mockDob);
        testLifelogProfileRequest.ZipCode = ("ZipCode", mockZipCode);

        // Act
        var createAccountResponse = await LifelogUserManagementService.CreateLifelogUser(testLifelogAccountRequest, testLifelogProfileRequest);

        // Assert
        Assert.True(createAccountResponse.HasError == true);
    }
    #endregion

    #region Delete Lifelog User Tests
    [Fact]
    public async void LifelogUserManagementServiceDeleteLifelogUserShould_DeleteAnAccountInTheDatabase()
    {
        //Arrange
        var timer = new Stopwatch();

        var lifelogUserManagementService = new LifelogUserManagementService();

        var readDataOnlyDAO = new ReadDataOnlyDAO();

        var mockUserId = "TestUserCreation";
        var mockRole = "Normal";
        var mockMfaId = "2";

        // Creating User Profile based off User Account
        var mockDob = DateTime.Now.ToString("yyyy-MM-dd");
        var mockZipCode = "92612";


        var testLifelogAccountRequest = new LifelogAccountRequest();
        testLifelogAccountRequest.UserId = ("UserId", mockUserId);
        testLifelogAccountRequest.Role = ("Role", mockRole);

        var testLifelogProfileRequest = new LifelogProfileRequest();
        testLifelogProfileRequest.DOB = ("DOB", mockDob);
        testLifelogProfileRequest.ZipCode = ("ZipCode", mockZipCode);

        var readAccountSql = $"SELECT * FROM LifelogAccount WHERE UserId = \"{mockUserId}\"";
        var createAccountResponse = await lifelogUserManagementService.CreateLifelogUser(testLifelogAccountRequest, testLifelogProfileRequest);

        // Act
        timer.Start();
        var deleteAccountResponse = await lifelogUserManagementService.DeleteLifelogUser(testLifelogAccountRequest, testLifelogProfileRequest);
        timer.Stop();

        var readResponse = await readDataOnlyDAO.ReadData(readAccountSql);


        // Assert
        Assert.True(deleteAccountResponse.HasError == false);
        Assert.True(timer.Elapsed.TotalSeconds <= TestVariables.MAX_EXECUTION_TIME_IN_SECONDS);
        Assert.True(readResponse.Output is null);

    }

    [Fact]
    public async void LifelogUserManagementServiceDeleteLifelogUserShould_ThrowArgumentNullErrorIfUserIdIsNull()
    {
        //Arrange
        var lifelogUserManagementService = new LifelogUserManagementService();

        var mockMfaId = "2";
        var mockRole = "Normal";

        // Creating User Profile based off User Account
        var mockDob = DateTime.Now.ToString("yyyy-MM-dd");
        var mockZipCode = "92612";

        var testLifelogAccountRequest = new LifelogAccountRequest();
        testLifelogAccountRequest.UserId = ("UserId", string.Empty);
        testLifelogAccountRequest.MfaId = ("MfaId", mockMfaId);
        testLifelogAccountRequest.Role = ("Role", mockRole);

        var testLifelogProfileRequest = new LifelogProfileRequest();
        testLifelogProfileRequest.DOB = ("DOB", mockDob);
        testLifelogProfileRequest.ZipCode = ("ZipCode", mockZipCode);

        var errorIsThrown = false;

        // Act
        try
        {
            var deleteAccountResponse = await lifelogUserManagementService.DeleteLifelogUser(testLifelogAccountRequest, testLifelogProfileRequest);
        }
        catch (ArgumentNullException)
        {
            errorIsThrown = true;
        }

        // Assert
        Assert.True(errorIsThrown);
    }

    [Fact]
    public async void LifelogUserManagementServiceDeleteLifelogUserShould_ReturnAnErrorResponseIfUserDoesntExist()
    {
        //Arrange
        var lifelogUserManagementService = new LifelogUserManagementService();

        var mockMfaId = "2";
        var mockRole = "Normal";

        // Creating User Profile based off User Account
        var mockDob = DateTime.Now.ToString("yyyy-MM-dd");
        var mockZipCode = "92612";

        var testLifelogAccountRequest = new LifelogAccountRequest();
        testLifelogAccountRequest.UserId = ("UserId", "userdoesntexist");
        testLifelogAccountRequest.MfaId = ("MfaId", mockMfaId);
        testLifelogAccountRequest.Role = ("Role", mockRole);

        var testLifelogProfileRequest = new LifelogProfileRequest();
        testLifelogProfileRequest.DOB = ("DOB", mockDob);
        testLifelogProfileRequest.ZipCode = ("ZipCode", mockZipCode);

        // Act
        var deleteAccountResponse = await lifelogUserManagementService.DeleteLifelogUser(testLifelogAccountRequest, testLifelogProfileRequest);



        // Assert
        Assert.True(deleteAccountResponse.HasError == true);
    }

    [Fact]
    public async void LifelogUserManagementServiceDeleteLifelogUserShould_ReturnAnErrorResponseIfRequestResultInInvalidSQL()
    {
        //Arrange
        var lifelogUserManagementService = new LifelogUserManagementService();

        var mockUserId = "TestUserCreation";
        var mockMfaId = "2";
        var mockRole = "Normal";

        // Creating User Profile based off User Account
        var mockDob = DateTime.Now.ToString("yyyy-MM-dd");
        var mockZipCode = "92612";

        var testLifelogAccountRequest = new LifelogAccountRequest();
        testLifelogAccountRequest.UserId = ("UserId", mockUserId);
        testLifelogAccountRequest.MfaId = ("MfaId", mockMfaId);
        testLifelogAccountRequest.Role = ("Role", mockRole);

        var testLifelogProfileRequest = new LifelogProfileRequest();
        testLifelogProfileRequest.DOB = ("DOB", mockDob);
        testLifelogProfileRequest.ZipCode = ("ZipCode", mockZipCode);

        // Act
        var deleteAccountResponse = await lifelogUserManagementService.DeleteLifelogUser(testLifelogAccountRequest, testLifelogProfileRequest);

        // Assert
        Assert.True(deleteAccountResponse.HasError == true);
    }

    #endregion
}