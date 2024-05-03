namespace Peace.Lifelog.UserManagementTest;

using Peace.Lifelog.DataAccess;
using Peace.Lifelog.UserManagement;
using System.Diagnostics;
using Peace.Lifelog.Infrastructure;
using Peace.Lifelog.Logging;
using Peace.Lifelog.Security;

public class LifelogUserManagementServiceShould
{

    #region Create Life Log User Tests
    [Fact]
    public async void LifelogUserManagementServiceCreateLifelogUserShould_CreateAnAccountInTheDatabase()
    {
        //Arrange
        // Create Test User Account
        CreateDataOnlyDAO createDataOnlyDAO = new CreateDataOnlyDAO();
        IReadDataOnlyDAO readDataOnlyDAO = new ReadDataOnlyDAO();
        IUpdateDataOnlyDAO updateDataOnlyDAO = new UpdateDataOnlyDAO();
        IDeleteDataOnlyDAO deleteDataOnlyDAO = new DeleteDataOnlyDAO();
        ILogTarget logTarget = new LogTarget(createDataOnlyDAO, readDataOnlyDAO);
        ILogging logger = new Logging(logTarget);
        SaltService saltService = new SaltService();
    
        IUserManagmentRepo userManagementRepo = new UserManagmentRepo(readDataOnlyDAO, deleteDataOnlyDAO, logger);
        AppUserManagementService appUserManagementService =  new AppUserManagementService();
        
        var lifelogUserManagementService = new LifelogUserManagementService(userManagementRepo, appUserManagementService, saltService, createDataOnlyDAO);
        var timer = new Stopwatch();


        var mockUserId = "TestUserCreation";
        var mockRole = "Normal";

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
        var createAccountResponse = await lifelogUserManagementService.CreateLifelogUser(testLifelogAccountRequest, testLifelogProfileRequest);
        timer.Stop();

        var readResponse = await readDataOnlyDAO.ReadData(readAccountSql);


        // Assert
        Assert.True(createAccountResponse.HasError == false);
        Assert.True(timer.Elapsed.TotalSeconds <= TestVariables.MAX_EXECUTION_TIME_IN_SECONDS);
        Assert.True(readResponse.Output != null);
        Assert.True(readResponse.Output.Count == 1);

        //Cleanup
        var deleteAccountResponse = await lifelogUserManagementService.DeleteLifelogUser(testLifelogAccountRequest, testLifelogProfileRequest);

    }

