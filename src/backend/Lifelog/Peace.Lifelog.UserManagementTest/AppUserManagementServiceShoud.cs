namespace Peace.Lifelog.UserManagementTest;

using Peace.Lifelog.DataAccess;
using Peace.Lifelog.UserManagement;
using System.Diagnostics;

public class AppUserManagementServiceShould : IDisposable
{
    // Setup for all test
    public AppUserManagementServiceShould()
    {

        var DDLTransactionDAO = new DDLTransactionDAO();
        var createMockAccountTableSql = $"CREATE TABLE {TestVariables.ACCOUNT_TABLE} ("
        + $"{TestVariables.USER_ID_TYPE} varchar(32) NOT NULL," +
        $"{TestVariables.MFA_ID_TYPE} varchar(9) NOT NULL," +
        $"{TestVariables.USER_HASH_TYPE} varchar(32) NOT NULL," +
        $"{TestVariables.CREATION_DATE_TYPE} date NOT NULL," +
        $"{TestVariables.PASSWORD_TYPE} varchar(32) NOT NULL," +
        $"{TestVariables.STATUS_TYPE} varchar(10) NOT NULL," +
        $"PRIMARY KEY ({TestVariables.USER_ID_TYPE})" +
        ");";

        var createMockProfileTable = $"CREATE TABLE {TestVariables.PROFILE_TABLE} ("
        + $"{TestVariables.USER_ID_TYPE} varchar(32) NOT NULL," +
        $"{TestVariables.DOB_TYPE} date NOT NULL," +
        $"{TestVariables.ZIP_CODE_TYPE} varchar(10) NOT NULL," +
        $"CONSTRAINT LifelogProfile_pk PRIMARY KEY ({TestVariables.USER_ID_TYPE})" +
        ");";

        var foreignKeyConstraint = $"ALTER TABLE {TestVariables.PROFILE_TABLE} ADD CONSTRAINT Table_4_LifelogAccount "
        + $"FOREIGN KEY Table_4_LifelogAccount ({TestVariables.USER_ID_TYPE}) " +
        $"REFERENCES {TestVariables.ACCOUNT_TABLE} ({TestVariables.USER_ID_TYPE}) " +
        "ON DELETE CASCADE " +
        "ON UPDATE CASCADE;";

        DDLTransactionDAO.ExecuteDDLCommand(createMockAccountTableSql);
        DDLTransactionDAO.ExecuteDDLCommand(createMockProfileTable);
        DDLTransactionDAO.ExecuteDDLCommand(foreignKeyConstraint);
    }

    // Cleanup for all tests
    public async void Dispose()
    {
        var DDLTransactionDAO = new DDLTransactionDAO();

        var deleteMockAccountTableSql = $"DROP TABLE {TestVariables.ACCOUNT_TABLE}";
        var deleteMockProfileTableSql = $"DROP TABLE {TestVariables.PROFILE_TABLE}";

        await DDLTransactionDAO.ExecuteDDLCommand(deleteMockProfileTableSql);
        await DDLTransactionDAO.ExecuteDDLCommand(deleteMockAccountTableSql);

    }

    #region Create Account Tests
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
        var mockUserHash = ")@#$*%!)#%*!dgjwodwnjvon";

        var testAccountRequest = new TestAccountRequest();

        testAccountRequest.UserId = (TestVariables.USER_ID_TYPE, mockUserId);
        testAccountRequest.MfaId = (TestVariables.MFA_ID_TYPE, mockMfaId);
        testAccountRequest.UserHash = (TestVariables.USER_HASH_TYPE, mockUserHash);
        testAccountRequest.CreationDate = (TestVariables.CREATION_DATE_TYPE, DateTime.Now.ToString("yyyy-MM-dd"));
        testAccountRequest.Password = (TestVariables.PASSWORD_TYPE, mockPassword);

        var readAccountSql = $"SELECT * FROM {TestVariables.ACCOUNT_TABLE} WHERE {TestVariables.USER_ID_TYPE} = {mockUserId}";

