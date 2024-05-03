namespace Peace.Lifelog.RegistrationTest;

using Peace.Lifelog.DataAccess;
using Peace.Lifelog.UserManagement;
using Peace.Lifelog.RegistrationService;
using Peace.Lifelog.Logging;
using Peace.Lifelog.Security;   
using Peace.Lifelog.Infrastructure;
using Peace.Lifelog.UserManagementTest;


public class RegistrationServiceShould
{

    private const string EMAIL = "zarifshams@gmail.com";
    private string DOB = "2002-01-01"; //DateTime.Today.ToString("yyyy-MM-dd");
    private const string ZIPCODE = "90010";


    #region Input validation Tests

    [Fact]
    public async void RegistrationServiceShould_HaveValidInputs()
    {

        // Arrange
        var registrationService = new RegistrationService();

        // Act
        var validEmailFormatResponse = await registrationService.CheckInputValidation(EMAIL, DOB, ZIPCODE);

        // Assert
        Assert.True(validEmailFormatResponse.HasError == false);

    }

    [Fact]
    public async void RegistrationServiceShould_HaveValidEmailFormat()
    {
        // Arrange
        string invalidEmail = "invalid@com.";
        var registrationService = new RegistrationService();

        // Act
        var validEmailFormatResponse = await registrationService.CheckInputValidation(invalidEmail, DOB, ZIPCODE);

        // Assert
        Assert.True(validEmailFormatResponse.HasError == true);

    }

    [Fact]
    public async void RegistrationServiceShould_EmailBeAtLeast3Chars()
    {
        // Arrange
        string invalidEmail = "z@gmail.com";
        var registrationService = new RegistrationService();

        // Act
        var threeCharsResponse = await registrationService.CheckInputValidation(invalidEmail, DOB, ZIPCODE);

        // Assert
        Assert.True(threeCharsResponse.HasError == true);
    }

    [Fact]
    public async void RegistrationServiceShould_EmailOnlyBeAlphanumeric()
    {
        // Arrange
        string invalidEmail = "zarif&123!@gmail.com";
        var registrationService = new RegistrationService();

        // Act
        var alphanumericResponse = await registrationService.CheckInputValidation(invalidEmail, DOB, ZIPCODE);

        // Assert
        Assert.True(alphanumericResponse.HasError == true);
    }

    [Fact]
    public async void RegistrationServiceShould_HaveValidDOB()
    {
        // Arrange
        string invalidDOB = "1969-01-01";
        var registrationService = new RegistrationService();

        // Act
        var validDOBResponse = await registrationService.CheckInputValidation(EMAIL, invalidDOB, ZIPCODE);

        // Assert
        Assert.True(validDOBResponse.HasError == true);
    }

    [Fact]
    public async void RegistrationServiceShould_HaveValidZipCode()
    {
        // Arrange
        string invalidZipCode = "90624";
        var registrationService = new RegistrationService();
        const string USERWITHINVALIDZIPCODE = "EmailWithInvalidZipcode@gmail.com";

        // Act
        var validZipCodeResponse = await registrationService.RegisterNormalUser(USERWITHINVALIDZIPCODE, DOB, invalidZipCode);

        // Assert
        Assert.True(validZipCodeResponse.HasError == true);
    }

    [Fact]
    public async void RegistrationServiceShould_InputsBeNotNull()
    {
        // Arrange
        string emptyEmail = "";
        string emptyDOB = ""; // <-should this be empty or null?
        string emptyZipCode = "";
        var registrationService = new RegistrationService();

        // Act
        var inputNotNullResponse = await registrationService.CheckInputValidation(emptyEmail, emptyDOB, emptyZipCode);

        // Assert
        Assert.True(inputNotNullResponse.HasError == true);
    }


    #endregion



    #region Create user tests

    [Fact]
    public async void RegistrationServiceShould_RegisterNormalUser()
    {
        // Arrange
        // init view and delete DAO
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

        const string USEREMAIL = "NormalUser@gmail.com";
        
        string readSQL = $"SELECT UserID FROM lifelogaccount WHERE UserID = \"{USEREMAIL}\" AND Role = \"Normal\"";

        // create account and profile request
        var testLifelogAccountRequest = new LifelogAccountRequest();
        testLifelogAccountRequest.UserId = ("UserId", USEREMAIL);
        testLifelogAccountRequest.Role = ("Role", "Normal");

        var testLifelogProfileRequest = new LifelogProfileRequest();
        testLifelogProfileRequest.DOB = ("DOB", DOB);
        testLifelogProfileRequest.ZipCode = ("ZipCode", ZIPCODE);

        // create reg object
        var registrationService = new RegistrationService();
        
        // Act
        var sucessfulRegistrationResponse = await registrationService.RegisterNormalUser(USEREMAIL, DOB, ZIPCODE);
        var readResponse = await readDataOnlyDAO.ReadData(readSQL);

        // Assert
        Assert.True(sucessfulRegistrationResponse.HasError == false);
        Assert.NotNull(readResponse.Output);
        Assert.True(readResponse.Output.Count == 1);

        // cleanup
        // delete using usermanagmentservice
        await lifelogUserManagementService.DeleteLifelogUser(testLifelogAccountRequest, testLifelogProfileRequest);
    }

    [Fact]
    public async void RegistrationServiceShould_RegisterAdminUser()
    {
        // Arrange
        // init view and delete DAO
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

        const string ADMINEMAIL = "AdminUser@gmail.com";

        string readSQL = $"SELECT UserID FROM lifelogaccount WHERE UserID = \"{ADMINEMAIL}\" AND Role = \"Admin\"";

        // create account and profile request
        var testLifelogAccountRequest = new LifelogAccountRequest();
        testLifelogAccountRequest.UserId = ("UserId", ADMINEMAIL);
        testLifelogAccountRequest.Role = ("Role", "Admin");

        var testLifelogProfileRequest = new LifelogProfileRequest();
        testLifelogProfileRequest.DOB = ("DOB", DOB);
        testLifelogProfileRequest.ZipCode = ("ZipCode", ZIPCODE);

        var registrationService = new RegistrationService();

        // Act
        var sucessfulRegistrationResponse = await registrationService.RegisterAdminUser(ADMINEMAIL, DOB, ZIPCODE);
        var readResponse = await readDataOnlyDAO.ReadData(readSQL);

        // Assert
        Assert.True(sucessfulRegistrationResponse.HasError == false);
        Assert.NotNull(readResponse.Output);
        Assert.True(readResponse.Output.Count == 1);

        // Cleanup
        await lifelogUserManagementService.DeleteLifelogUser(testLifelogAccountRequest, testLifelogProfileRequest);


    }



    #endregion

}