    [Fact]
    public async void LifelogUserManagementServiceCreateLifelogUserShould_ThrowArgumentNullErrorIfUserIdIsNull()
    {
        //Arrange
        // Create Test User Account
        CreateDataOnlyDAO createDataOnlyDAO = new CreateDataOnlyDAO();
        IReadDataOnlyDAO readDataOnlyDAO = new ReadDataOnlyDAO();
        IUpdateDataOnlyDAO updateDataOnlyDAO = new UpdateDataOnlyDAO();
        IDeleteDataOnlyDAO deleteDataOnlyDAO = new DeleteDataOnlyDAO();
        ILogTarget logTarget = new LogTarget(createDataOnlyDAO, readDataOnlyDAO);
        ILogging logger = new Logging(logTarget);
        SaltService saltService = new SaltService();
    
        IUserManagmentRepo userManagementRepo = new UserManagmentRepo(readDataOnlyDAO, deleteDataOnlyDAO, logger);
        AppUserManagementService appUserManagementService =  new AppUserManagementService();
        
        var lifelogUserManagementService = new LifelogUserManagementService(userManagementRepo, appUserManagementService, saltService, createDataOnlyDAO);


        var mockRole = "Normal";

        // Creating User Profile based off User Account
        var mockDob = DateTime.Now.ToString("yyyy-MM-dd");
        var mockZipCode = "92612";

        var testLifelogAccountRequest = new LifelogAccountRequest();
        testLifelogAccountRequest.UserId = ("UserId", string.Empty);
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
        // Create Test User Account
        CreateDataOnlyDAO createDataOnlyDAO = new CreateDataOnlyDAO();
        IReadDataOnlyDAO readDataOnlyDAO = new ReadDataOnlyDAO();
        IUpdateDataOnlyDAO updateDataOnlyDAO = new UpdateDataOnlyDAO();
        IDeleteDataOnlyDAO deleteDataOnlyDAO = new DeleteDataOnlyDAO();
        ILogTarget logTarget = new LogTarget(createDataOnlyDAO, readDataOnlyDAO);
        ILogging logger = new Logging(logTarget);
        SaltService saltService = new SaltService();
    
        IUserManagmentRepo userManagementRepo = new UserManagmentRepo(readDataOnlyDAO, deleteDataOnlyDAO, logger);
        AppUserManagementService appUserManagementService =  new AppUserManagementService();
        
        var lifelogUserManagementService = new LifelogUserManagementService(userManagementRepo, appUserManagementService, saltService, createDataOnlyDAO);

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
        // Create Test User Account
        CreateDataOnlyDAO createDataOnlyDAO = new CreateDataOnlyDAO();
        IReadDataOnlyDAO readDataOnlyDAO = new ReadDataOnlyDAO();
        IUpdateDataOnlyDAO updateDataOnlyDAO = new UpdateDataOnlyDAO();
        IDeleteDataOnlyDAO deleteDataOnlyDAO = new DeleteDataOnlyDAO();
        ILogTarget logTarget = new LogTarget(createDataOnlyDAO, readDataOnlyDAO);
        ILogging logger = new Logging(logTarget);
        SaltService saltService = new SaltService();
    
        IUserManagmentRepo userManagementRepo = new UserManagmentRepo(readDataOnlyDAO, deleteDataOnlyDAO, logger);
        AppUserManagementService appUserManagementService =  new AppUserManagementService();
        
        var lifelogUserManagementService = new LifelogUserManagementService(userManagementRepo, appUserManagementService, saltService, createDataOnlyDAO);

        var mockUserId = "TestUserCreation";
        var mockRole = "Normal";

        // Creating User Profile based off User Account
        var mockDob = DateTime.Now.ToString("yyyy-MM-dd");
        var mockZipCode = "92612";


        var testLifelogAccountRequest = new LifelogAccountRequest();
        testLifelogAccountRequest.UserId = ("UserId", mockUserId);
        testLifelogAccountRequest.Role = ("Role", mockRole);

        var testLifelogProfileRequest = new LifelogProfileRequest();
        testLifelogProfileRequest.DOB = ("DOB", mockDob);
        testLifelogProfileRequest.ZipCode = ("ZipCode", mockZipCode);

        await lifelogUserManagementService.CreateLifelogUser(testLifelogAccountRequest, testLifelogProfileRequest);

        // Act
        var createAccountResponse = await lifelogUserManagementService.CreateLifelogUser(testLifelogAccountRequest, testLifelogProfileRequest);

        // Assert
        Assert.True(createAccountResponse.HasError == true);

        //Cleanup
        var deleteAccountResponse = await lifelogUserManagementService.DeleteLifelogUser(testLifelogAccountRequest, testLifelogProfileRequest);
    }