        // Act
        timer.Start();
        var createAccountResponse = await appUserManagementService.CreateAccount(testAccountRequest);
        timer.Stop();

        var readResponse = await readDataOnlyDAO.ReadData(readAccountSql);


        // Assert
        Assert.True(createAccountResponse.HasError == false);
        Assert.True(timer.Elapsed.TotalSeconds <= TestVariables.MAX_EXECUTION_TIME_IN_SECONDS);
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

        testAccountRequest.UserId = (TestVariables.USER_ID_TYPE, string.Empty);

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

        testAccountRequest.UserId = (TestVariables.USER_ID_TYPE, mockUserId);

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
        var mockUserHash = ")@#$*%!)#%*!dgjwodwnjvon";

        var testAccountRequest = new TestAccountRequest();

        testAccountRequest.UserId = (TestVariables.USER_ID_TYPE, mockUserId);
        testAccountRequest.MfaId = (TestVariables.MFA_ID_TYPE, mockMfaId);
        testAccountRequest.UserHash = (TestVariables.USER_HASH_TYPE, mockUserHash);
        testAccountRequest.CreationDate = (TestVariables.CREATION_DATE_TYPE, DateTime.Now.ToString("yyyy-MM-dd"));
        testAccountRequest.Password = (TestVariables.PASSWORD_TYPE, mockPassword);

        await appUserManagementService.CreateAccount(testAccountRequest);

        // Act
        var createAccountResponse = await appUserManagementService.CreateAccount(testAccountRequest);

