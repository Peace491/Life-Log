namespace Peace.Lifelog.MapTest;

using DomainModels;
using Peace.Lifelog.DataAccess;
using Peace.Lifelog.Infrastructure;
using Peace.Lifelog.Logging;
using Peace.Lifelog.Map;
using Peace.Lifelog.Security;
using Peace.Lifelog.UserManagement;
using System.Diagnostics;
using Peace.Lifelog.Email;

public class PinServiceShould : IAsyncLifetime, IDisposable
{
    private static CreateDataOnlyDAO createDataOnlyDAO = new CreateDataOnlyDAO();
    private static ReadDataOnlyDAO readDataOnlyDAO = new ReadDataOnlyDAO();
    private static UpdateDataOnlyDAO updateDataOnlyDAO = new UpdateDataOnlyDAO();
    private static DeleteDataOnlyDAO deleteDataOnlyDAO = new DeleteDataOnlyDAO();
    private static LogTarget logTarget = new LogTarget(createDataOnlyDAO, readDataOnlyDAO);
    private static Logging logging = new Logging(logTarget);
    private static AppAuthService appAuthService = new AppAuthService();
    private static ILifelogAuthService lifelogAuthService = new LifelogAuthService(appAuthService, new UserManagmentRepo(createDataOnlyDAO, readDataOnlyDAO, updateDataOnlyDAO, deleteDataOnlyDAO, logging));
    private static IMapRepo mapRepo = new MapRepo(createDataOnlyDAO, readDataOnlyDAO, updateDataOnlyDAO, deleteDataOnlyDAO);
    private PinService pinService = new PinService(mapRepo, lifelogAuthService, logging);
    private const string USER_ID = "TestMapServiceAccount";
    private string USER_HASH = "";
    private const string ROLE = "Normal";

    private string DOB = DateTime.Today.ToString("yyyy-MM-dd");
    private const string ZIP_CODE = "90701";
    //double check principal declratoin
    private AppPrincipal? PRINCIPAL = new AppPrincipal();
    private string PINID = "TestPinID";
    private string LLIID = "TestLLIID";
    private string ADDRESS = "Test address";
    private double LATITUDE = 100.00001;
    private double LONGITUDE = 100.00001;
    private Stopwatch timer = new Stopwatch();

    public async Task InitializeAsync()
    {
        // Create Test User Account
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

        var testLifelogAccountRequest = new LifelogAccountRequest();
        testLifelogAccountRequest.UserId = ("UserId", USER_ID);
        testLifelogAccountRequest.Role = ("Role", ROLE);

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
        var testLifelogAccountRequest = new LifelogAccountRequest();
        testLifelogAccountRequest.UserId = ("UserId", USER_ID);
        var deleteAccountResponse = appUserManagementService.DeleteAccount(testLifelogAccountRequest);
    }

    #region Create Pin Tests
    // [Fact]
    // public async Task PinServiceCreatePinShould_CreateAPinInTheDatabase()
    // {
    //     //Arrange

    //     var testPin = new CreatePinRequest();
    //     testPin.Principal = PRINCIPAL;
    //     testPin.PinId = PINID;
    //     testPin.LLIId = LLIID;
    //     testPin.UserHash = USER_HASH;
    //     testPin.Address = ADDRESS;
    //     testPin.Latitude = LATITUDE;
    //     testPin.Longitude = LONGITUDE;


    //     // Act
    //     timer.Start();
    //     var createPinResponse = await pinService.CreatePin(testPin);
    //     timer.Stop();
    //     var readDataOnlyDAO = new ReadDataOnlyDAO();
    //     var readPinSql = $"SELECT PinId, LLIId, Address, Latitude, Longitude " + $"FROM MapPin Where PinId=\"{PINID}\";"; //double check this
    //     var readResponse = await readDataOnlyDAO.ReadData(readPinSql);

    //     // Assert
    //     Assert.True(createPinResponse.HasError == false);
    //     Assert.True(timer.Elapsed.TotalSeconds <= 3);
    //     Assert.NotNull(readResponse.Output);
    //     Assert.True(readResponse.Output.Count == 1);

    //     // Cleanup
    //     var deleteDataOnlyDAO = new DeleteDataOnlyDAO();

    //     var deletePinSql = $"DELETE FROM MapPin WHERE PinId=\"{PINID}\";";

    //     await deleteDataOnlyDAO.DeleteData(deletePinSql);
    // }

