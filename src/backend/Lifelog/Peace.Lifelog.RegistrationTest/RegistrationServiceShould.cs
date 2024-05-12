namespace Peace.Lifelog.RegistrationTest;

using Peace.Lifelog.DataAccess;
using Peace.Lifelog.UserManagement;
using Peace.Lifelog.RegistrationService;
using Peace.Lifelog.Logging;
using Peace.Lifelog.Security;   
using Peace.Lifelog.Infrastructure;
using Peace.Lifelog.UserManagementTest;
using Peace.Lifelog.Email;


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
        ICreateDataOnlyDAO createDataOnlyDAO = new CreateDataOnlyDAO();
        IReadDataOnlyDAO readDataOnlyDAO = new ReadDataOnlyDAO();
        IUpdateDataOnlyDAO updateDataOnlyDAO = new UpdateDataOnlyDAO();
        IDeleteDataOnlyDAO deleteDataOnlyDAO = new DeleteDataOnlyDAO();
        ILogTarget logTarget = new LogTarget(createDataOnlyDAO, readDataOnlyDAO);
        ILogging logger = new Logging(logTarget);
        ISaltService saltService = new SaltService();
        IHashService hashService = new HashService();
        IEmailService emailService = new EmailService(readDataOnlyDAO, new OTPService(updateDataOnlyDAO), updateDataOnlyDAO);
    
        IUserManagmentRepo userManagementRepo = new UserManagmentRepo(createDataOnlyDAO, readDataOnlyDAO, updateDataOnlyDAO, deleteDataOnlyDAO, logger);
        AppUserManagementService appUserManagementService =  new AppUserManagementService(createDataOnlyDAO, readDataOnlyDAO, updateDataOnlyDAO, deleteDataOnlyDAO,logger);
        
        var lifelogUserManagementService = new LifelogUserManagementService(userManagementRepo, appUserManagementService, saltService, emailService, hashService);
        var registrationService = new RegistrationService(lifelogUserManagementService, logger);

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
        // Arrange
        ICreateDataOnlyDAO createDataOnlyDAO = new CreateDataOnlyDAO();
        IReadDataOnlyDAO readDataOnlyDAO = new ReadDataOnlyDAO();
        IUpdateDataOnlyDAO updateDataOnlyDAO = new UpdateDataOnlyDAO();
        IDeleteDataOnlyDAO deleteDataOnlyDAO = new DeleteDataOnlyDAO();
        ILogTarget logTarget = new LogTarget(createDataOnlyDAO, readDataOnlyDAO);
        ILogging logger = new Logging(logTarget);
        ISaltService saltService = new SaltService();
        IHashService hashService = new HashService();
        IEmailService emailService = new EmailService(readDataOnlyDAO, new OTPService(updateDataOnlyDAO), updateDataOnlyDAO);
    
        IUserManagmentRepo userManagementRepo = new UserManagmentRepo(createDataOnlyDAO, readDataOnlyDAO, updateDataOnlyDAO, deleteDataOnlyDAO, logger);
        AppUserManagementService appUserManagementService =  new AppUserManagementService(createDataOnlyDAO, readDataOnlyDAO, updateDataOnlyDAO, deleteDataOnlyDAO,logger);
        
        var lifelogUserManagementService = new LifelogUserManagementService(userManagementRepo, appUserManagementService, saltService, emailService, hashService);
        var registrationService = new RegistrationService(lifelogUserManagementService, logger);

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
        // Arrange
        ICreateDataOnlyDAO createDataOnlyDAO = new CreateDataOnlyDAO();
        IReadDataOnlyDAO readDataOnlyDAO = new ReadDataOnlyDAO();
        IUpdateDataOnlyDAO updateDataOnlyDAO = new UpdateDataOnlyDAO();
        IDeleteDataOnlyDAO deleteDataOnlyDAO = new DeleteDataOnlyDAO();
        ILogTarget logTarget = new LogTarget(createDataOnlyDAO, readDataOnlyDAO);
        ILogging logger = new Logging(logTarget);
        ISaltService saltService = new SaltService();
        IHashService hashService = new HashService();
        IEmailService emailService = new EmailService(readDataOnlyDAO, new OTPService(updateDataOnlyDAO), updateDataOnlyDAO);
    
         IUserManagmentRepo userManagementRepo = new UserManagmentRepo(createDataOnlyDAO, readDataOnlyDAO, updateDataOnlyDAO, deleteDataOnlyDAO, logger);
        AppUserManagementService appUserManagementService =  new AppUserManagementService(createDataOnlyDAO, readDataOnlyDAO, updateDataOnlyDAO, deleteDataOnlyDAO,logger);
        
        var lifelogUserManagementService = new LifelogUserManagementService(userManagementRepo, appUserManagementService, saltService, emailService, hashService);
        var registrationService = new RegistrationService(lifelogUserManagementService, logger);

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
        // Arrange
        ICreateDataOnlyDAO createDataOnlyDAO = new CreateDataOnlyDAO();
        IReadDataOnlyDAO readDataOnlyDAO = new ReadDataOnlyDAO();
        IUpdateDataOnlyDAO updateDataOnlyDAO = new UpdateDataOnlyDAO();
        IDeleteDataOnlyDAO deleteDataOnlyDAO = new DeleteDataOnlyDAO();
        ILogTarget logTarget = new LogTarget(createDataOnlyDAO, readDataOnlyDAO);
        ILogging logger = new Logging(logTarget);
        ISaltService saltService = new SaltService();
        IHashService hashService = new HashService();
        IEmailService emailService = new EmailService(readDataOnlyDAO, new OTPService(updateDataOnlyDAO), updateDataOnlyDAO);
    
         IUserManagmentRepo userManagementRepo = new UserManagmentRepo(createDataOnlyDAO, readDataOnlyDAO, updateDataOnlyDAO, deleteDataOnlyDAO, logger);
        AppUserManagementService appUserManagementService =  new AppUserManagementService(createDataOnlyDAO, readDataOnlyDAO, updateDataOnlyDAO, deleteDataOnlyDAO,logger);
        
        var lifelogUserManagementService = new LifelogUserManagementService(userManagementRepo, appUserManagementService, saltService, emailService, hashService);
        var registrationService = new RegistrationService(lifelogUserManagementService, logger);

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
        // Arrange
        ICreateDataOnlyDAO createDataOnlyDAO = new CreateDataOnlyDAO();
        IReadDataOnlyDAO readDataOnlyDAO = new ReadDataOnlyDAO();
        IUpdateDataOnlyDAO updateDataOnlyDAO = new UpdateDataOnlyDAO();
        IDeleteDataOnlyDAO deleteDataOnlyDAO = new DeleteDataOnlyDAO();
        ILogTarget logTarget = new LogTarget(createDataOnlyDAO, readDataOnlyDAO);
        ILogging logger = new Logging(logTarget);
        ISaltService saltService = new SaltService();
        IHashService hashService = new HashService();
        IEmailService emailService = new EmailService(readDataOnlyDAO, new OTPService(updateDataOnlyDAO), updateDataOnlyDAO);
    
        IUserManagmentRepo userManagementRepo = new UserManagmentRepo(createDataOnlyDAO, readDataOnlyDAO, updateDataOnlyDAO, deleteDataOnlyDAO, logger);
        AppUserManagementService appUserManagementService =  new AppUserManagementService(createDataOnlyDAO, readDataOnlyDAO, updateDataOnlyDAO, deleteDataOnlyDAO,logger);
        
        var lifelogUserManagementService = new LifelogUserManagementService(userManagementRepo, appUserManagementService, saltService, emailService, hashService);
        var registrationService = new RegistrationService(lifelogUserManagementService, logger);

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
        // Arrange
        ICreateDataOnlyDAO createDataOnlyDAO = new CreateDataOnlyDAO();
        IReadDataOnlyDAO readDataOnlyDAO = new ReadDataOnlyDAO();
        IUpdateDataOnlyDAO updateDataOnlyDAO = new UpdateDataOnlyDAO();
        IDeleteDataOnlyDAO deleteDataOnlyDAO = new DeleteDataOnlyDAO();
        ILogTarget logTarget = new LogTarget(createDataOnlyDAO, readDataOnlyDAO);
        ILogging logger = new Logging(logTarget);
        ISaltService saltService = new SaltService();
        IHashService hashService = new HashService();
        IEmailService emailService = new EmailService(readDataOnlyDAO, new OTPService(updateDataOnlyDAO), updateDataOnlyDAO);
    
        IUserManagmentRepo userManagementRepo = new UserManagmentRepo(createDataOnlyDAO, readDataOnlyDAO, updateDataOnlyDAO, deleteDataOnlyDAO, logger);
        AppUserManagementService appUserManagementService =  new AppUserManagementService(createDataOnlyDAO, readDataOnlyDAO, updateDataOnlyDAO, deleteDataOnlyDAO,logger);
        
        var lifelogUserManagementService = new LifelogUserManagementService(userManagementRepo, appUserManagementService, saltService, emailService, hashService);
        var registrationService = new RegistrationService(lifelogUserManagementService, logger);
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
        // Arrange
        ICreateDataOnlyDAO createDataOnlyDAO = new CreateDataOnlyDAO();
        IReadDataOnlyDAO readDataOnlyDAO = new ReadDataOnlyDAO();
        IUpdateDataOnlyDAO updateDataOnlyDAO = new UpdateDataOnlyDAO();
        IDeleteDataOnlyDAO deleteDataOnlyDAO = new DeleteDataOnlyDAO();
        ILogTarget logTarget = new LogTarget(createDataOnlyDAO, readDataOnlyDAO);
        ILogging logger = new Logging(logTarget);
        ISaltService saltService = new SaltService();
        IHashService hashService = new HashService();
        IEmailService emailService = new EmailService(readDataOnlyDAO, new OTPService(updateDataOnlyDAO), updateDataOnlyDAO);
    
         IUserManagmentRepo userManagementRepo = new UserManagmentRepo(createDataOnlyDAO, readDataOnlyDAO, updateDataOnlyDAO, deleteDataOnlyDAO, logger);
        AppUserManagementService appUserManagementService =  new AppUserManagementService(createDataOnlyDAO, readDataOnlyDAO, updateDataOnlyDAO, deleteDataOnlyDAO,logger);
        
        var lifelogUserManagementService = new LifelogUserManagementService(userManagementRepo, appUserManagementService, saltService, emailService, hashService);
        var registrationService = new RegistrationService(lifelogUserManagementService, logger);

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
        // Arrange
        ICreateDataOnlyDAO createDataOnlyDAO = new CreateDataOnlyDAO();
        IReadDataOnlyDAO readDataOnlyDAO = new ReadDataOnlyDAO();
        IUpdateDataOnlyDAO updateDataOnlyDAO = new UpdateDataOnlyDAO();
        IDeleteDataOnlyDAO deleteDataOnlyDAO = new DeleteDataOnlyDAO();
        ILogTarget logTarget = new LogTarget(createDataOnlyDAO, readDataOnlyDAO);
        ILogging logger = new Logging(logTarget);
        ISaltService saltService = new SaltService();
        IHashService hashService = new HashService();
        IEmailService emailService = new EmailService(readDataOnlyDAO, new OTPService(updateDataOnlyDAO), updateDataOnlyDAO);
    
        IUserManagmentRepo userManagementRepo = new UserManagmentRepo(createDataOnlyDAO, readDataOnlyDAO, updateDataOnlyDAO, deleteDataOnlyDAO, logger);
        AppUserManagementService appUserManagementService =  new AppUserManagementService(createDataOnlyDAO, readDataOnlyDAO, updateDataOnlyDAO, deleteDataOnlyDAO,logger);
        
        var lifelogUserManagementService = new LifelogUserManagementService(userManagementRepo, appUserManagementService, saltService, emailService, hashService);
        var registrationService = new RegistrationService(lifelogUserManagementService, logger);

        const string USEREMAIL = "NormalUser@gmail.com";
        
        string readSQL = $"SELECT UserID FROM lifelogaccount WHERE UserID = \"{USEREMAIL}\" AND Role = \"Normal\"";

        // create account and profile request
        var testLifelogAccountRequest = new LifelogAccountRequest();
        testLifelogAccountRequest.UserId = ("UserId", USEREMAIL);
        testLifelogAccountRequest.Role = ("Role", "Normal");

        var testLifelogProfileRequest = new LifelogProfileRequest();
        testLifelogProfileRequest.DOB = ("DOB", DOB);
        testLifelogProfileRequest.ZipCode = ("ZipCode", ZIPCODE);
        
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
        // Arrange
        ICreateDataOnlyDAO createDataOnlyDAO = new CreateDataOnlyDAO();
        IReadDataOnlyDAO readDataOnlyDAO = new ReadDataOnlyDAO();
        IUpdateDataOnlyDAO updateDataOnlyDAO = new UpdateDataOnlyDAO();
        IDeleteDataOnlyDAO deleteDataOnlyDAO = new DeleteDataOnlyDAO();
        ILogTarget logTarget = new LogTarget(createDataOnlyDAO, readDataOnlyDAO);
        ILogging logger = new Logging(logTarget);
        ISaltService saltService = new SaltService();
        IHashService hashService = new HashService();
        IEmailService emailService = new EmailService(readDataOnlyDAO, new OTPService(updateDataOnlyDAO), updateDataOnlyDAO);
    
        IUserManagmentRepo userManagementRepo = new UserManagmentRepo(createDataOnlyDAO, readDataOnlyDAO, updateDataOnlyDAO, deleteDataOnlyDAO, logger);
        AppUserManagementService appUserManagementService =  new AppUserManagementService(createDataOnlyDAO, readDataOnlyDAO, updateDataOnlyDAO, deleteDataOnlyDAO,logger);
        
        var lifelogUserManagementService = new LifelogUserManagementService(userManagementRepo, appUserManagementService, saltService, emailService, hashService);
        var registrationService = new RegistrationService(lifelogUserManagementService, logger);

        const string ADMINEMAIL = "AdminUser@gmail.com";

        string readSQL = $"SELECT UserID FROM lifelogaccount WHERE UserID = \"{ADMINEMAIL}\" AND Role = \"Admin\"";

        // create account and profile request
        var testLifelogAccountRequest = new LifelogAccountRequest();
        testLifelogAccountRequest.UserId = ("UserId", ADMINEMAIL);
        testLifelogAccountRequest.Role = ("Role", "Admin");

        var testLifelogProfileRequest = new LifelogProfileRequest();
        testLifelogProfileRequest.DOB = ("DOB", DOB);
        testLifelogProfileRequest.ZipCode = ("ZipCode", ZIPCODE);


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