        // Assert
        Assert.True(createAccountResponse.HasError == true);
    }

    [Fact]
    public async void AppUserManagementServiceCreateAccountShould_ReturnAnErrorResponseIfTheRequestResultInInvalidSQL()
    {
        //Arrange
        var appUserManagementService = new AppUserManagementService();

        var wrongUserIdType = "Phone";
        var mockUserId = "1";
        var mockMfaId = "2";
        var mockPassword = "password";
        var mockUserHash = ")@#$*%!)#%*!dgjwodwnjvon";

        var testAccountRequest = new TestAccountRequest();

        testAccountRequest.UserId = (wrongUserIdType, mockUserId);
        testAccountRequest.MfaId = (TestVariables.MFA_ID_TYPE, mockMfaId);
        testAccountRequest.UserHash = (TestVariables.USER_HASH_TYPE, mockUserHash);
        testAccountRequest.CreationDate = (TestVariables.CREATION_DATE_TYPE, DateTime.Now.ToString("yyyy-MM-dd"));
        testAccountRequest.Password = (TestVariables.PASSWORD_TYPE, mockPassword);

        await appUserManagementService.CreateAccount(testAccountRequest);

        // Act
        var createAccountResponse = await appUserManagementService.CreateAccount(testAccountRequest);

        // Assert
        Assert.True(createAccountResponse.HasError == true);
    }
    #endregion

    #region Recover Account Tests
    [Fact]
    public async void AppUserManagementServiceRecoverAccountShould_RecoverUserIdWithMfaId()
    {
        //Arrange
        var timer = new Stopwatch();

        var appUserManagementService = new AppUserManagementService();

        var mockUserId = "2";
        var mockMfaId = "3";
        var mockPassword = "password";
        var mockUserHash = ")@#$*%!)#%*!dgjwodwnjvon";

        var createAccountRequest = new TestAccountRequest();

        createAccountRequest.UserId = (TestVariables.USER_ID_TYPE, mockUserId);
        createAccountRequest.MfaId = (TestVariables.MFA_ID_TYPE, mockMfaId);
        createAccountRequest.UserHash = (TestVariables.USER_HASH_TYPE, mockUserHash);
        createAccountRequest.CreationDate = (TestVariables.CREATION_DATE_TYPE, DateTime.Now.ToString("yyyy-MM-dd"));
        createAccountRequest.Password = (TestVariables.PASSWORD_TYPE, mockPassword);

        var createAccountResponse = await appUserManagementService.CreateAccount(createAccountRequest);

        var recoverAccountRequest = new TestAccountRequest();

        recoverAccountRequest.UserId = (TestVariables.USER_ID_TYPE, mockUserId);
        recoverAccountRequest.MfaId = (TestVariables.MFA_ID_TYPE, mockMfaId);

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

        Assert.True(timer.Elapsed.TotalSeconds <= TestVariables.MAX_EXECUTION_TIME_IN_SECONDS);

    }

    [Fact]
    public async void AppUserManagementServiceRecoverAccountShould_RecoverDisabledAccount()
    {
        //Arrange
        var timer = new Stopwatch();

        var appUserManagementService = new AppUserManagementService();

        var readDataOnlyDAO = new ReadDataOnlyDAO();


        var mockUserId = "2";
        var mockMfaId = "3";
        var mockPassword = "password";
        var mockUserHash = ")@#$*%!)#%*!dgjwodwnjvon";
        var disabledStatus = "Disabled";

        var createAccountRequest = new TestAccountRequest();

        createAccountRequest.UserId = (TestVariables.USER_ID_TYPE, mockUserId);
        createAccountRequest.MfaId = (TestVariables.MFA_ID_TYPE, mockMfaId);
        createAccountRequest.UserHash = (TestVariables.USER_HASH_TYPE, mockUserHash);
        createAccountRequest.CreationDate = (TestVariables.CREATION_DATE_TYPE, DateTime.Now.ToString("yyyy-MM-dd"));
        createAccountRequest.Password = (TestVariables.PASSWORD_TYPE, mockPassword);
        createAccountRequest.AccountStatus = (TestVariables.STATUS_TYPE, disabledStatus);

        var createAccountResponse = await appUserManagementService.CreateAccount(createAccountRequest);

        var enableStatus = "Enabled";

        var recoverAccountRequest = new TestAccountRequest();

        recoverAccountRequest.UserId = (TestVariables.USER_ID_TYPE, mockUserId);
        recoverAccountRequest.AccountStatus = (TestVariables.STATUS_TYPE, enableStatus);

        var readAccountStatusSql = $"SELECT {TestVariables.STATUS_TYPE} FROM {TestVariables.ACCOUNT_TABLE} WHERE {TestVariables.USER_ID_TYPE} = \"{mockUserId}\"";

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

        Assert.True(timer.Elapsed.TotalSeconds <= TestVariables.MAX_EXECUTION_TIME_IN_SECONDS);

    }

    [Fact]
    public async void AppUserManagementServiceRecoverStatusAccountShould_ReturnArgumentNullExceptionWithNullUserId()
    {
        // Arrange
        var appUserManagementService = new AppUserManagementService();
        var recoverAccountRequest = new TestAccountRequest();

        string mockUserId = null;
        var enableStatus = "Enabled";

        recoverAccountRequest.UserId = (TestVariables.USER_ID_TYPE, mockUserId);
        recoverAccountRequest.AccountStatus = (TestVariables.STATUS_TYPE, enableStatus);

        var errorIsThrown = false;

        // Act
        try
        {
            var recoverProfileResponse = await appUserManagementService.RecoverStatusAccount(recoverAccountRequest);
        }
        catch (ArgumentNullException)
        {
            errorIsThrown = true;
        }

        // Assert
        Assert.True(errorIsThrown);
    }

    [Fact]
    public async void AppUserManagementServiceRecoverStatusAccountShould_ReturnArgumentNullExceptionWithNullAccountStatus()
    {
        // Arrange
        var appUserManagementService = new AppUserManagementService();
        var recoverAccountRequest = new TestAccountRequest();

        string mockUserId = "phongNeedsBetterVariableNames";
        string enableStatus = null;

        recoverAccountRequest.UserId = (TestVariables.USER_ID_TYPE, mockUserId);
        recoverAccountRequest.AccountStatus = (TestVariables.STATUS_TYPE, enableStatus);

        var errorIsThrown = false;

        // Act
        try
        {
            var recoverProfileResponse = await appUserManagementService.RecoverStatusAccount(recoverAccountRequest);
        }
        catch (ArgumentNullException)
        {
            errorIsThrown = true;
        }

        // Assert
        Assert.True(errorIsThrown);
    }

    [Fact]
    public async void AppUserManagementServiceRecoverStatusAccountShould_ReturnAnErrorResponseIfAccountDoesNotExist()
    {
        // Arrange
        var appUserManagementService = new AppUserManagementService();
        var recoverAccountRequest = new TestAccountRequest();

        string mockUserId = "jackDoesNotExist";
        var enableStatus = "Enabled";

        recoverAccountRequest.UserId = (TestVariables.USER_ID_TYPE, mockUserId);
        recoverAccountRequest.AccountStatus = (TestVariables.STATUS_TYPE, enableStatus);

        // Act
        var recoverProfileResponse = await appUserManagementService.RecoverStatusAccount(recoverAccountRequest);

        // Assert
        Assert.True(recoverProfileResponse.HasError);
    }

    [Fact]
    public async void AppUserManagementServiceRecoverStatusAccountShould_ReturnAnErrorResponseIfRequestResultInIncorrectSQL()
    {
        // Arrange
        var appUserManagementService = new AppUserManagementService();
        var recoverAccountRequest = new TestAccountRequest();

        string mockUserId = "jackDoesNotExist";
        var enableStatus = "Enabled";
        var wrongUserIdType = "Phone";

        recoverAccountRequest.UserId = (wrongUserIdType, mockUserId);
        recoverAccountRequest.AccountStatus = (TestVariables.STATUS_TYPE, enableStatus);

        // Act
        var recoverProfileResponse = await appUserManagementService.RecoverStatusAccount(recoverAccountRequest);

        // Assert
        Assert.True(recoverProfileResponse.HasError);
    }

    [Fact]
    public async void AppUserManagementServiceRecoverMfaAccountShould_ReturnArgumentNullExceptionWithNullUserId()
    {
        // Arrange
        var appUserManagementService = new AppUserManagementService();
        var recoverAccountRequest = new TestAccountRequest();

        string mockUserId = null;
        var enableStatus = "Enabled";

        recoverAccountRequest.UserId = (TestVariables.USER_ID_TYPE, mockUserId);
        recoverAccountRequest.AccountStatus = (TestVariables.STATUS_TYPE, enableStatus);

        var errorIsThrown = false;

        // Act
        try
        {
            var recoverProfileResponse = await appUserManagementService.RecoverMfaAccount(recoverAccountRequest);
        }
        catch (ArgumentNullException)
        {
            errorIsThrown = true;
        }

        // Assert
        Assert.True(errorIsThrown);
    }

    [Fact]
    public async void AppUserManagementServiceRecoverMfaAccountShould_ReturnArgumentNullExceptionWithNullMfaId()
    {
        // Arrange
        var appUserManagementService = new AppUserManagementService();
        var recoverAccountRequest = new TestAccountRequest();

        string mockUserId = "phongNeedsBetterVariableNames";

        recoverAccountRequest.UserId = (TestVariables.USER_ID_TYPE, mockUserId);
        recoverAccountRequest.MfaId = (TestVariables.MFA_ID_TYPE, string.Empty);

        var errorIsThrown = false;

        // Act
        try
        {
            var recoverProfileResponse = await appUserManagementService.RecoverMfaAccount(recoverAccountRequest);
        }
        catch (ArgumentNullException)
        {
            errorIsThrown = true;
        }

        // Assert
        Assert.True(errorIsThrown);
    }

    [Fact]
    public async void AppUserManagementServiceRecoverMfaAccountShould_ReturnAnErrorResponseIfAccountDoesNotExist()
    {
        // Arrange
        var appUserManagementService = new AppUserManagementService();
        var recoverAccountRequest = new TestAccountRequest();

        string mockUserId = "jackDoesNotExist";
        string mockMfaId = "multiFactorIsForTheBrids";

        recoverAccountRequest.UserId = (TestVariables.USER_ID_TYPE, mockUserId);
        recoverAccountRequest.MfaId = (TestVariables.MFA_ID_TYPE, mockMfaId);

        // Act
        var recoverProfileResponse = await appUserManagementService.RecoverMfaAccount(recoverAccountRequest);

        // Assert
        Assert.True(recoverProfileResponse.HasError);
    }

    [Fact]
    public async void AppUserManagementServiceRecoverMfaAccountShould_ReturnAnErrorResponseIfRequestResultInIncorrectSQL()
    {
        // Arrange
        var appUserManagementService = new AppUserManagementService();
        var recoverAccountRequest = new TestAccountRequest();

        string mockUserId = "jackDoesNotExist";
        string mockMfaId = "multiFactorIsForTheBrids";
        var enableStatus = "Enabled";
        var wrongUserIdType = "Phone";

        recoverAccountRequest.UserId = (wrongUserIdType, mockUserId);
        recoverAccountRequest.MfaId = (TestVariables.MFA_ID_TYPE, mockMfaId);
        recoverAccountRequest.AccountStatus = (TestVariables.STATUS_TYPE, enableStatus);

        // Act
        var recoverProfileResponse = await appUserManagementService.RecoverMfaAccount(recoverAccountRequest);

        // Assert
        Assert.True(recoverProfileResponse.HasError);
    }
    #endregion

    #region Modify Profile Tests
    [Fact]
    public async void AppUserManagementServiceModifyProfileShould_ModifyTheProfileInTheDatabase()
    {
        //Arrange
        var timer = new Stopwatch();

        var appUserManagementService = new AppUserManagementService();

        var createDataOnlyDAO = new CreateDataOnlyDAO();
        var readDataOnlyDAO = new ReadDataOnlyDAO();


        // Creating User Account
        var mockUserId = "1";
        var mockMfaId = "3";
        var mockPassword = "password";
        var mockUserHash = ")@#$*%!)#%*!dgjwodwnjvon";

        var testAccountRequest = new TestAccountRequest();

        testAccountRequest.UserId = (TestVariables.USER_ID_TYPE, mockUserId);
        testAccountRequest.MfaId = (TestVariables.MFA_ID_TYPE, mockMfaId);
        testAccountRequest.UserHash = (TestVariables.USER_HASH_TYPE, mockUserHash);
        testAccountRequest.CreationDate = (TestVariables.CREATION_DATE_TYPE, DateTime.Now.ToString("yyyy-MM-dd"));
        testAccountRequest.Password = (TestVariables.PASSWORD_TYPE, mockPassword);

        await appUserManagementService.CreateAccount(testAccountRequest);

        // Creating User Profile based off User Account
        var mockDob = DateTime.Now.ToString("yyyy-MM-dd");
        var oldZipCode = "12345-6789";
        var newZipCode = "54321-9876";

        var createProfileSql = $"INSERT INTO {TestVariables.PROFILE_TABLE} ({TestVariables.USER_ID_TYPE}, {TestVariables.DOB_TYPE}, {TestVariables.ZIP_CODE_TYPE}) "
                                + $"VALUES (\"{mockUserId}\", \"{mockDob}\", \"{oldZipCode}\")";

        var readProfileSql = $"SELECT {TestVariables.ZIP_CODE_TYPE} FROM {TestVariables.PROFILE_TABLE} WHERE {TestVariables.USER_ID_TYPE} = \"{mockUserId}\"";

        await createDataOnlyDAO.CreateData(createProfileSql);

        var testProfileRequest = new TestProfileRequest();

        testProfileRequest.UserId = (TestVariables.USER_ID_TYPE, mockUserId);
        testProfileRequest.ZipCode = (TestVariables.ZIP_CODE_TYPE, newZipCode);


        // Act
        timer.Start();
        var modifyProfileResponse = await appUserManagementService.ModifyProfile(testProfileRequest);
        timer.Stop();

        var readResponse = await readDataOnlyDAO.ReadData(readProfileSql);


        // Assert
        Assert.True(modifyProfileResponse.HasError == false);
        foreach (List<Object> responseData in readResponse.Output)
        {
            Assert.True(responseData[0].ToString() == newZipCode);
        }

        Assert.True(timer.Elapsed.TotalSeconds <= TestVariables.MAX_EXECUTION_TIME_IN_SECONDS);

    }

    [Fact]
    public async void AppUserManagementServiceModifyProfileShould_ThrowArgumentNullExceptionIfUserIdIsNull()
    {
        //Arrange
        var timer = new Stopwatch();

        var appUserManagementService = new AppUserManagementService();

        var testProfileRequest = new TestProfileRequest();

        testProfileRequest.UserId = (TestVariables.USER_ID_TYPE, string.Empty);

        var errorIsThrown = false;

        //Act
        try
        {
            var createProfileResponse = await appUserManagementService.ModifyProfile(testProfileRequest);
        }
        catch (ArgumentNullException)
        {
            errorIsThrown = true;
        }

        //Assert
        timer.Start();
        Assert.True(errorIsThrown);
        timer.Stop();
    }

    [Fact]
    public async void AppUserManagementServiceModifyProfileShould_ThrowArgumentNullExceptionIfUserDetailsIsNull()
    {
        //Arrange
        var timer = new Stopwatch();

        var appUserManagementService = new AppUserManagementService();

        var testProfileRequest = new TestProfileRequest();

        var user = "user007";

        testProfileRequest.UserId = (TestVariables.USER_ID_TYPE, user);
        testProfileRequest.ZipCode = (TestVariables.ZIP_CODE_TYPE, string.Empty);
        testProfileRequest.DOB = (TestVariables.DOB_TYPE, string.Empty);

        var errorIsThrown = false;

        //Act
        try
        {
            var createProfileResponse = await appUserManagementService.ModifyProfile(testProfileRequest);
        }
        catch (ArgumentNullException)
        {
            errorIsThrown = true;
        }

        //Assert
        timer.Start();
        Assert.True(errorIsThrown);
        timer.Stop();
    }

    [Fact]
    public async void AppUserManagementServiceModifyProfileShould_ReturnAnErrorResponseIfUserAttemptToModifyNonexistingAccount()
    {
        //Arrange
        var timer = new Stopwatch();

        var appUserManagementService = new AppUserManagementService();

        var mockUserId = "7";
        var newZipCode = "54321-9876";

        var testProfileRequest = new TestProfileRequest();

        testProfileRequest.UserId = (TestVariables.USER_ID_TYPE, mockUserId);
        testProfileRequest.ZipCode = (TestVariables.ZIP_CODE_TYPE, newZipCode);

        // Act
        timer.Start();
        var modifyProfileResponse = await appUserManagementService.ModifyProfile(testProfileRequest);
        timer.Stop();

        // Assert
        Assert.True(modifyProfileResponse.HasError == true);
        Assert.True(timer.Elapsed.TotalSeconds <= TestVariables.MAX_EXECUTION_TIME_IN_SECONDS);
    }

    [Fact]
    public async void AppUserManagementServiceModifyProfileShould_ReturnAnErrorResponseIfRequestResultInInvalidSQL()
    {
        //Arrange
        var timer = new Stopwatch();

        var appUserManagementService = new AppUserManagementService();

        var mockUserId = "7";
        var newZipCode = "54321-9876";
        var wrongUserIdType = "Phone";

        var testProfileRequest = new TestProfileRequest();

        testProfileRequest.UserId = (wrongUserIdType, mockUserId);
        testProfileRequest.ZipCode = (TestVariables.ZIP_CODE_TYPE, newZipCode);

        // Act
        timer.Start();
        var modifyProfileResponse = await appUserManagementService.ModifyProfile(testProfileRequest);
        timer.Stop();

        // Assert
        Assert.True(modifyProfileResponse.HasError == true);
        Assert.True(timer.Elapsed.TotalSeconds <= TestVariables.MAX_EXECUTION_TIME_IN_SECONDS);
    }
    #endregion

    #region Delete Account Tests

    [Fact]
    public async void AppUserManagementServiceDeleteAccountShould_DeleteAnAccountInTheDatabase()
    {
        //Arrange
        var timer = new Stopwatch();

        var appUserManagementService = new AppUserManagementService();

        var readDataOnlyDAO = new ReadDataOnlyDAO();

        var mockUserId = "2";
        var mockMfaId = "3";
        var mockPassword = "password";
        var mockUserHash = ")@#$*%!)#%*!dgjwodwnjvon";

        var testAccountRequest = new TestAccountRequest();

        testAccountRequest.UserId = (TestVariables.USER_ID_TYPE, mockUserId);
        testAccountRequest.MfaId = (TestVariables.MFA_ID_TYPE, mockMfaId);
        testAccountRequest.UserHash = (TestVariables.USER_HASH_TYPE, mockUserHash);
        testAccountRequest.CreationDate = (TestVariables.CREATION_DATE_TYPE, DateTime.Now.ToString("yyyy-MM-dd"));
        testAccountRequest.Password = (TestVariables.PASSWORD_TYPE, mockPassword);

        var createAccountResponse = await appUserManagementService.CreateAccount(testAccountRequest);

        // Create test account
        await appUserManagementService.CreateAccount(testAccountRequest);


        var readAccountSql = $"SELECT * FROM {TestVariables.ACCOUNT_TABLE} WHERE {TestVariables.USER_ID_TYPE} = {mockUserId}";

        // Act
        timer.Start();
        var deleteAccountResponse = await appUserManagementService.DeleteAccount(testAccountRequest);
        timer.Stop();

        var readResponse = await readDataOnlyDAO.ReadData(readAccountSql);

        // Assert
        Assert.True(deleteAccountResponse.HasError == false);
        Assert.True(timer.Elapsed.TotalSeconds <= TestVariables.MAX_EXECUTION_TIME_IN_SECONDS);
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

        testAccountRequest.UserId = (TestVariables.USER_ID_TYPE, string.Empty);
        testAccountRequest.MfaId = (TestVariables.MFA_ID_TYPE, mockMfaId);
        testAccountRequest.Password = (TestVariables.PASSWORD_TYPE, mockPassword);

        var errorIsThrown = false;

        // Act
        try
        {
            var deleteAccountResponse = await appUserManagementService.DeleteAccount(testAccountRequest);
        }
        catch (ArgumentNullException)
        {
            errorIsThrown = true;
        }

        // Assert
        Assert.True(errorIsThrown);
    }

    [Fact]
    public async void AppUserManagementServiceDeleteAccountShould_ReturnAnErrorResponseIfAccountDoesntExist()
    {
        //Arrange
        var appUserManagementService = new AppUserManagementService();

        var mockUserId = "accountDoesntExist";
        var mockMfaId = "2";
        var mockPassword = "password";

        var testAccountRequest = new TestAccountRequest();

        testAccountRequest.UserId = (TestVariables.USER_ID_TYPE, mockUserId);
        testAccountRequest.MfaId = (TestVariables.MFA_ID_TYPE, mockMfaId);
        testAccountRequest.Password = (TestVariables.PASSWORD_TYPE, mockPassword);

        // Act
        var deleteAccountResponse = await appUserManagementService.DeleteAccount(testAccountRequest);



        // Assert
        Assert.True(deleteAccountResponse.HasError == true);
    }

    [Fact]
    public async void AppUserManagementServiceDeleteAccountShould_ReturnAnErrorResponseIfRequestResultInInvalidSQL()
    {
        //Arrange
        var appUserManagementService = new AppUserManagementService();

        var mockUserId = "mockUserId";
        var mockMfaId = "2";
        var mockPassword = "password";
        var wrongUserIdType = "Phone";

        var testAccountRequest = new TestAccountRequest();

        testAccountRequest.UserId = (wrongUserIdType, mockUserId);
        testAccountRequest.MfaId = (TestVariables.MFA_ID_TYPE, mockMfaId);
        testAccountRequest.Password = (TestVariables.PASSWORD_TYPE, mockPassword);

        // Act
        var deleteAccountResponse = await appUserManagementService.DeleteAccount(testAccountRequest);



        // Assert
        Assert.True(deleteAccountResponse.HasError == true);
    }
    #endregion
}