    [Fact]
    public async Task PinServiceCreatePinShould_ThrowAnErrorIfTimerHasPassed3Seconds()
    {
        //Arrange

        var testPin = new CreatePinRequest();
        testPin.Principal = PRINCIPAL;
        testPin.PinId = PINID;
        testPin.LLIId = LLIID;
        testPin.UserHash = USER_HASH;
        testPin.Address = ADDRESS;
        testPin.Latitude = LATITUDE;
        testPin.Longitude = LONGITUDE;


        // Act
        timer.Start();
        var createPinResponse = await pinService.CreatePin(testPin);
        var additionalTime = TimeSpan.FromSeconds(5);
        var newElapsed = timer.Elapsed + additionalTime;
        timer.Restart();
        while (timer.Elapsed < newElapsed)
        {
        }
        timer.Stop();
        var readDataOnlyDAO = new ReadDataOnlyDAO();
        var readPinSql = $"SELECT PinId, LLIId, Address, Latitude, Longitude " + $"FROM MapPin Where PinId=\"{PINID}\";"; //double check this
        var readResponse = await readDataOnlyDAO.ReadData(readPinSql);

        // Assert
        Assert.True(createPinResponse.HasError == true);
        Assert.True(timer.Elapsed.TotalSeconds > 3);
        Assert.Null(readResponse.Output);

        // Cleanup
        var deleteDataOnlyDAO = new DeleteDataOnlyDAO();

        var deletePinSql = $"DELETE FROM MapPin WHERE PinId=\"{PINID}\";";

        await deleteDataOnlyDAO.DeleteData(deletePinSql);
    }
    #endregion
    #region Implement here

    //implement failure outcome 2 here

    #endregion

    // [Fact]
    // public async Task PinServiceCreatePinShould_ThrowAnErrorIfPinAttemptsHasPassed20()
    // {
    //     //Arrange

    //     var testPin = new CreatePinRequest();
    //     testPin.Principal = PRINCIPAL;
    //     testPin.PinId = PINID;
    //     testPin.LLIId = LLIID;
    //     testPin.UserHash = USER_HASH;
    //     testPin.Address = ADDRESS;
    //     testPin.Latitude = LATITUDE;
    //     testPin.Longitude = LONGITUDE;

    //     var pin = "0";
    //     var createPinResponse = new Response();

    //     // Act
    //     for (int pinNum = 0; pinNum < 22; pinNum++)
    //     {
    //         pin = pinNum.ToString();
    //         testPin.PinId = pin;
    //         createPinResponse = await pinService.CreatePin(testPin);
    //     }
    //     timer.Stop();
    //     var readDataOnlyDAO = new ReadDataOnlyDAO();
    //     var readPinSql = $"SELECT LLIId, COUNT(*) AS count_of_pins FROM MapPin WHERE LLIId = '{LLIID}' GROUP BY LLIId;"; //double check this
    //     var readResponse = await readDataOnlyDAO.ReadData(readPinSql);

    //     // Assert
    //     Assert.True(createPinResponse.HasError == true);
    //     Assert.True(timer.Elapsed.TotalSeconds > 3);
    //     Assert.Null(readResponse.Output);

    //     // Cleanup
    //     var deleteDataOnlyDAO = new DeleteDataOnlyDAO();

    //     var deletePinSql = $"DELETE FROM MapPin WHERE LLIId=\"{LLIID}\";";

    //     await deleteDataOnlyDAO.DeleteData(deletePinSql);
    // }

    #region Delete Pin Tests
    // [Fact]
    // public async Task PinServiceDeletePinShould_DeleteAPinInTheDatabase()
    // {
    //     //Arrange

    //     var testCreatePin = new CreatePinRequest();
    //     testCreatePin.Principal = PRINCIPAL;
    //     testCreatePin.PinId = PINID;
    //     testCreatePin.LLIId = LLIID;
    //     testCreatePin.Address = ADDRESS;
    //     testCreatePin.Latitude = LATITUDE;
    //     testCreatePin.Longitude = LONGITUDE;

    //     var testDeletePin = new DeletePinRequest();
    //     testDeletePin.Principal = PRINCIPAL;
    //     testDeletePin.PinId = PINID;
    //     testDeletePin.LLIId = LLIID;
    //     testDeletePin.Address = ADDRESS;
    //     testDeletePin.Latitude = LATITUDE;
    //     testDeletePin.Longitude = LONGITUDE;