    [Fact]
    public async void LifelogUserManagementServiceCreateLifelogUserShould_ReturnAnErrorResponseIfTheRequestResultInInvalidSQL()
    {
        //Arrange
        // Create Test User Account
        CreateDataOnlyDAO createDataOnlyDAO = new CreateDataOnlyDAO();
        IReadDataOnlyDAO readDataOnlyDAO = new ReadDataOnlyDAO();
        IUpdateDataOnlyDAO updateDataOnlyDAO = new UpdateDataOnlyDAO();
        IDeleteDataOnlyDAO deleteDataOnlyDAO = new DeleteDataOnlyDAO();
        ILogTarget logTarget = new LogTarget(createDataOnlyDAO, readDataOnlyDAO);
        ILogging logger = new Logging(logTarget);
        SaltService saltService = new SaltService();
    
        IUserManagmentRepo userManagementRepo = new UserManagmentRepo(readDataOnlyDAO, deleteDataOnlyDAO, logger);
        AppUserManagementService appUserManagementService =  new AppUserManagementService();
        
        var lifelogUserManagementService = new LifelogUserManagementService(userManagementRepo, appUserManagementService, saltService, createDataOnlyDAO);

        var mockUserId = "TestInvalidSQLCreation";
        var mockRole = "Normal";

        // Creating User Profile based off User Account
        var mockDob = DateTime.Now.ToString("yyyy-MM-dd");
        var mockZipCode = "92612";


        var testLifelogAccountRequest = new LifelogAccountRequest();
        testLifelogAccountRequest.UserId = ("WrongType", mockUserId);
        testLifelogAccountRequest.Role = ("Role", mockRole);

        var testLifelogProfileRequest = new LifelogProfileRequest();
        testLifelogProfileRequest.DOB = ("DOB", mockDob);
        testLifelogProfileRequest.ZipCode = ("ZipCode", mockZipCode);

        // Act
        var createAccountResponse = await lifelogUserManagementService.CreateLifelogUser(testLifelogAccountRequest, testLifelogProfileRequest);

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

        // Create Test User Account
        CreateDataOnlyDAO createDataOnlyDAO = new CreateDataOnlyDAO();
        IReadDataOnlyDAO readDataOnlyDAO = new ReadDataOnlyDAO();
        IUpdateDataOnlyDAO updateDataOnlyDAO = new UpdateDataOnlyDAO();
        IDeleteDataOnlyDAO deleteDataOnlyDAO = new DeleteDataOnlyDAO();
        ILogTarget logTarget = new LogTarget(createDataOnlyDAO, readDataOnlyDAO);
        ILogging logger = new Logging(logTarget);
        SaltService saltService = new SaltService();
    
        IUserManagmentRepo userManagementRepo = new UserManagmentRepo(readDataOnlyDAO, deleteDataOnlyDAO, logger);
        AppUserManagementService appUserManagementService =  new AppUserManagementService();
        
        var lifelogUserManagementService = new LifelogUserManagementService(userManagementRepo, appUserManagementService, saltService, createDataOnlyDAO);



        var mockUserId = "TestUserCreation";
        var mockRole = "Normal";

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
        // Create Test User Account
        CreateDataOnlyDAO createDataOnlyDAO = new CreateDataOnlyDAO();
        IReadDataOnlyDAO readDataOnlyDAO = new ReadDataOnlyDAO();
        IUpdateDataOnlyDAO updateDataOnlyDAO = new UpdateDataOnlyDAO();
        IDeleteDataOnlyDAO deleteDataOnlyDAO = new DeleteDataOnlyDAO();
        ILogTarget logTarget = new LogTarget(createDataOnlyDAO, readDataOnlyDAO);
        ILogging logger = new Logging(logTarget);
        SaltService saltService = new SaltService();
    
        IUserManagmentRepo userManagementRepo = new UserManagmentRepo(readDataOnlyDAO, deleteDataOnlyDAO, logger);
        AppUserManagementService appUserManagementService =  new AppUserManagementService();
        
        var lifelogUserManagementService = new LifelogUserManagementService(userManagementRepo, appUserManagementService, saltService, createDataOnlyDAO);

        var mockRole = "Normal";

        // Creating User Profile based off User Account
        var mockDob = DateTime.Now.ToString("yyyy-MM-dd");
        var mockZipCode = "92612";

        var testLifelogAccountRequest = new LifelogAccountRequest();
        testLifelogAccountRequest.UserId = ("UserId", string.Empty);
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
        // Create Test User Account
        CreateDataOnlyDAO createDataOnlyDAO = new CreateDataOnlyDAO();
        IReadDataOnlyDAO readDataOnlyDAO = new ReadDataOnlyDAO();
        IUpdateDataOnlyDAO updateDataOnlyDAO = new UpdateDataOnlyDAO();
        IDeleteDataOnlyDAO deleteDataOnlyDAO = new DeleteDataOnlyDAO();
        ILogTarget logTarget = new LogTarget(createDataOnlyDAO, readDataOnlyDAO);
        ILogging logger = new Logging(logTarget);
        SaltService saltService = new SaltService();
    
        IUserManagmentRepo userManagementRepo = new UserManagmentRepo(readDataOnlyDAO, deleteDataOnlyDAO, logger);
        AppUserManagementService appUserManagementService =  new AppUserManagementService();
        
        var lifelogUserManagementService = new LifelogUserManagementService(userManagementRepo, appUserManagementService, saltService, createDataOnlyDAO);

        var mockRole = "Normal";

        // Creating User Profile based off User Account
        var mockDob = DateTime.Now.ToString("yyyy-MM-dd");
        var mockZipCode = "92612";

        var testLifelogAccountRequest = new LifelogAccountRequest();
        testLifelogAccountRequest.UserId = ("UserId", "userdoesntexist");
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
        // Create Test User Account
        CreateDataOnlyDAO createDataOnlyDAO = new CreateDataOnlyDAO();
        IReadDataOnlyDAO readDataOnlyDAO = new ReadDataOnlyDAO();
        IUpdateDataOnlyDAO updateDataOnlyDAO = new UpdateDataOnlyDAO();
        IDeleteDataOnlyDAO deleteDataOnlyDAO = new DeleteDataOnlyDAO();
        ILogTarget logTarget = new LogTarget(createDataOnlyDAO, readDataOnlyDAO);
        ILogging logger = new Logging(logTarget);
        SaltService saltService = new SaltService();
    
        IUserManagmentRepo userManagementRepo = new UserManagmentRepo(readDataOnlyDAO, deleteDataOnlyDAO, logger);
        AppUserManagementService appUserManagementService =  new AppUserManagementService();
        
        var lifelogUserManagementService = new LifelogUserManagementService(userManagementRepo, appUserManagementService, saltService, createDataOnlyDAO);

        var mockUserId = "TestUserCreation";
        var mockRole = "Normal";

        // Creating User Profile based off User Account
        var mockDob = DateTime.Now.ToString("yyyy-MM-dd");
        var mockZipCode = "92612";

        var testLifelogAccountRequest = new LifelogAccountRequest();
        testLifelogAccountRequest.UserId = ("UserId", mockUserId);
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

    #region Modify Profile Tests
    [Fact]
    public async void LifelogUserManagementServiceModifyLifelogProfileShould_ModifyAProfileInTheDatabase()
    {
        //Arrange
        var timer = new Stopwatch();
        // Create Test User Account
        CreateDataOnlyDAO createDataOnlyDAO = new CreateDataOnlyDAO();
        IReadDataOnlyDAO readDataOnlyDAO = new ReadDataOnlyDAO();
        IUpdateDataOnlyDAO updateDataOnlyDAO = new UpdateDataOnlyDAO();
        IDeleteDataOnlyDAO deleteDataOnlyDAO = new DeleteDataOnlyDAO();
        ILogTarget logTarget = new LogTarget(createDataOnlyDAO, readDataOnlyDAO);
        ILogging logger = new Logging(logTarget);
        SaltService saltService = new SaltService();
    
        IUserManagmentRepo userManagementRepo = new UserManagmentRepo(readDataOnlyDAO, deleteDataOnlyDAO, logger);
        AppUserManagementService appUserManagementService =  new AppUserManagementService();
        
        var lifelogUserManagementService = new LifelogUserManagementService(userManagementRepo, appUserManagementService, saltService, createDataOnlyDAO);

        var mockUserId = "TestUserModify";
        var mockRole = "Normal";
        var USER_HASH = "";
        // Creating User Profile based off User Account
        var mockDob = DateTime.Now.ToString("yyyy-MM-dd");
        var mockZipCode = "90704";
        var newZipCode = "90007";



        var testLifelogAccountRequest = new LifelogAccountRequest();
        testLifelogAccountRequest.UserId = ("UserId", mockUserId);
        testLifelogAccountRequest.Role = ("Role", mockRole);

        var testLifelogProfileRequest = new LifelogProfileRequest();
        testLifelogProfileRequest.DOB = ("DOB", mockDob);
        testLifelogProfileRequest.ZipCode = ("ZipCode", mockZipCode);

        var createAccountResponse = await lifelogUserManagementService.CreateLifelogUser(testLifelogAccountRequest, testLifelogProfileRequest);

        if (createAccountResponse.Output is not null)
        {
            foreach (string output in createAccountResponse.Output)
            {
                USER_HASH = output;
            }
        }

        var readAccountSql = $"SELECT * FROM LifelogProfile WHERE UserHash = \"{USER_HASH}\"";

        testLifelogProfileRequest.ZipCode = ("ZipCode", newZipCode);


        // Act
        timer.Start();
        var modifyProfileResponse = await lifelogUserManagementService.ModifyLifelogUser(testLifelogProfileRequest);
        timer.Stop();


        var readResponse = await readDataOnlyDAO.ReadData(readAccountSql);


        // Assert
        Assert.True(modifyProfileResponse.HasError == false);
        Assert.True(timer.Elapsed.TotalSeconds <= TestVariables.MAX_EXECUTION_TIME_IN_SECONDS);
        Assert.True(readResponse.Output != null);
        foreach (List<Object> responseData in readResponse.Output)
        {

            Assert.True(responseData[2].ToString() == newZipCode);
        }

        //Cleanup
        var deleteAccountResponse = await lifelogUserManagementService.DeleteLifelogUser(testLifelogAccountRequest, testLifelogProfileRequest);

    }

    [Fact]
    public async void LifelogUserManagementServiceModifyLifelogProfileShould_ThrowArgumentNullExceptionIfUserIdIsNull()
    {
        //Arrange
        var timer = new Stopwatch();

        // Create Test User Account
        CreateDataOnlyDAO createDataOnlyDAO = new CreateDataOnlyDAO();
        IReadDataOnlyDAO readDataOnlyDAO = new ReadDataOnlyDAO();
        IUpdateDataOnlyDAO updateDataOnlyDAO = new UpdateDataOnlyDAO();
        IDeleteDataOnlyDAO deleteDataOnlyDAO = new DeleteDataOnlyDAO();
        ILogTarget logTarget = new LogTarget(createDataOnlyDAO, readDataOnlyDAO);
        ILogging logger = new Logging(logTarget);
        SaltService saltService = new SaltService();
    
        IUserManagmentRepo userManagementRepo = new UserManagmentRepo(readDataOnlyDAO, deleteDataOnlyDAO, logger);
        AppUserManagementService appUserManagementService =  new AppUserManagementService();
        
        var lifelogUserManagementService = new LifelogUserManagementService(userManagementRepo, appUserManagementService, saltService, createDataOnlyDAO);

        var testLifelogProfileRequest = new LifelogProfileRequest();

        testLifelogProfileRequest.UserId = (TestVariables.USER_ID_TYPE, string.Empty);

        var errorIsThrown = false;

        //Act
        try
        {
            var modifyProfileResponse = await lifelogUserManagementService.ModifyLifelogUser(testLifelogProfileRequest);
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
    public async void LifelogUserManagementServiceModifyLifelogUserShould_ThrowArgumentNullErrorIfUserDetailsIsNull()
    {
        //Arrange
        // Create Test User Account
        CreateDataOnlyDAO createDataOnlyDAO = new CreateDataOnlyDAO();
        IReadDataOnlyDAO readDataOnlyDAO = new ReadDataOnlyDAO();
        IUpdateDataOnlyDAO updateDataOnlyDAO = new UpdateDataOnlyDAO();
        IDeleteDataOnlyDAO deleteDataOnlyDAO = new DeleteDataOnlyDAO();
        ILogTarget logTarget = new LogTarget(createDataOnlyDAO, readDataOnlyDAO);
        ILogging logger = new Logging(logTarget);
        SaltService saltService = new SaltService();
    
        IUserManagmentRepo userManagementRepo = new UserManagmentRepo(readDataOnlyDAO, deleteDataOnlyDAO, logger);
        AppUserManagementService appUserManagementService =  new AppUserManagementService();
        
        var lifelogUserManagementService = new LifelogUserManagementService(userManagementRepo, appUserManagementService, saltService, createDataOnlyDAO);

        // Creating User Profile with null user details

        var testLifelogProfileRequest = new LifelogProfileRequest();
        testLifelogProfileRequest.ZipCode = (TestVariables.ZIP_CODE_TYPE, string.Empty);
        testLifelogProfileRequest.DOB = (TestVariables.DOB_TYPE, string.Empty);

        var errorIsThrown = false;

        // Act
        try
        {
            var modifyProfileResponse = await lifelogUserManagementService.ModifyLifelogUser(testLifelogProfileRequest);
        }
        catch (ArgumentNullException)
        {
            errorIsThrown = true;
        }

        // Assert
        Assert.True(errorIsThrown);
    }

    [Fact]
    public async void LifelogUserManagementServiceModifyLifelogUserShould_ReturnAnErrorResponseIfUserAttemptToModifyNonexistingAccount()
    {
        //Arrange
        var timer = new Stopwatch();

        // Create Test User Account
        CreateDataOnlyDAO createDataOnlyDAO = new CreateDataOnlyDAO();
        IReadDataOnlyDAO readDataOnlyDAO = new ReadDataOnlyDAO();
        IUpdateDataOnlyDAO updateDataOnlyDAO = new UpdateDataOnlyDAO();
        IDeleteDataOnlyDAO deleteDataOnlyDAO = new DeleteDataOnlyDAO();
        ILogTarget logTarget = new LogTarget(createDataOnlyDAO, readDataOnlyDAO);
        ILogging logger = new Logging(logTarget);
        SaltService saltService = new SaltService();
    
        IUserManagmentRepo userManagementRepo = new UserManagmentRepo(readDataOnlyDAO, deleteDataOnlyDAO, logger);
        AppUserManagementService appUserManagementService =  new AppUserManagementService();
        
        var lifelogUserManagementService = new LifelogUserManagementService(userManagementRepo, appUserManagementService, saltService, createDataOnlyDAO);

        var mockUserId = "7";
        var newZipCode = "54321-9876";

        var testProfileRequest = new LifelogProfileRequest();

        testProfileRequest.UserId = (TestVariables.USER_ID_TYPE, mockUserId);
        testProfileRequest.ZipCode = (TestVariables.ZIP_CODE_TYPE, newZipCode);

        // Act
        timer.Start();
        var modifyProfileResponse = await lifelogUserManagementService.ModifyLifelogUser(testProfileRequest);
        timer.Stop();

        // Assert
        Assert.True(modifyProfileResponse.HasError == true);
        Assert.True(timer.Elapsed.TotalSeconds <= TestVariables.MAX_EXECUTION_TIME_IN_SECONDS);
    }
    #endregion

    #region Recover User Test
    [Fact]
    public async void LifelogUserManagementServiceRecoverAccountShould_RecoverDisabledAccount()
    {
        //Arrange
        var timer = new Stopwatch();

        // Create Test User Account
        CreateDataOnlyDAO createDataOnlyDAO = new CreateDataOnlyDAO();
        IReadDataOnlyDAO readDataOnlyDAO = new ReadDataOnlyDAO();
        IUpdateDataOnlyDAO updateDataOnlyDAO = new UpdateDataOnlyDAO();
        IDeleteDataOnlyDAO deleteDataOnlyDAO = new DeleteDataOnlyDAO();
        ILogTarget logTarget = new LogTarget(createDataOnlyDAO, readDataOnlyDAO);
        ILogging logger = new Logging(logTarget);
        SaltService saltService = new SaltService();
    
        IUserManagmentRepo userManagementRepo = new UserManagmentRepo(readDataOnlyDAO, deleteDataOnlyDAO, logger);
        AppUserManagementService appUserManagementService =  new AppUserManagementService();
        
        var lifelogUserManagementService = new LifelogUserManagementService(userManagementRepo, appUserManagementService, saltService, createDataOnlyDAO);


        var mockUserId = "TestUserRecovery";
        var mockRole = "Normal";

        // Creating User Profile based off User Account
        var mockDob = DateTime.Now.ToString("yyyy-MM-dd");
        var mockZipCode = "90704";


        var testLifelogAccountRequest = new LifelogAccountRequest();
        testLifelogAccountRequest.UserId = ("UserId", mockUserId);
        testLifelogAccountRequest.Role = ("Role", mockRole);
        testLifelogAccountRequest.AccountStatus = ("AccountStatus", "Disabled");

        var testLifelogProfileRequest = new LifelogProfileRequest();
        testLifelogProfileRequest.DOB = ("DOB", mockDob);
        testLifelogProfileRequest.ZipCode = ("ZipCode", mockZipCode);

        var createAccountResponse = await lifelogUserManagementService.CreateLifelogUser(testLifelogAccountRequest, testLifelogProfileRequest);

        var enableStatus = "Enabled";

        var recoverAccountRequest = new LifelogAccountRequest();

        recoverAccountRequest.UserId = ("UserId", mockUserId);
        recoverAccountRequest.AccountStatus = ("AccountStatus", enableStatus);

        var readAccountStatusSql = $"SELECT AccountStatus FROM LifelogAccount WHERE UserId = \"{mockUserId}\"";

        // Act
        timer.Start();
        var recoverAccountResponse = await lifelogUserManagementService.RecoverLifelogUser(recoverAccountRequest);
        timer.Stop();

        var readAccountStatusResponse = await readDataOnlyDAO.ReadData(readAccountStatusSql);


        // Assert
        Assert.True(recoverAccountResponse.HasError == false);
        Assert.True(readAccountStatusResponse.Output != null);
        foreach (List<Object> responseData in readAccountStatusResponse.Output)
        {
            Assert.True(responseData[0].ToString() == enableStatus);
        }

        Assert.True(timer.Elapsed.TotalSeconds <= TestVariables.MAX_EXECUTION_TIME_IN_SECONDS);

        //Cleanup
        var deleteAccountResponse = await lifelogUserManagementService.DeleteLifelogUser(testLifelogAccountRequest, testLifelogProfileRequest);

    }

    [Fact]
    public async void LifelogUserManagementServiceRecoverAccountShould_ReturnArgumentNullExceptionWithNullUserId()
    {
        //Arrange
        var timer = new Stopwatch();

        // Create Test User Account
        CreateDataOnlyDAO createDataOnlyDAO = new CreateDataOnlyDAO();
        IReadDataOnlyDAO readDataOnlyDAO = new ReadDataOnlyDAO();
        IUpdateDataOnlyDAO updateDataOnlyDAO = new UpdateDataOnlyDAO();
        IDeleteDataOnlyDAO deleteDataOnlyDAO = new DeleteDataOnlyDAO();
        ILogTarget logTarget = new LogTarget(createDataOnlyDAO, readDataOnlyDAO);
        ILogging logger = new Logging(logTarget);
        SaltService saltService = new SaltService();
    
        IUserManagmentRepo userManagementRepo = new UserManagmentRepo(readDataOnlyDAO, deleteDataOnlyDAO, logger);
        AppUserManagementService appUserManagementService =  new AppUserManagementService();
        
        var lifelogUserManagementService = new LifelogUserManagementService(userManagementRepo, appUserManagementService, saltService, createDataOnlyDAO);

        var mockUserId = "";
        var mockRole = "Normal";

        var testLifelogAccountRequest = new LifelogAccountRequest();
        testLifelogAccountRequest.UserId = ("UserId", mockUserId);
        testLifelogAccountRequest.Role = ("Role", mockRole);
        testLifelogAccountRequest.AccountStatus = ("AccountStatus", "Disabled");

        var errorIsThrown = false;

        // Act
        try
        {
            var recoverAccountResponse = await lifelogUserManagementService.RecoverLifelogUser(testLifelogAccountRequest);
        }
        catch (ArgumentNullException)
        {
            errorIsThrown = true;
        }

        // Assert
        Assert.True(errorIsThrown);

    }

    [Fact]
    public async void LifelogUserManagementServiceRecoverAccountShould_ReturnArgumentNullExceptionWithNullAccountStatus()
    {
        // Arrange
        // Create Test User Account
        CreateDataOnlyDAO createDataOnlyDAO = new CreateDataOnlyDAO();
        IReadDataOnlyDAO readDataOnlyDAO = new ReadDataOnlyDAO();
        IUpdateDataOnlyDAO updateDataOnlyDAO = new UpdateDataOnlyDAO();
        IDeleteDataOnlyDAO deleteDataOnlyDAO = new DeleteDataOnlyDAO();
        ILogTarget logTarget = new LogTarget(createDataOnlyDAO, readDataOnlyDAO);
        ILogging logger = new Logging(logTarget);
        SaltService saltService = new SaltService();
    
        IUserManagmentRepo userManagementRepo = new UserManagmentRepo(readDataOnlyDAO, deleteDataOnlyDAO, logger);
        AppUserManagementService appUserManagementService =  new AppUserManagementService();
        
        var lifelogUserManagementService = new LifelogUserManagementService(userManagementRepo, appUserManagementService, saltService, createDataOnlyDAO);
        var recoverAccountRequest = new LifelogAccountRequest();

        string mockUserId = "phongNeedsBetterVariableNames";
        string enableStatus = "";

        recoverAccountRequest.UserId = ("UserId", mockUserId);
        recoverAccountRequest.AccountStatus = ("AccountStatus", enableStatus);

        var errorIsThrown = false;

        // Act
        try
        {
            var recoverProfileResponse = await lifelogUserManagementService.RecoverLifelogUser(recoverAccountRequest);
        }
        catch (ArgumentNullException)
        {
            errorIsThrown = true;
        }

        // Assert
        Assert.True(errorIsThrown);
    }

    [Fact]
    public async void LifelogUserManagementServiceRecoverAccountShould_ReturnAnErrorResponseIfAccountDoesNotExist()
    {
        // Arrange
        // Create Test User Account
        CreateDataOnlyDAO createDataOnlyDAO = new CreateDataOnlyDAO();
        IReadDataOnlyDAO readDataOnlyDAO = new ReadDataOnlyDAO();
        IUpdateDataOnlyDAO updateDataOnlyDAO = new UpdateDataOnlyDAO();
        IDeleteDataOnlyDAO deleteDataOnlyDAO = new DeleteDataOnlyDAO();
        ILogTarget logTarget = new LogTarget(createDataOnlyDAO, readDataOnlyDAO);
        ILogging logger = new Logging(logTarget);
        SaltService saltService = new SaltService();
    
        IUserManagmentRepo userManagementRepo = new UserManagmentRepo(readDataOnlyDAO, deleteDataOnlyDAO, logger);
        AppUserManagementService appUserManagementService =  new AppUserManagementService();
        
        var lifelogUserManagementService = new LifelogUserManagementService(userManagementRepo, appUserManagementService, saltService, createDataOnlyDAO);
        var recoverAccountRequest = new LifelogAccountRequest();

        string mockUserId = "jackDoesNotExist";
        var enableStatus = "Enabled";

        recoverAccountRequest.UserId = ("UserId", mockUserId);
        recoverAccountRequest.AccountStatus = ("AccountStatus", enableStatus);

        // Act
        var recoverProfileResponse = await lifelogUserManagementService.RecoverLifelogUser(recoverAccountRequest);

        // Assert
        Assert.True(recoverProfileResponse.HasError);
    }
    #endregion

}