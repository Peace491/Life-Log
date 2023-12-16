namespace Peace.Lifelog.UserManagementTest;

using Peace.Lifelog.DataAccess;
using Peace.Lifelog.UserManagement;
using System.Diagnostics;

public class AppUserManagementServiceShould : IDisposable
{
    private const int MAX_EXECUTION_TIME_IN_SECONDS = 3;
    private const string TABLE = "TestAccount";
    private const string MFA_ID_TYPE = "Phone";
    private const string USER_ID_TYPE = "Email";
    private const string PASSWORD_TYPE = "OTP";
    private const string STATUS_TYPE = "Status";

    // Setup for all test
    public AppUserManagementServiceShould()
    {

        var DDLTransactionDAO = new DDLTransactionDAO();
        var createMockTableSql = $"CREATE TABLE {TABLE} ("
        + $"{USER_ID_TYPE} varchar(32) NOT NULL," +
        $"{MFA_ID_TYPE} varchar(9) NOT NULL," +
        $"{PASSWORD_TYPE} varchar(32) NOT NULL," +
        $"{STATUS_TYPE} varchar(10) NOT NULL," +
        $"PRIMARY KEY ({USER_ID_TYPE})" +
        ");";

        DDLTransactionDAO.ExecuteDDLCommand(createMockTableSql);

    }

    // Cleanup for all tests
    public async void Dispose()
    {
        var DDLTransactionDAO = new DDLTransactionDAO();

        var deleteMockTableSql = $"DROP TABLE {TABLE}";

        var test = await DDLTransactionDAO.ExecuteDDLCommand(deleteMockTableSql);

    }

    [Fact]
    public async void AppUserManagementServiceCreateAccountShould_CreateAnAccountInTheDatabase()
    {
        //Arrange
        var timer = new Stopwatch();

        var appUserManagementService = new AppUserManagementService();

        var readDataOnlyDAO = new ReadDataOnlyDAO();

        var mockUserId = "1";
        var mockMfaId = "2";
        var mockPassword = "password";

        var testAccountRequest = new TestAccountRequest();

        testAccountRequest.UserId = (USER_ID_TYPE, mockUserId);
        testAccountRequest.MfaId = (MFA_ID_TYPE, mockMfaId);
        testAccountRequest.Password = (PASSWORD_TYPE, mockPassword);

        var readAccountSql = $"SELECT * FROM {TABLE} WHERE {USER_ID_TYPE} = {mockUserId}";

        // Act
        timer.Start();
        var createAccountResponse = await appUserManagementService.CreateAccount(testAccountRequest);
        timer.Stop();

        var readResponse = await readDataOnlyDAO.ReadData(readAccountSql);


        // Assert
        Assert.True(createAccountResponse.HasError == false);
        Assert.True(timer.Elapsed.TotalSeconds <= MAX_EXECUTION_TIME_IN_SECONDS);
        Assert.True(readResponse.Output.Count == 1);

    }

    [Fact]
    public async void AppUserManagementServiceCreateAccountShould_ThrowArgumentNullErrorIfUserIdIsNull()
    {
        //Arrange
        var appUserManagementService = new AppUserManagementService();

        var mockMfaId = "2";
        var mockPassword = "password";

        var testAccountRequest = new TestAccountRequest();

        testAccountRequest.UserId = (USER_ID_TYPE, string.Empty);
        testAccountRequest.MfaId = (MFA_ID_TYPE, mockMfaId);
        testAccountRequest.Password = (PASSWORD_TYPE, mockPassword);

        var errorIsThrown = false;

        // Act
        try
        {
            var createAccountResponse = await appUserManagementService.CreateAccount(testAccountRequest);
        }
        catch (ArgumentNullException)
        {
            errorIsThrown = true;
        }


        // Assert
        Assert.True(errorIsThrown);
    }

    [Fact]
    public async void AppUserManagementServiceCreateAccountShould_ThrowArgumentNullErrorIfAccountDetailsIsNull()
    {
        //Arrange
        var appUserManagementService = new AppUserManagementService();

        var mockUserId = "1";

        var testAccountRequest = new TestAccountRequest();

        testAccountRequest.UserId = (USER_ID_TYPE, mockUserId);
        testAccountRequest.MfaId = (MFA_ID_TYPE, string.Empty);
        testAccountRequest.Password = (PASSWORD_TYPE, string.Empty);

        var errorIsThrown = false;

        // Act
        try
        {
            var createAccountResponse = await appUserManagementService.CreateAccount(testAccountRequest);
        }
        catch (ArgumentNullException)
        {
            errorIsThrown = true;
        }


        // Assert
        Assert.True(errorIsThrown);
    }

    [Fact]
    public async void AppUserManagementServiceCreateAccountShould_ReturnAnErrorResponseIfTheAccountAlreadyExist()
    {
        //Arrange
        var appUserManagementService = new AppUserManagementService();


        var mockUserId = "1";
        var mockMfaId = "2";
        var mockPassword = "password";

        var testAccountRequest = new TestAccountRequest();

        testAccountRequest.UserId = (USER_ID_TYPE, mockUserId);
        testAccountRequest.MfaId = (MFA_ID_TYPE, mockMfaId);
        testAccountRequest.Password = (PASSWORD_TYPE, mockPassword);

        await appUserManagementService.CreateAccount(testAccountRequest);

        // Act
        var createAccountResponse = await appUserManagementService.CreateAccount(testAccountRequest);

        // Assert
        Assert.True(createAccountResponse.HasError == true);
    }