    //     // Act
    //     timer.Start();
    //     var createPinResponse = await pinService.CreatePin(testCreatePin);
    //     var deletePinResponse = await pinService.DeletePin(testDeletePin.PinId, USER_HASH);
    //     timer.Stop();
    //     var readDataOnlyDAO = new ReadDataOnlyDAO();
    //     var readPinSql = $"SELECT PinId, LLIId, Address, Latitude, Longitude " + $"FROM MapPin Where PinId=\"{PINID}\";"; //double check this
    //     var readResponse = await readDataOnlyDAO.ReadData(readPinSql);

    //     // Assert
    //     Assert.True(createPinResponse.HasError == false);
    //     Assert.True(timer.Elapsed.TotalSeconds <= 3);
    //     Assert.NotNull(readResponse.Output);
    //     Assert.True(readResponse.Output.Count == 0);
    // }

    [Fact]
    public async Task PinServiceDeletePinShould_ThrowAnErrorIfDeletePinTakesTooLong()
    {
        //Arrange

        var testCreatePin = new CreatePinRequest();
        testCreatePin.Principal = PRINCIPAL;
        testCreatePin.PinId = PINID;
        testCreatePin.LLIId = LLIID;
        testCreatePin.Address = ADDRESS;
        testCreatePin.Latitude = LATITUDE;
        testCreatePin.Longitude = LONGITUDE;

        var testDeletePin = new DeletePinRequest();
        testDeletePin.Principal = PRINCIPAL;
        testDeletePin.PinId = PINID;
        testDeletePin.LLIId = LLIID;
        testDeletePin.Address = ADDRESS;
        testDeletePin.Latitude = LATITUDE;
        testDeletePin.Longitude = LONGITUDE;


        // Act
        timer.Start();
        var createPinResponse = await pinService.CreatePin(testCreatePin);
        var deletePinResponse = await pinService.DeletePin(testDeletePin.PinId, USER_HASH);
        var additionalTime = TimeSpan.FromSeconds(5);
        var newElapsed = timer.Elapsed + additionalTime;
        timer.Restart();
        while (timer.Elapsed < newElapsed)
        {
        }
        timer.Stop();
        var readDataOnlyDAO = new ReadDataOnlyDAO();
        var readPinSql = $"SELECT PinId, LLIId, Address, Latitude, Longitude " + $"FROM MapPin Where PinId=\"{PINID}\";"; //double check this
        var readResponse = await readDataOnlyDAO.ReadData(readPinSql);

        // Assert
        Assert.True(deletePinResponse.HasError == true);
        Assert.True(timer.Elapsed.TotalSeconds > 3);
        Assert.Null(readResponse.Output);
    }
    #endregion

    #region Implement here

    //second test can be done here or prefereablly in front end

    #endregion

    #region View Pin Details Tests
    // [Fact]
    // public async Task PinServiceViewPinDetailsShould_ViewPinDetailsInTheDatabase()
    // {
    //     //Arrange

    //     var testCreatePin = new CreatePinRequest();
    //     testCreatePin.Principal = PRINCIPAL;
    //     testCreatePin.PinId = PINID;
    //     testCreatePin.LLIId = LLIID;
    //     testCreatePin.Address = ADDRESS;
    //     testCreatePin.Latitude = LATITUDE;
    //     testCreatePin.Longitude = LONGITUDE;

    //     var testViewPin = new ViewPinRequest();
    //     testViewPin.Principal = PRINCIPAL;
    //     testViewPin.PinId = PINID;
    //     testViewPin.LLIId = LLIID;
    //     testViewPin.Address = ADDRESS;
    //     testViewPin.Latitude = LATITUDE;
    //     testViewPin.Longitude = LONGITUDE;


    //     // Act
    //     timer.Start();
    //     var createPinResponse = await pinService.CreatePin(testCreatePin);
    //     var viewPinResponse = await pinService.ViewPin(testViewPin);
    //     timer.Stop();

    //     // Assert
    //     Assert.True(viewPinResponse.HasError == false);
    //     Assert.True(timer.Elapsed.TotalSeconds <= 3);
    //     Assert.NotNull(viewPinResponse.Output);
    //     Assert.True(viewPinResponse.Output.Count == 1);

    //     // Cleanup
    //     var deleteDataOnlyDAO = new DeleteDataOnlyDAO();

    //     var deletePinSql = $"DELETE FROM MapPin WHERE PinId=\"{PINID}\";";

    //     await deleteDataOnlyDAO.DeleteData(deletePinSql);
    // }

