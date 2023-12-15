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

    // Setup for all test
    public AppUserManagementServiceShould()
    {

        var DDLTransactionDAO = new DDLTransactionDAO();
        var createMockTableSql = $"CREATE TABLE {TABLE} ("
        + $"{USER_ID_TYPE} varchar(32) NOT NULL," +
        $"{MFA_ID_TYPE} varchar(9) NOT NULL," +
        $"{PASSWORD_TYPE} varchar(32) NOT NULL," +
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
    public async void AppUserManagementServiceShould_CreateAnAccountInTheDatabase()
    {
        //Arrange
        var timer = new Stopwatch();

        var appUserManagementService = new AppUserManagementService();

        var readDataOnlyDAO = new ReadDataOnlyDAO();

        var mockUserId = "1";
        var mockMfaId = "2";
        var mockPassword = "password";

        var testAccount = new TestAccount();

        testAccount.UserId = (USER_ID_TYPE, mockUserId);
        testAccount.MfaId = (MFA_ID_TYPE, mockMfaId);
        testAccount.Password = (PASSWORD_TYPE, mockPassword);

        var readAccountSql = $"SELECT * FROM {TABLE} WHERE {USER_ID_TYPE} = {mockUserId}";

        // Act
        timer.Start();
        var createAccountResponse = await appUserManagementService.CreateAccount(testAccount);
        timer.Stop();

        var readResponse = await readDataOnlyDAO.ReadData(readAccountSql);


        // Assert
        Assert.True(createAccountResponse.HasError == false);
        Assert.True(timer.Elapsed.TotalSeconds <= MAX_EXECUTION_TIME_IN_SECONDS);
        Assert.True(readResponse.Output.Count == 1);

    }

    [Fact]
    public async void AppUserManagementServiceShould_ThrowArgumentNullErrorIfUserIdIsNull()
    {
        //Arrange
        var appUserManagementService = new AppUserManagementService();

        var mockMfaId = "2";
        var mockPassword = "password";

        var testAccount = new TestAccount();

        testAccount.UserId = (USER_ID_TYPE, string.Empty);
        testAccount.MfaId = (MFA_ID_TYPE, mockMfaId);
        testAccount.Password = (PASSWORD_TYPE, mockPassword);

        var errorIsThrown = false;

        // Act
        try
        {
            var createAccountResponse = await appUserManagementService.CreateAccount(testAccount);
        }
        catch (ArgumentNullException)
        {
            errorIsThrown = true;
        }


        // Assert
        Assert.True(errorIsThrown);
    }

    [Fact]
    public async void AppUserManagementServiceShould_ThrowArgumentNullErrorIfAccountDetailsIsNull()
    {
        //Arrange
        var appUserManagementService = new AppUserManagementService();

        var mockUserId = "1";

        var testAccount = new TestAccount();

        testAccount.UserId = (USER_ID_TYPE, mockUserId);
        testAccount.MfaId = (MFA_ID_TYPE, string.Empty);
        testAccount.Password = (PASSWORD_TYPE, string.Empty);

        var errorIsThrown = false;

        // Act
        try
        {
            var createAccountResponse = await appUserManagementService.CreateAccount(testAccount);
        }
        catch (ArgumentNullException)
        {
            errorIsThrown = true;
        }


        // Assert
        Assert.True(errorIsThrown);
    }

    [Fact]
    public async void AppUserManagementServiceShould_ReturnAnErrorResponseIfTheAccountAlreadyExist()
    {
        //Arrange
        var appUserManagementService = new AppUserManagementService();


        var mockUserId = "1";
        var mockMfaId = "2";
        var mockPassword = "password";

        var testAccount = new TestAccount();

        testAccount.UserId = (USER_ID_TYPE, mockUserId);
        testAccount.MfaId = (MFA_ID_TYPE, mockMfaId);
        testAccount.Password = (PASSWORD_TYPE, mockPassword);

        await appUserManagementService.CreateAccount(testAccount);

        // Act
        var createAccountResponse = await appUserManagementService.CreateAccount(testAccount);

        // Assert
        Assert.True(createAccountResponse.HasError == true);
    }

}