    [Fact]
    public async void AppUserManagementServiceRecoverAccountShould_RecoverUserIdWithMfaId()
    {
        //Arrange
        var timer = new Stopwatch();

        var appUserManagementService = new AppUserManagementService();

        
        var mockUserId = "MfaAccount";
        var mockMfaId = "2";
        var mockPassword = "password";

        var createAccountRequest = new TestAccountRequest();

        createAccountRequest.UserId = (USER_ID_TYPE, mockUserId);
        createAccountRequest.MfaId = (MFA_ID_TYPE, mockMfaId);
        createAccountRequest.Password = (PASSWORD_TYPE, mockPassword);

        var createAccountResponse = await appUserManagementService.CreateAccount(createAccountRequest);     

        var recoverAccountRequest = new TestAccountRequest();

        recoverAccountRequest.UserId = (USER_ID_TYPE, "");
        recoverAccountRequest.MfaId = (MFA_ID_TYPE, mockMfaId);

        // Act
        timer.Start();
        var recoverAccountResponse = await appUserManagementService.RecoverMfaAccount(recoverAccountRequest);
        timer.Stop();


        // Assert
        Assert.True(recoverAccountResponse.HasError == false);
        foreach (List<Object> responseData in recoverAccountResponse.Output)
        {
            Assert.True(responseData[0].ToString() == mockUserId);
        }

        Assert.True(timer.Elapsed.TotalSeconds <= MAX_EXECUTION_TIME_IN_SECONDS);

    }

    [Fact]
    public async void AppUserManagementServiceRecoverAccountShould_RecoverDisabledAccount()
    {
        //Arrange
        var timer = new Stopwatch();

        var appUserManagementService = new AppUserManagementService();

        var readDataOnlyDAO = new ReadDataOnlyDAO();

        
        var mockUserId = "StatusAccount";
        var mockMfaId = "2";
        var mockPassword = "password";
        var disableStatus = "Disabled";

        var createAccountRequest = new TestAccountRequest();

        createAccountRequest.UserId = (USER_ID_TYPE, mockUserId);
        createAccountRequest.MfaId = (MFA_ID_TYPE, mockMfaId);
        createAccountRequest.Password = (PASSWORD_TYPE, mockPassword);
        createAccountRequest.AccountStatus = (STATUS_TYPE, disableStatus);

        var createAccountResponse = await appUserManagementService.CreateAccount(createAccountRequest);     

        var enableStatus = "Enabled";

        var recoverAccountRequest = new TestAccountRequest();

        recoverAccountRequest.UserId = (USER_ID_TYPE, mockUserId);
        recoverAccountRequest.AccountStatus = (STATUS_TYPE, enableStatus);

        var readAccountStatusSql = $"SELECT {STATUS_TYPE} FROM {TABLE} WHERE {USER_ID_TYPE} = \"{mockUserId}\"";

        // Act
        timer.Start();
        var recoverAccountResponse = await appUserManagementService.RecoverStatusAccount(recoverAccountRequest);
        timer.Stop();

        var readAccountStatusResponse = await readDataOnlyDAO.ReadData(readAccountStatusSql);


        // Assert
        Assert.True(recoverAccountResponse.HasError == false);
        foreach (List<Object> responseData in readAccountStatusResponse.Output)
        {
            Assert.True(responseData[0].ToString() == enableStatus);
        }

        Assert.True(timer.Elapsed.TotalSeconds <= MAX_EXECUTION_TIME_IN_SECONDS);

    }

    [Fact]
    public async void AppUserManagementServiceDeleteAccountShould_DeleteAnAccountInTheDatabase()
    {
        //Arrange
        var timer = new Stopwatch();

        var appUserManagementService = new AppUserManagementService();

        var readDataOnlyDAO = new ReadDataOnlyDAO();

        var mockUserId = "1";
        var mockMfaId = "2";
        var mockPassword = "password";

        var testAccountRequest = new TestAccountRequest();

        testAccountRequest.UserId = (USER_ID_TYPE, mockUserId);
        testAccountRequest.MfaId = (MFA_ID_TYPE, mockMfaId);
        testAccountRequest.Password = (PASSWORD_TYPE, mockPassword);

        var readAccountSql = $"SELECT * FROM {TABLE} WHERE {USER_ID_TYPE} = {mockUserId}";

        // Create test account
        await appUserManagementService.CreateAccount(testAccountRequest);

        // Act
        timer.Start();
        var deleteAccountResponse = await appUserManagementService.DeleteAccount(testAccountRequest);
        timer.Stop();

        var readResponse = await readDataOnlyDAO.ReadData(readAccountSql);

        // Assert
        Assert.True(deleteAccountResponse.HasError == false);
        Assert.True(timer.Elapsed.TotalSeconds <= MAX_EXECUTION_TIME_IN_SECONDS);
        Assert.True(readResponse.Output is null);

    }

    [Fact]
    public async void AppUserManagementServiceDeleteAccountShould_ThrowArgumentNullErrorIfUserIdIsNull()
    {
        //Arrange
        var appUserManagementService = new AppUserManagementService();

        var mockMfaId = "2";
        var mockPassword = "password";

        var testAccountRequest = new TestAccountRequest();

        testAccountRequest.UserId = (USER_ID_TYPE, string.Empty);
        testAccountRequest.MfaId = (MFA_ID_TYPE, mockMfaId);
        testAccountRequest.Password = (PASSWORD_TYPE, mockPassword);

        var errorIsThrown = false;

        // Act
        try
        {
            var createAccountResponse = await appUserManagementService.DeleteAccount(testAccountRequest);
        }
        catch (ArgumentNullException)
        {
            errorIsThrown = true;
        }


        // Assert
        Assert.True(errorIsThrown);

    }

}