    // [Fact]
    // public async Task PinServiceViewPinDetailsShould_ShouldThrowAnErrorIfFailToRetrieveDetailsFromDatabase()
    // {
    //     //Arrange

    //     var testCreatePin = new CreatePinRequest();
    //     testCreatePin.Principal = PRINCIPAL;
    //     testCreatePin.PinId = PINID;
    //     testCreatePin.LLIId = LLIID;
    //     testCreatePin.Address = ADDRESS;
    //     testCreatePin.Latitude = LATITUDE;
    //     testCreatePin.Longitude = LONGITUDE;

    //     var failPin = "wrongPin";
    //     var testViewPin = new ViewPinRequest();
    //     testViewPin.Principal = PRINCIPAL;
    //     testViewPin.PinId = failPin;
    //     testViewPin.LLIId = LLIID;
    //     testViewPin.Address = ADDRESS;
    //     testViewPin.Latitude = LATITUDE;
    //     testViewPin.Longitude = LONGITUDE;


    //     // Act
    //     var createPinResponse = await pinService.CreatePin(testCreatePin);
    //     var viewPinResponse = await pinService.ViewPin(testViewPin);

    //     // Assert
    //     Assert.True(viewPinResponse.HasError == true);
    //     Assert.Null(viewPinResponse.Output);
    //     Assert.True(viewPinResponse.Output!.Count == 0);

    //     // Cleanup
    //     var deleteDataOnlyDAO = new DeleteDataOnlyDAO();

    //     var deletePinSql = $"DELETE FROM MapPin WHERE PinId=\"{PINID}\";";

    //     await deleteDataOnlyDAO.DeleteData(deletePinSql);
    // }

    [Fact]
    public async Task PinServiceViewPinDetailsShould_ShouldThrowAnErrorIfTookLongerThan3Seconds()
    {
        //Arrange

        var testCreatePin = new CreatePinRequest();
        testCreatePin.Principal = PRINCIPAL;
        testCreatePin.PinId = PINID;
        testCreatePin.LLIId = LLIID;
        testCreatePin.Address = ADDRESS;
        testCreatePin.Latitude = LATITUDE;
        testCreatePin.Longitude = LONGITUDE;

        var testViewPin = new ViewPinRequest();
        testViewPin.Principal = PRINCIPAL;
        testViewPin.PinId = PINID;
        testViewPin.LLIId = LLIID;
        testViewPin.Address = ADDRESS;
        testViewPin.Latitude = LATITUDE;
        testViewPin.Longitude = LONGITUDE;


        // Act
        timer.Start();
        var createPinResponse = await pinService.CreatePin(testCreatePin);
        var viewPinResponse = await pinService.ViewPin(testViewPin);
        var additionalTime = TimeSpan.FromSeconds(5);
        var newElapsed = timer.Elapsed + additionalTime;
        timer.Restart();
        while (timer.Elapsed < newElapsed)
        {
        }
        timer.Stop();

        // Assert
        Assert.True(viewPinResponse.HasError == true);
        Assert.True(timer.Elapsed.TotalSeconds > 3);
        Assert.Null(viewPinResponse.Output);

        // Cleanup
        var deleteDataOnlyDAO = new DeleteDataOnlyDAO();

        var deletePinSql = $"DELETE FROM MapPin WHERE PinId=\"{PINID}\";";

        await deleteDataOnlyDAO.DeleteData(deletePinSql);
    }
    #endregion

    #region Update Pin Tests
    // [Fact]
    // public async Task PinServiceUpdatePinShould_UpdatePinInTheDatabase()
    // {
    //     //Arrange

    //     var testCreatePin = new CreatePinRequest();
    //     testCreatePin.Principal = PRINCIPAL;
    //     testCreatePin.PinId = PINID;
    //     testCreatePin.LLIId = LLIID;
    //     testCreatePin.Address = ADDRESS;
    //     testCreatePin.Latitude = LATITUDE;
    //     testCreatePin.Longitude = LONGITUDE;

    //     var newLat = 200.00002;
    //     var testUpdatePin = new UpdatePinRequest();
    //     testUpdatePin.Principal = PRINCIPAL;
    //     testUpdatePin.PinId = PINID;
    //     testUpdatePin.LLIId = LLIID;
    //     testUpdatePin.Address = ADDRESS;
    //     testUpdatePin.Latitude = newLat;
    //     testUpdatePin.Longitude = LONGITUDE;


