namespace Peace.Lifelog.LLITest;

using System.Diagnostics;
using System.Threading.Tasks;
using Peace.Lifelog.DataAccess;
using Peace.Lifelog.LLI;
using Peace.Lifelog.Logging;
using Peace.Lifelog.UserManagement;
using Peace.Lifelog.UserManagementTest;
using Peace.Lifelog.Infrastructure;
using Peace.Lifelog.Security;
using Peace.Lifelog.Email;

public class LLIServiceShould : IAsyncLifetime, IDisposable
{
    private static CreateDataOnlyDAO createDataOnlyDAO = new CreateDataOnlyDAO();
    private static ReadDataOnlyDAO readDataOnlyDAO = new ReadDataOnlyDAO();
    private static UpdateDataOnlyDAO updateDataOnlyDAO = new UpdateDataOnlyDAO();
    private static DeleteDataOnlyDAO deleteDataOnlyDAO = new DeleteDataOnlyDAO();
    private static LogTarget logTarget = new LogTarget(createDataOnlyDAO, readDataOnlyDAO);
    private static Logging logging = new Logging(logTarget);
    private static LLIRepo lliRepo = new LLIRepo(createDataOnlyDAO, readDataOnlyDAO, updateDataOnlyDAO, deleteDataOnlyDAO);
    private LLIService LLIService = new LLIService(lliRepo, logging);
    private const string USER_ID = "TestLLIServiceAccount";
    private string USER_HASH = "";
    private const string ROLE = "Normal";

    private string DOB = DateTime.Today.ToString("yyyy-MM-dd");
    private string DEADLINE = DateTime.Today.ToString("yyyy-MM-dd");
    private const string ZIP_CODE = "90704";
    private Stopwatch timer = new Stopwatch();
    private const int MAX_TIME_IN_SECOND = 3;

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
        IEmailService emailService = new EmailService();
    
        IUserManagmentRepo userManagementRepo = new UserManagmentRepo(createDataOnlyDAO, readDataOnlyDAO, updateDataOnlyDAO, deleteDataOnlyDAO, logger);
        AppUserManagementService appUserManagementService =  new AppUserManagementService(createDataOnlyDAO, readDataOnlyDAO, updateDataOnlyDAO, deleteDataOnlyDAO,logger);
        
        var lifelogUserManagementService = new LifelogUserManagementService(userManagementRepo, appUserManagementService, saltService, emailService, hashService);
        

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
        ICreateDataOnlyDAO createDataOnlyDAO = new CreateDataOnlyDAO();
        IReadDataOnlyDAO readDataOnlyDAO = new ReadDataOnlyDAO();
        IUpdateDataOnlyDAO updateDataOnlyDAO = new UpdateDataOnlyDAO();
        IDeleteDataOnlyDAO deleteDataOnlyDAO = new DeleteDataOnlyDAO();
        ILogTarget logTarget = new LogTarget(createDataOnlyDAO, readDataOnlyDAO);
        ILogging logger = new Logging(logTarget);
        ISaltService saltService = new SaltService();
        IHashService hashService = new HashService();
        IEmailService emailService = new EmailService();
    
        IUserManagmentRepo userManagementRepo = new UserManagmentRepo(createDataOnlyDAO, readDataOnlyDAO, updateDataOnlyDAO, deleteDataOnlyDAO, logger);
        AppUserManagementService appUserManagementService =  new AppUserManagementService(createDataOnlyDAO, readDataOnlyDAO, updateDataOnlyDAO, deleteDataOnlyDAO,logger);
        