    //     // Act
    //     timer.Start();
    //     var createPinResponse = await pinService.CreatePin(testCreatePin);
    //     var updatePinResponse = await pinService.UpdatePin(testUpdatePin);
    //     timer.Stop();
    //     var readDataOnlyDAO = new ReadDataOnlyDAO();
    //     var readPinSql = $"SELECT PinId, LLIId, Address, Latitude, Longitude " + $"FROM MapPin Where PinId=\"{PINID}\";"; //double check this
    //     var readResponse = await readDataOnlyDAO.ReadData(readPinSql);

    //     // Assert
    //     Assert.True(updatePinResponse.HasError == false);
    //     Assert.True(timer.Elapsed.TotalSeconds <= 3);
    //     Assert.NotNull(readResponse.Output);
    //     Assert.True(readResponse.Output.Count == 1);

    //     // Cleanup
    //     var deleteDataOnlyDAO = new DeleteDataOnlyDAO();

    //     var deletePinSql = $"DELETE FROM MapPin WHERE PinId=\"{PINID}\";";

    //     await deleteDataOnlyDAO.DeleteData(deletePinSql);
    // }
/*
    [Fact]
    public async Task PinServiceUpdatePinShould_ThrowAnErrorIfUnableToUpdatePin()
    {
        //Arrange

        var testCreatePin = new CreatePinRequest();
        testCreatePin.Principal = PRINCIPAL;
        testCreatePin.PinId = PINID;
        testCreatePin.LLIId = LLIID;
        testCreatePin.Address = ADDRESS;
        testCreatePin.Latitude = LATITUDE;
        testCreatePin.Longitude = LONGITUDE;

        var newLat = 200.000000000002;
        var testUpdatePin = new UpdatePinRequest();
        testUpdatePin.Principal = PRINCIPAL;
        testUpdatePin.PinId = PINID;
        testUpdatePin.LLIId = LLIID;
        testUpdatePin.Address = ADDRESS;
        testUpdatePin.Latitude = newLat;
        testUpdatePin.Longitude = LONGITUDE;


        // Act
        var createPinResponse = await pinService.CreatePin(testCreatePin);
        var updatePinResponse = await pinService.UpdatePin(testUpdatePin);

        // Assert
        Assert.True(updatePinResponse.HasError == true);
        Assert.Null(updatePinResponse.Output);
        Assert.True(updatePinResponse.Output!.Count == 0);

        // Cleanup
        var deleteDataOnlyDAO = new DeleteDataOnlyDAO();

        var deletePinSql = $"DELETE FROM MapPin WHERE PinId=\"{PINID}\";";

        await deleteDataOnlyDAO.DeleteData(deletePinSql);
    }*/
    #region Implement Here

    //Last failure outcome or do in the front end

    #endregion
    [Fact]
    public async Task PinServiceUpdatePinShould_ThrowAnErrorIfTimeTookLongerThan3Seconds()
    {
        //Arrange

        var testCreatePin = new CreatePinRequest();
        testCreatePin.Principal = PRINCIPAL;
        testCreatePin.PinId = PINID;
        testCreatePin.LLIId = LLIID;
        testCreatePin.Address = ADDRESS;
        testCreatePin.Latitude = LATITUDE;
        testCreatePin.Longitude = LONGITUDE;

        var newLat = 200.00002;
        var testUpdatePin = new UpdatePinRequest();
        testUpdatePin.Principal = PRINCIPAL;
        testUpdatePin.PinId = PINID;
        testUpdatePin.LLIId = LLIID;
        testUpdatePin.Address = ADDRESS;
        testUpdatePin.Latitude = newLat;
        testUpdatePin.Longitude = LONGITUDE;


        // Act
        timer.Start();
        var createPinResponse = await pinService.CreatePin(testCreatePin);
        var updatePinResponse = await pinService.UpdatePin(testUpdatePin);
        var additionalTime = TimeSpan.FromSeconds(5);
        var newElapsed = timer.Elapsed + additionalTime;
        timer.Restart();
        while (timer.Elapsed < newElapsed)
        {
        }
        timer.Stop();

        // Assert
        Assert.True(updatePinResponse.HasError == true);
        Assert.True(timer.Elapsed.TotalSeconds > 3);
        Assert.Null(updatePinResponse.Output);


        // Cleanup
        var deleteDataOnlyDAO = new DeleteDataOnlyDAO();

        var deletePinSql = $"DELETE FROM MapPin WHERE PinId=\"{PINID}\";";

        await deleteDataOnlyDAO.DeleteData(deletePinSql);
    }
    #endregion

}