        var lifelogUserManagementService = new LifelogUserManagementService(userManagementRepo, appUserManagementService, saltService, emailService, hashService);
        var testLifelogAccountRequest = new LifelogAccountRequest();
        testLifelogAccountRequest.UserId = ("UserId", USER_ID);
        var deleteAccountResponse = appUserManagementService.DeleteAccount(testLifelogAccountRequest);
    }
    
    #region Create LLI Tests
    [Fact]
    public async void LLIServiceCreateLLIShould_CreateAnLLIInTheDatabase()
    {
        // Arrange
        string testLLITitle = "Test Create LLI Title";

        

        var testLLI = new LLI();
        testLLI.UserHash = USER_HASH;
        testLLI.Title = testLLITitle;
        testLLI.Description = "Test Create LLI Description";
        testLLI.Category1 = LLICategory.Travel;
        testLLI.Category2 = LLICategory.Outdoor;
        testLLI.Category3 = LLICategory.Volunteering;
        testLLI.Status = LLIStatus.Active;
        testLLI.Visibility = LLIVisibility.Public;
        testLLI.Deadline = DEADLINE;
        testLLI.Cost = 0;
        
        var LLIRecurrence = new LLIRecurrence();
        LLIRecurrence.Status = LLIRecurrenceStatus.On;
        LLIRecurrence.Frequency = LLIRecurrenceFrequency.Weekly;

        testLLI.Recurrence = LLIRecurrence;

        // Act
        timer.Start();
        var createLLIResponse = await LLIService.CreateLLI(USER_HASH, testLLI);
        timer.Stop();

        var readDataOnlyDAO = new ReadDataOnlyDAO();
        var readLLISql = $"SELECT LLIId FROM LLI WHERE Title=\"{testLLITitle}\"";
        var readResponse = await readDataOnlyDAO.ReadData(readLLISql);

        // Assert
        Assert.True(createLLIResponse.HasError == false);
        Assert.NotNull(readResponse.Output);
        Assert.True(readResponse.Output.Count == 1);
        Assert.True(timer.Elapsed.TotalSeconds < MAX_TIME_IN_SECOND);
        
        
        // Cleanup
        var deleteDataOnlyDAO = new DeleteDataOnlyDAO();

        string? LLIId = "";


        // The read sql return a list of LLI with a list of attribute within that LLI 
        foreach (List<Object> LLI in readResponse.Output)
        {
            foreach (var attribute in LLI) 
            {
                LLIId = attribute.ToString(); // There is only one attribute being return, which is the LLIId
            }
            
        }
        
        
        var deleteLLISql = $"DELETE FROM LLI WHERE LLIId=\"{LLIId}\";";

        await deleteDataOnlyDAO.DeleteData(deleteLLISql);

    }

    [Fact]
    public async void LLIServiceCreateLLIShould_ThrowAnErrorIfTheUserHashIsEmpty()
    {
        // Arrange
        string testLLITitle = "Test LLI Title";
        string invalidUserHash = "";

        

        var testLLI = new LLI();
        testLLI.UserHash = invalidUserHash;
        testLLI.Title = testLLITitle;
        testLLI.Description = "Test Create LLI Description";
        testLLI.Category1 = LLICategory.Travel;
        testLLI.Status = LLIStatus.Active;
        testLLI.Visibility = LLIVisibility.Public;
        testLLI.Deadline = DEADLINE;
        testLLI.Cost = 0;
        
        var LLIRecurrence = new LLIRecurrence();
        LLIRecurrence.Status = LLIRecurrenceStatus.On;
        LLIRecurrence.Frequency = LLIRecurrenceFrequency.Weekly;

        testLLI.Recurrence = LLIRecurrence;

        // Act
        var createLLIResponse = await LLIService.CreateLLI(invalidUserHash, testLLI);

        var readDataOnlyDAO = new ReadDataOnlyDAO();
        var readLLISql = $"SELECT LLIId FROM LLI WHERE Title=\"{testLLITitle}\"";
        var readResponse = await readDataOnlyDAO.ReadData(readLLISql);

        // Assert
        Assert.True(createLLIResponse.HasError == true);
        Assert.True(createLLIResponse.ErrorMessage == "User Hash must not be empty");
        Assert.Null(readResponse.Output);
    }

    [Fact]
    public async void LLIServiceCreateLLIShould_ThrowAnErrorIfANonNullableFieldIsNull()
    {
        // Arrange
        string? testLLITitle = null;
        

        

        var testLLI = new LLI();
        testLLI.Title = testLLITitle!;

        // Act
        var createLLIResponse = await LLIService.CreateLLI(USER_HASH, testLLI);

        // Assert
        Assert.True(createLLIResponse.HasError == true);
        Assert.True(createLLIResponse.ErrorMessage == "The non-nullable LLI input is null");
    }

    [Fact]
    public async void LLIServiceCreateLLIShould_ThrowAnErrorIfTheTitleIsTooLong()
    {
        // Arrange
        string testLLITitle = "Test Create LLI Title";

        

        var testLLI = new LLI();
        testLLI.UserHash = USER_HASH;
        testLLI.Title = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.";
        testLLI.Category1 = LLICategory.Travel;
        testLLI.Description = "Test Create LLI Description";
        testLLI.Status = LLIStatus.Active;
        testLLI.Visibility = LLIVisibility.Public;
        testLLI.Deadline = DEADLINE;
        testLLI.Cost = 0;
        
        var LLIRecurrence = new LLIRecurrence();
        LLIRecurrence.Status = LLIRecurrenceStatus.On;
        LLIRecurrence.Frequency = LLIRecurrenceFrequency.Weekly;

        testLLI.Recurrence = LLIRecurrence;

        // Act
        var createLLIResponse = await LLIService.CreateLLI(USER_HASH, testLLI);

        var readDataOnlyDAO = new ReadDataOnlyDAO();
        var readLLISql = $"SELECT LLIId FROM LLI WHERE Title=\"{testLLITitle}\"";
        var readResponse = await readDataOnlyDAO.ReadData(readLLISql);

        // Assert
        Assert.True(createLLIResponse.HasError == true);
        Assert.True(createLLIResponse.ErrorMessage == "The LLI title is invalid");
        Assert.Null(readResponse.Output);
    }

    [Fact]
    public async void LLIServiceCreateLLIShould_ThrowAnErrorIfTheCategoriesListIsEmpty()
    {
        // Arrange
        string testLLITitle = "TestLLITitle";

        

        var testLLI = new LLI();
        testLLI.Title = testLLITitle;
        testLLI.Category1 = null!;

        // Act
        var createLLIResponse = await LLIService.CreateLLI(USER_HASH, testLLI);

        // Assert
        Assert.True(createLLIResponse.HasError == true);
        Assert.True(createLLIResponse.ErrorMessage == "The non-nullable LLI input is null");
    }

    [Fact]
    public async void LLIServiceCreateLLIShould_ThrowAnErrorIfACategoryIsInvalid()
    {
        // Arrange
        string testLLITitle = "TestLLITitle";
        string testInvalidCategory = "LLI categories must not be null or empty";

        var testLLI = new LLI();
        testLLI.Title = testLLITitle;
        testLLI.Category1 = testInvalidCategory;

        // Act
        var createLLIResponse = await LLIService.CreateLLI(USER_HASH, testLLI);

        // Assert
        Assert.True(createLLIResponse.HasError == true);
        Assert.True(createLLIResponse.ErrorMessage == "LLI categories must not be null or empty");
    }

    [Fact]
    public async void LLIServiceCreateLLIShould_ThrowAnErrorIfDescriptionIsTooLong()
    {
        // Arrange
        string testLLITitle = "Test Create LLI Title";

        

        var testLLI = new LLI();
        testLLI.UserHash = "Test Invalid User Hash";
        testLLI.Title = "Test Create LLI Title";
        testLLI.Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.";
        testLLI.Category1 = LLICategory.Travel;
        testLLI.Status = LLIStatus.Active;
        testLLI.Visibility = LLIVisibility.Public;
        testLLI.Deadline = DEADLINE;
        testLLI.Cost = 0;
        
        var LLIRecurrence = new LLIRecurrence();
        LLIRecurrence.Status = LLIRecurrenceStatus.On;
        LLIRecurrence.Frequency = LLIRecurrenceFrequency.Weekly;

        testLLI.Recurrence = LLIRecurrence;

        // Act
        var createLLIResponse = await LLIService.CreateLLI(USER_HASH, testLLI);

        var readDataOnlyDAO = new ReadDataOnlyDAO();
        var readLLISql = $"SELECT LLIId FROM LLI WHERE Title=\"{testLLITitle}\"";
        var readResponse = await readDataOnlyDAO.ReadData(readLLISql);

        // Assert
        Assert.True(createLLIResponse.HasError == true);
        Assert.True(createLLIResponse.ErrorMessage == "LLI Description is too long");
        Assert.Null(readResponse.Output);
    }

    [Fact]
    public async void LLIServiceCreateLLIShould_ThrowAnErrorIfTheStatusIsInvalid()
    {
        // Arrange
        string testLLITitle = "TestLLITitle";
        string testInvalidStatus = "Invalid Status";

        

        var testLLI = new LLI();
        testLLI.Title = testLLITitle;
        testLLI.Category1 = LLICategory.Travel;
        testLLI.Description = "";
        testLLI.Status = testInvalidStatus;

        // Act
        var createLLIResponse = await LLIService.CreateLLI(USER_HASH, testLLI);

        // Assert
        Assert.True(createLLIResponse.HasError == true);
        Assert.True(createLLIResponse.ErrorMessage == "LLI status is invalid");
    }

    [Fact]
    public async void LLIServiceCreateLLIShould_ThrowAnErrorIfTheVisibilityIsInvalid()
    {
        // Arrange
        string testLLITitle = "TestLLITitle";
        string testInvalidVisibility = "Invalid Visibility";

        

        var testLLI = new LLI();
        testLLI.Title = testLLITitle;
        testLLI.Category1 = LLICategory.Travel;
        testLLI.Description = "";
        testLLI.Status = LLIStatus.Completed;
        testLLI.Visibility = testInvalidVisibility;

        // Act
        var createLLIResponse = await LLIService.CreateLLI(USER_HASH, testLLI);

        // Assert
        Assert.True(createLLIResponse.HasError == true);
        Assert.True(createLLIResponse.ErrorMessage == "LLI visibility is invalid");
    }

    [Fact]
    public async void LLIServiceCreateLLIShould_ThrowAnErrorIfDeadlineIsOutOfRange()
    {
        // Arrange
        string testLLITitle = "Test LLI Title";

        var testLLI = new LLI();
        testLLI.UserHash = "Test Invalid User Hash";
        testLLI.Title = "Test LLI Title";
        testLLI.Description = "Test LLI Description";
        testLLI.Category1 = LLICategory.Travel;
        testLLI.Status = LLIStatus.Active;
        testLLI.Visibility = LLIVisibility.Public;
        testLLI.Deadline = "1800-11-11";
        testLLI.Cost = 0;

        var LLIRecurrence = new LLIRecurrence();
        LLIRecurrence.Status = LLIRecurrenceStatus.On;
        LLIRecurrence.Frequency = LLIRecurrenceFrequency.Weekly;

        testLLI.Recurrence = LLIRecurrence;

        // Act
        var createLLIResponse = await LLIService.CreateLLI(USER_HASH, testLLI);

        var readDataOnlyDAO = new ReadDataOnlyDAO();
        var readLLISql = $"SELECT LLIId FROM LLI WHERE Title=\"{testLLITitle}\"";
        var readResponse = await readDataOnlyDAO.ReadData(readLLISql);

        //Assert
        Assert.True(createLLIResponse.HasError == true);
        Assert.True(createLLIResponse.ErrorMessage == "LLI Deadline is out of range");
        Assert.Null(readResponse.Output);
    }

    [Fact]
    public async void LLIServiceCreateLLIShould_ThrowAnErrorIfTheRecurrenceStatusIsInvalid()
    {
        // Arrange
        string testLLITitle = "TestLLITitle";
        string testInvalidRecurrenceStatus = "Invalid Recurrence Status";

        

        var testLLI = new LLI();
        testLLI.Title = testLLITitle;
        testLLI.Category1 = LLICategory.Travel;
        testLLI.Description = "";
        testLLI.Status = LLIStatus.Completed;
        testLLI.Visibility = LLIVisibility.Public;
        testLLI.Recurrence.Status = testInvalidRecurrenceStatus;

        // Act
        var createLLIResponse = await LLIService.CreateLLI(USER_HASH, testLLI);

        // Assert
        Assert.True(createLLIResponse.HasError == true);
        Assert.True(createLLIResponse.ErrorMessage == "LLI recurrence status is invalid");
    }

    [Fact]
    public async void LLIServiceCreateLLIShould_ThrowAnErrorIfTheRecurrenceFrequencyIsInvalid()
    {
        // Arrange
        string testLLITitle = "TestLLITitle";
        string testInvalidRecurrenceFrequency = "Invalid Recurrence Frequency";

        

        var testLLI = new LLI();
        testLLI.Title = testLLITitle;
        testLLI.Category1 = LLICategory.Travel;
        testLLI.Description = "";
        testLLI.Status = LLIStatus.Completed;
        testLLI.Visibility = LLIVisibility.Public;
        testLLI.Recurrence.Status = LLIRecurrenceStatus.On;
        testLLI.Recurrence.Frequency = testInvalidRecurrenceFrequency;

        // Act
        var createLLIResponse = await LLIService.CreateLLI(USER_HASH, testLLI);

        // Assert
        Assert.True(createLLIResponse.HasError == true);
        Assert.True(createLLIResponse.ErrorMessage == "LLI recurrence frequency is invalid");
    }

    [Fact]
    public async void LLIServiceCreateLLIShould_ThrowAnErrorIfCostIsLessThanZero()
    {
        // Arrange
        string testLLITitle = "Test LLI Title";

        

        var testLLI = new LLI();
        testLLI.UserHash = "Test Invalid User Hash";
        testLLI.Title = "Test LLI Title";
        testLLI.Description = "Test LLI Description";
        testLLI.Category1 = LLICategory.Travel;
        testLLI.Status = LLIStatus.Active;
        testLLI.Visibility = LLIVisibility.Public;
        testLLI.Deadline = "2011-11-11";
        testLLI.Cost = -1;

        var LLIRecurrence = new LLIRecurrence();
        LLIRecurrence.Status = LLIRecurrenceStatus.On;
        LLIRecurrence.Frequency = LLIRecurrenceFrequency.Weekly;

        testLLI.Recurrence = LLIRecurrence;

        // Act
        var createLLIResponse = await LLIService.CreateLLI(USER_HASH, testLLI);

        var readDataOnlyDAO = new ReadDataOnlyDAO();
        var readLLISql = $"SELECT LLIId FROM LLI WHERE Title=\"{testLLITitle}\"";
        var readResponse = await readDataOnlyDAO.ReadData(readLLISql);

        //Assert
        Assert.True(createLLIResponse.HasError == true);
        Assert.True(createLLIResponse.ErrorMessage == "LLI Cost must not be negative");
        Assert.Null(readResponse.Output);
    }

    [Fact]
    public async void LLIServiceCreateLLIShould_NotCreateAnLLIIfTheUserDoesNotExist()
    {
        // Arrange
        string testLLITitle = "Test LLI Create With Empty User Hash";

        

        var testLLI = new LLI();
        testLLI.UserHash = "Test Invalid User Hash";
        testLLI.Title = "Test LLI Title";
        testLLI.Description = "Test LLI Description";
        testLLI.Category1 = LLICategory.Travel;
        testLLI.Status = LLIStatus.Active;
        testLLI.Visibility = LLIVisibility.Public;
        testLLI.Deadline = "2011-11-11";
        testLLI.Cost = 0;

        var LLIRecurrence = new LLIRecurrence();
        LLIRecurrence.Status = LLIRecurrenceStatus.On;
        LLIRecurrence.Frequency = LLIRecurrenceFrequency.Weekly;

        testLLI.Recurrence = LLIRecurrence;

        // Act
        var createLLIResponse = await LLIService.CreateLLI("Test Invalid User Hash", testLLI);

        var readDataOnlyDAO = new ReadDataOnlyDAO();
        var readLLISql = $"SELECT LLIId FROM LLI WHERE Title=\"{testLLITitle}\"";
        var readResponse = await readDataOnlyDAO.ReadData(readLLISql);

        //Assert
        Assert.True(createLLIResponse.HasError);
        Assert.Null(readResponse.Output);
    }

    [Fact]
    public async void LLIServiceCreateLLIShould_ThrowAnErrorIfAPropertyFieldViolateDBSchema()
    {
        // Arrange
        string testLLITitle = "Test LLI Create With Empty User Hash";

        

        var testLLI = new LLI();
        testLLI.UserHash = USER_HASH;
        testLLI.Title = "Test LLI Title";
        testLLI.Description = "Test LLI Description";
        testLLI.Category1 = LLICategory.Travel;
        testLLI.Status = LLIStatus.Active;
        testLLI.Visibility = LLIVisibility.Public;
        testLLI.Deadline = "2011-11"; // Invalid date format
        testLLI.Cost = 0;

        var LLIRecurrence = new LLIRecurrence();
        LLIRecurrence.Status = LLIRecurrenceStatus.On;
        LLIRecurrence.Frequency = LLIRecurrenceFrequency.Weekly;

        testLLI.Recurrence = LLIRecurrence;

        // Act
        var createLLIResponse = await LLIService.CreateLLI(USER_HASH, testLLI);

        var readDataOnlyDAO = new ReadDataOnlyDAO();
        var readLLISql = $"SELECT LLIId FROM LLI WHERE Title=\"{testLLITitle}\"";
        var readResponse = await readDataOnlyDAO.ReadData(readLLISql);

        //Assert
        Assert.True(createLLIResponse.HasError);
        Assert.True(createLLIResponse.ErrorMessage == "LLI fields are invalid");
        Assert.Null(readResponse.Output);
    }

    [Fact]
    public async void LLIServiceCreateLLIShould_ThrowAnErrorIfALLIWithTheSameTitleHasBeenCreatedInThePastYear()
    {
        // Arrange
        var testLLITitle = "Test Create LLI Within A Year";
        

        var testLLI = new LLI();
        testLLI.UserHash = USER_HASH;
        testLLI.Title = testLLITitle;
        testLLI.Description = "Test LLI Description";
        testLLI.Category1 = LLICategory.Travel;
        testLLI.Status = LLIStatus.Active;
        testLLI.Visibility = LLIVisibility.Public;
        testLLI.Deadline = "2011-11-11"; // Invalid date format
        testLLI.Cost = 0;

        var LLIRecurrence = new LLIRecurrence();
        LLIRecurrence.Status = LLIRecurrenceStatus.On;
        LLIRecurrence.Frequency = LLIRecurrenceFrequency.Weekly;

        testLLI.Recurrence = LLIRecurrence;

        // Act
        // Create LLI
        var _ = await LLIService.CreateLLI(USER_HASH, testLLI);

        // Update LLI to be completed
        var readDataOnlyDAO = new ReadDataOnlyDAO();
        var readLLISql = $"SELECT LLIId FROM LLI WHERE Title=\"{testLLITitle}\"";
        var readResponse = await readDataOnlyDAO.ReadData(readLLISql);

        string? LLIId = "";

        if (readResponse.Output != null)
        {
            // The read sql return a list of LLI with a list of attribute within that LLI 
            foreach (List<Object> LLI in readResponse.Output)
            {
                foreach (var attribute in LLI) 
                {
                    LLIId = attribute.ToString(); // There is only one attribute being return, which is the LLIId
                }
                
            }
        }

        if (LLIId == null)
        {
            Assert.Fail("LLIId failed to be created");
        }

        var updateLLICompletionDate = new LLI();
        updateLLICompletionDate.LLIID = LLIId;
        updateLLICompletionDate.CompletionDate = DateTime.Today.ToString("yyyy-MM-dd");

        var __ = await LLIService.UpdateLLI(USER_HASH, updateLLICompletionDate);

        // Create Second LLI After First LLI
        var createSecondLLIResponse = await LLIService.CreateLLI(USER_HASH, testLLI);

        

        //Assert
        Assert.True(createSecondLLIResponse.HasError);
        Assert.True(createSecondLLIResponse.ErrorMessage == "LLI has been completed within the last year");

        // Cleanup
        

        var deleteDataOnlyDAO = new DeleteDataOnlyDAO();

        
        
        var deleteLLISql = $"DELETE FROM LLI WHERE LLIId=\"{LLIId}\";";

        await deleteDataOnlyDAO.DeleteData(deleteLLISql);   
    }
    #endregion

    #region Read LLI Tests
    [Fact]
    public async void LLIServiceGetAllLLIFromUserShould_GetAllLLIForAUser()
    {
        // Arrange
        string testLLITitle = "Test Get LLI Title";

        // Create 10 LLI
        int numberOfLLI = 10;

        for (int i = 1; i <= numberOfLLI; i++)
        {
            var testLLI = new LLI();
            testLLI.UserHash = USER_HASH;
            testLLI.Title = testLLITitle;
            testLLI.Description = $"Test Get LLI number {i}";
            testLLI.Category1 = LLICategory.Travel;
            testLLI.Category2 = LLICategory.Hobby;
            testLLI.Status = LLIStatus.Active;
            testLLI.Visibility = LLIVisibility.Public;
            testLLI.Deadline = DEADLINE;
            testLLI.Cost = 0;
            
            var LLIRecurrence = new LLIRecurrence();
            LLIRecurrence.Status = LLIRecurrenceStatus.On;
            LLIRecurrence.Frequency = LLIRecurrenceFrequency.Weekly;

            testLLI.Recurrence = LLIRecurrence;
            var createLLIResponse = await LLIService.CreateLLI(USER_HASH, testLLI);
        }
        
        // Act
        timer.Start();
        var readResponse = await LLIService.GetAllLLIFromUser(USER_HASH);
        timer.Stop();

        // Assert
        Assert.True(readResponse.HasError == false);
        Assert.True(readResponse.Output != null);
        Assert.True(readResponse.Output.Count == numberOfLLI);
        Assert.True(timer.Elapsed.TotalSeconds < MAX_TIME_IN_SECOND);

        // Cleanup
        var deleteDataOnlyDAO = new DeleteDataOnlyDAO();
        
        var deleteLLISql = $"DELETE FROM LLI WHERE Title=\"{testLLITitle}\";";

        await deleteDataOnlyDAO.DeleteData(deleteLLISql);
    }

    [Fact]
    public async void LLIServiceGetAllLLIFromUserShould_ThrowAnErrorIfUserHashIsEmpty()
    {
        // Arrange
        
        // Act
        var readResponse = await LLIService.GetAllLLIFromUser("");

        // Assert
        Assert.True(readResponse.HasError == true);
        Assert.True(readResponse.ErrorMessage == "UserHash can not be empty");
        Assert.True(readResponse.Output == null);
    }

    [Fact]
    public async void LLIServiceGetAllLLIFromUserShould_ReturnNullIfUserDoesntExist()
    {
        // Arrange
        
        // Act
        var readResponse = await LLIService.GetAllLLIFromUser("NonExistentUser");

        // Assert
        Assert.True(readResponse.Output == null);
    }
    #endregion

    #region Update LLI Tests
    [Fact]
    public async void LLIServiceUpdateLLIShould_UpdateALLIInTheDatabase()
    {
        // Arrange
        string testOldLLITitle = "Test LLI Title";
        string testNewLLITitle = "New LLI Title";
        string testOldLLIDescription = "Test Update LLI";
        string testNewLLIDescription = "Test New Update LLI"; 
        int testOldLLICost = 0;
        int testNewLLICost = 1;

        // Old LLI        
        var testOldLLI = new LLI();
        testOldLLI.UserHash = USER_HASH;
        testOldLLI.Title = testOldLLITitle;
        testOldLLI.Description = testOldLLIDescription;
        testOldLLI.Category1 = LLICategory.Travel;
        testOldLLI.Category2 = LLICategory.Outdoor;
        testOldLLI.Status = LLIStatus.Active;
        testOldLLI.Visibility = LLIVisibility.Public;
        testOldLLI.Deadline = DEADLINE;
        testOldLLI.Cost = testOldLLICost;
        
        var LLIRecurrence = new LLIRecurrence();
        LLIRecurrence.Status = LLIRecurrenceStatus.On;
        LLIRecurrence.Frequency = LLIRecurrenceFrequency.Weekly;

        testOldLLI.Recurrence = LLIRecurrence;
        var createLLIResponse = await LLIService.CreateLLI(USER_HASH, testOldLLI);

        string id = string.Empty;
        var readResponse = await LLIService.GetAllLLIFromUser(USER_HASH);
        if (readResponse.Output != null) {
            foreach (LLI lli in readResponse.Output) {
                id = lli.LLIID;
            }
        }

        // New LLI
        var testNewLLI = new LLI();
        testNewLLI.LLIID = id;
        testNewLLI.Title = testNewLLITitle;
        testNewLLI.Description = testNewLLIDescription;
        testNewLLI.Category1 = LLICategory.Volunteering;
        testNewLLI.Category2 = LLICategory.Sport;
        testNewLLI.Status = LLIStatus.Completed;
        testNewLLI.Visibility = LLIVisibility.Private;
        testNewLLI.Cost = testNewLLICost;
        
        // Act
        timer.Start();
        var updateResponse = await LLIService.UpdateLLI(USER_HASH, testNewLLI);
        timer.Stop();

        readResponse = await LLIService.GetAllLLIFromUser(USER_HASH);

        // Assert
        Assert.True(readResponse.Output != null);
        foreach (LLI lli in readResponse.Output)
        {
            Assert.True(lli.Title == testNewLLITitle);
            Assert.True(lli.Description == testNewLLIDescription);
            
            // Check if the two new categories match 
            Assert.True(testNewLLI.Category1 == lli.Category1); 
            Assert.True(testNewLLI.Category2 == lli.Category2);

            Assert.True(lli.Status == testNewLLI.Status);
            Assert.True(lli.Visibility == testNewLLI.Visibility);
            Assert.True(lli.Cost == testNewLLICost);
        }
        Assert.True(timer.Elapsed.TotalSeconds < MAX_TIME_IN_SECOND);
        
        // Cleanup
        var deleteDataOnlyDAO = new DeleteDataOnlyDAO();
        
        var deleteLLISql = $"DELETE FROM LLI WHERE Title=\"{testOldLLITitle}\";";

        await deleteDataOnlyDAO.DeleteData(deleteLLISql);

        deleteLLISql = $"DELETE FROM LLI WHERE Title=\"{testNewLLITitle}\";";

        await deleteDataOnlyDAO.DeleteData(deleteLLISql);
    }

    [Fact]
    public async void LLIServiceUpdateLLIShould_ThrowAnErrorIfUserIsEmpty()
    {
        // Arrange

        // New LLI
        var testNewLLI = new LLI();
        
        // Act
        var updateResponse = await LLIService.UpdateLLI("", testNewLLI);

        // Assert
        Assert.True(updateResponse.HasError == true);
        Assert.True(updateResponse.ErrorMessage == "User Hash must not be empty");
        Assert.Null(updateResponse.Output);
    }

    [Fact]
    public async void LLIServiceUpdateLLIShould_ThrowAnErrorIfTitleIsTooLong()
    {
        // Arrange

        // New LLI
        var testNewLLI = new LLI();
        testNewLLI.Title = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.";
        
        // Act
        var updateResponse = await LLIService.UpdateLLI("UserHash", testNewLLI);

        // Assert
        Assert.True(updateResponse.HasError == true);
        Assert.True(updateResponse.ErrorMessage == "The LLI title is invalid");
        Assert.Null(updateResponse.Output);
    }

    [Fact]
    public async void LLIServiceUpdateLLIShould_ThrowAnErrorIfACategoryIsInvalid()
    {
        // Arrange

        // New LLI
        var testNewLLI = new LLI();
        testNewLLI.Category1 = "Invalid Category";
        
        // Act
        var updateResponse = await LLIService.UpdateLLI("UserHash", testNewLLI);

        // Assert
        Assert.True(updateResponse.HasError == true);
        Assert.True(updateResponse.ErrorMessage == "LLI categories must not be null or empty");
        Assert.Null(updateResponse.Output);
    }

    [Fact]
    public async void LLIServiceUpdateLLIShould_ThrowAnErrorIfDescriptionIsTooLong()
    {
        // Arrange

        // New LLI
        var testNewLLI = new LLI();
        testNewLLI.Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.";
        
        // Act
        var updateResponse = await LLIService.UpdateLLI("UserHash", testNewLLI);

        // Assert
        Assert.True(updateResponse.HasError == true);
        Assert.True(updateResponse.ErrorMessage == "LLI Description is too long");
        Assert.Null(updateResponse.Output);
    }

    [Fact]
    public async void LLIServiceUpdateLLIShould_ThrowAnErrorIfTheLLIStatusIsInvalid()
    {
        // Arrange

        // New LLI
        var testNewLLI = new LLI();
        testNewLLI.Status = "Invalid Status";
        
        // Act
        var updateResponse = await LLIService.UpdateLLI("UserHash", testNewLLI);

        // Assert
        Assert.True(updateResponse.HasError == true);
        Assert.True(updateResponse.ErrorMessage == "LLI status is invalid");
        Assert.Null(updateResponse.Output);
    }

    [Fact]
    public async void LLIServiceUpdateLLIShould_ThrowAnErrorIfTheLLIVisibilityIsInvalid()
    {
        // Arrange

        // New LLI
        var testNewLLI = new LLI();
        testNewLLI.Visibility = "Invalid Visibility";
        
        // Act
        var updateResponse = await LLIService.UpdateLLI("UserHash", testNewLLI);

        // Assert
        Assert.True(updateResponse.HasError == true);
        Assert.True(updateResponse.ErrorMessage == "LLI visibility is invalid");
        Assert.Null(updateResponse.Output);
    }

    [Fact]
    public async void LLIServiceUpdateLLIShould_ThrowAnErrorIfDeadlineIsOutOfRange()
    {
        // Arrange

        // New LLI
        var testNewLLI = new LLI();
        testNewLLI.Deadline = "1800-11-11";
        
        // Act
        var updateResponse = await LLIService.UpdateLLI("UserHash", testNewLLI);

        // Assert
        Assert.True(updateResponse.HasError == true);
        Assert.True(updateResponse.ErrorMessage == "LLI Deadline is out of range");
        Assert.Null(updateResponse.Output);
    }

    [Fact]
    public async void LLIServiceUpdateLLIShould_ThrowAnErrorIfCostIsLessThanZero()
    {
        // Arrange

        // New LLI
        var testNewLLI = new LLI();
        testNewLLI.Cost = -1;
        
        // Act
        var updateResponse = await LLIService.UpdateLLI("UserHash", testNewLLI);

        // Assert
        Assert.True(updateResponse.HasError == true);
        Assert.True(updateResponse.ErrorMessage == "LLI Cost must not be negative");
        Assert.Null(updateResponse.Output);
    }

    [Fact]
    public async void LLIServiceUpdateLLIShould_ThrowAnErrorIfTheLLIRecurrenceStatusIsInvalid()
    {
        // Arrange

        // New LLI
        var testNewLLI = new LLI();
        testNewLLI.Recurrence.Status = "Invalid Recurrenc Status";
        
        // Act
        var updateResponse = await LLIService.UpdateLLI("UserHash", testNewLLI);

        // Assert
        Assert.True(updateResponse.HasError == true);
        Assert.True(updateResponse.ErrorMessage == "LLI recurrence status is invalid");
        Assert.Null(updateResponse.Output);
    }

    [Fact]
    public async void LLIServiceUpdateLLIShould_ThrowAnErrorIfTheLLIRecurrenceFrequencyIsInvalid()
    {
        // Arrange

        // New LLI
        var testNewLLI = new LLI();
        testNewLLI.Recurrence.Status = LLIRecurrenceStatus.On;
        testNewLLI.Recurrence.Frequency = "Invalid Recurrence Frequency";
        
        // Act
        var updateResponse = await LLIService.UpdateLLI("UserHash", testNewLLI);

        // Assert
        Assert.True(updateResponse.HasError == true);
        Assert.True(updateResponse.ErrorMessage == "LLI recurrence frequency is invalid");
        Assert.Null(updateResponse.Output);
    }

    [Fact]
    public async void LLIServiceUpdateLLIShould_ReturnNoRowsAffectedlIfTheUserDoesNotExist()
    {
        // Arrange

        // New LLI
        var testNewLLI = new LLI();
        testNewLLI.Cost = 1;
        
        // Act
        var updateResponse = await LLIService.UpdateLLI("UserHash", testNewLLI);

        // Assert
        Assert.True(updateResponse.Output is not null);
        foreach (int rowsAffected in updateResponse.Output)
        {
            Assert.True(rowsAffected == 0);
        }
    }

    [Fact]
    public async void LLIServiceUpdateLLIShould_ThrowAnErrorIfLLIViolatesDatabaseSchema()
    {
        // Arrange
        string testOldLLITitle = "Test LLI Title";
        string testOldLLIDescription = "Test Update LLI";
        int testOldLLICost = 0;

        // Old LLI        
        var testOldLLI = new LLI();
        testOldLLI.UserHash = USER_HASH;
        testOldLLI.Title = testOldLLITitle;
        testOldLLI.Description = testOldLLIDescription;
        testOldLLI.Category1 = LLICategory.Travel;
        testOldLLI.Status = LLIStatus.Active;
        testOldLLI.Visibility = LLIVisibility.Public;
        testOldLLI.Deadline = DEADLINE;
        testOldLLI.Cost = testOldLLICost;
        
        var LLIRecurrence = new LLIRecurrence();
        LLIRecurrence.Status = LLIRecurrenceStatus.On;
        LLIRecurrence.Frequency = LLIRecurrenceFrequency.Weekly;

        testOldLLI.Recurrence = LLIRecurrence;
        var createLLIResponse = await LLIService.CreateLLI(USER_HASH, testOldLLI);

        string id = string.Empty;
        var readResponse = await LLIService.GetAllLLIFromUser(USER_HASH);
        if (readResponse.Output != null) {
            foreach (LLI lli in readResponse.Output) {
                id = lli.LLIID;
            }
        }

        // New LLI
        var testNewLLI = new LLI();
        testNewLLI.LLIID = id;
        testNewLLI.Deadline = "2000-11";
        
        // Act
        var updateResponse = await LLIService.UpdateLLI(USER_HASH, testNewLLI);

        // Assert
        Assert.True(updateResponse.HasError);
        Assert.True(updateResponse.ErrorMessage == "LLI fields are invalid");
        Assert.Null(updateResponse.Output);
        
    }

    [Fact]
    public async void LLIServiceUpdateLLIShould_ThrowAnErrorIfTheAnLLIWithTheSameNameHasBeenCompletedInTheLastYear()
    {
        // Arrange
        string testOldLLITitle = "Test LLI Title";
        string testOldLLIDescription = "Test Update LLI";
        int testOldLLICost = 0;

        // Old LLI        
        var testOldLLI = new LLI();
        testOldLLI.UserHash = USER_HASH;
        testOldLLI.Title = testOldLLITitle;
        testOldLLI.Description = testOldLLIDescription;
        testOldLLI.Category1 = LLICategory.Travel;
        testOldLLI.Status = LLIStatus.Completed;
        testOldLLI.Visibility = LLIVisibility.Public;
        testOldLLI.Deadline = DEADLINE;
        testOldLLI.Cost = testOldLLICost;
        
        var LLIRecurrence = new LLIRecurrence();
        LLIRecurrence.Status = LLIRecurrenceStatus.On;
        LLIRecurrence.Frequency = LLIRecurrenceFrequency.Weekly;

        testOldLLI.Recurrence = LLIRecurrence;
        var createLLIResponse = await LLIService.CreateLLI(USER_HASH, testOldLLI);

        // Act
        timer.Start();
        var updateResponse = await LLIService.UpdateLLI(USER_HASH, testOldLLI);
        timer.Stop();

        // Assert
        Assert.True(updateResponse.HasError == true);
        
        // Cleanup
        var deleteDataOnlyDAO = new DeleteDataOnlyDAO();
        
        var deleteLLISql = $"DELETE FROM LLI WHERE Title=\"{testOldLLITitle}\";";

        await deleteDataOnlyDAO.DeleteData(deleteLLISql);
    }
    #endregion

    [Fact]
    public async void LLIServiceDeleteLLIShould_DeleteALLInTheDatabase()
    {
        // Arrange
        string testLLITitle = "Test LLI Title";  
        
        var testLLI = new LLI();
        testLLI.UserHash = USER_HASH;
        testLLI.Title = testLLITitle;
        testLLI.Description = $"Test Delete LLI";
        testLLI.Category1 = LLICategory.Travel;
        testLLI.Status = LLIStatus.Active;
        testLLI.Visibility = LLIVisibility.Public;
        testLLI.Deadline = DEADLINE;
        testLLI.Cost = 0;
        
        var LLIRecurrence = new LLIRecurrence();
        LLIRecurrence.Status = LLIRecurrenceStatus.On;
        LLIRecurrence.Frequency = LLIRecurrenceFrequency.Weekly;

        testLLI.Recurrence = LLIRecurrence;
        var createLLIResponse = await LLIService.CreateLLI(USER_HASH, testLLI);

        var readResponse = await LLIService.GetAllLLIFromUser(USER_HASH);

        if (readResponse.Output != null) {
            foreach (LLI lli in readResponse.Output) {
                testLLI = lli;
            }
        }
        
        // Act
        var deleteResponse = await LLIService.DeleteLLI(USER_HASH, testLLI);

        readResponse = await LLIService.GetAllLLIFromUser(USER_HASH);

        // Assert
        Assert.True(deleteResponse.HasError == false);
        Assert.True(readResponse.Output == null);
    }

    [Fact]
    public async void LLIServiceDeleteLLIShould_ThrowAnErrorIfUserHashIsEmpty()
    {
        // Arrange    
        var testLLI = new LLI();
        
        // Act
        var deleteResponse = await LLIService.DeleteLLI("", testLLI);

        // Assert
        Assert.True(deleteResponse.HasError == true);
        Assert.True(deleteResponse.ErrorMessage == "UserHash can not be empty");
    }

    [Fact]
    public async void LLIServiceDeleteLLIShould_ThrowAnErrorIfLLIIdIsEmpty()
    {
        // Arrange  
        var testLLI = new LLI();
        
        // Act
        var deleteResponse = await LLIService.DeleteLLI(USER_HASH, testLLI);

        // Assert
        Assert.True(deleteResponse.HasError == true);
        Assert.True(deleteResponse.ErrorMessage == "LLIId can not be empty");
    }

    [Fact]
    public async void LLIServiceDeleteLLIShould_ReturnNoRowsAffectedlIfTheUserDoesNotExistt()
    {
        // Arrange  
        var testLLI = new LLI();
        testLLI.LLIID = "1";
        
        // Act
        var deleteResponse = await LLIService.DeleteLLI("Nonexistant UserHash", testLLI);

        // Assert
        Assert.True(deleteResponse.HasError);
        Assert.True(deleteResponse.ErrorMessage == "Failed to delete LLI");
    }

    [Fact]
    public async void LLIServiceDeleteLLIShould_ReturnNoRowsAffectedlIfLLIDoesNotExistt()
    {
        // Arrange
        var testLLI = new LLI();
        testLLI.LLIID = "-1";
        
        // Act
        var deleteResponse = await LLIService.DeleteLLI(USER_HASH, testLLI);

        // Assert
        Assert.True(deleteResponse.HasError);
        Assert.True(deleteResponse.ErrorMessage == "Failed to delete LLI");
    }


}