namespace Peace.Lifelog.LLITest;

using System.Threading.Tasks;
using Peace.Lifelog.DataAccess;
using Peace.Lifelog.LLI;
using Peace.Lifelog.UserManagement;
using Peace.Lifelog.UserManagementTest;

public class LLIServiceShould : IAsyncLifetime, IDisposable
{
    private const string USER_ID = "TestLLIServiceAccount";
    private string USER_HASH = "";
    private const string ROLE = "Normal";

    private string DOB = DateTime.Today.ToString("yyyy-MM-dd");
    private string DEADLINE = DateTime.Today.ToString("yyyy-MM-dd");
    private const string ZIP_CODE = "90704";

    public async Task InitializeAsync()
    {   
        var lifelogUserManagementService = new LifelogUserManagementService();

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
        var appUserManagementService = new AppUserManagementService();
        var testLifelogAccountRequest = new LifelogAccountRequest();
        testLifelogAccountRequest.UserId = ("UserId", USER_ID);
        var deleteAccountResponse = appUserManagementService.DeleteAccount(testLifelogAccountRequest);
    }
    

    [Fact]
    public async void LLIServiceCreateLLIShould_CreateAnLLIInTheDatabase()
    {
        // Arrange
        string testLLITitle = "Test Create LLI Title";

        var LLIService = new LLIService();

        var testLLI = new LLI();
        testLLI.UserHash = USER_HASH;
        testLLI.Title = testLLITitle;
        testLLI.Description = "Test Create LLI Description";
        testLLI.Categories = [LLICategory.Travel, LLICategory.Art];
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
        Assert.True(createLLIResponse.HasError == false);
        Assert.NotNull(readResponse.Output);
        Assert.True(readResponse.Output.Count == 1);
        
        
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

        var LLIService = new LLIService();

        var testLLI = new LLI();
        testLLI.UserHash = invalidUserHash;
        testLLI.Title = testLLITitle;
        testLLI.Description = "Test Create LLI Description";
        testLLI.Categories = [LLICategory.Travel];
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
    public async void LLIServiceCreateLLIShould_ThrowAnErrorIfTheTitleIsTooLong()
    {
        // Arrange
        string testLLITitle = "Test Create LLI Title";

        var LLIService = new LLIService();

        var testLLI = new LLI();
        testLLI.UserHash = USER_HASH;
        testLLI.Title = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.";
        testLLI.Description = "Test Create LLI Description";
        testLLI.Categories = [LLICategory.Travel];
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
        Assert.True(createLLIResponse.ErrorMessage == "LLI Title is too long");
        Assert.Null(readResponse.Output);
    }

    [Fact]
    public async void LLIServiceCreateLLIShould_ThrowAnErrorIfDescriptionIsTooLong()
    {
        // Arrange
        string testLLITitle = "Test Create LLI Title";

        var LLIService = new LLIService();

        var testLLI = new LLI();
        testLLI.UserHash = "Test Invalid User Hash";
        testLLI.Title = "Test Create LLI Title";
        testLLI.Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.";
        testLLI.Categories = [LLICategory.Travel];
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
    public async void LLIServiceCreateLLIShould_ThrowAnErrorIfDeadlineIsOutOfRange()
    {
        // Arrange
        string testLLITitle = "Test LLI Title";

        var LLIService = new LLIService();

        var testLLI = new LLI();
        testLLI.UserHash = "Test Invalid User Hash";
        testLLI.Title = "Test LLI Title";
        testLLI.Description = "Test LLI Description";
        testLLI.Categories = [LLICategory.Travel];
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
    public async void LLIServiceCreateLLIShould_ThrowAnErrorIfCostIsLessThanZero()
    {
        // Arrange
        string testLLITitle = "Test LLI Title";

        var LLIService = new LLIService();

        var testLLI = new LLI();
        testLLI.UserHash = "Test Invalid User Hash";
        testLLI.Title = "Test LLI Title";
        testLLI.Description = "Test LLI Description";
        testLLI.Categories = [LLICategory.Travel];
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

        var LLIService = new LLIService();

        var testLLI = new LLI();
        testLLI.UserHash = "Test Invalid User Hash";
        testLLI.Title = "Test LLI Title";
        testLLI.Description = "Test LLI Description";
        testLLI.Categories = [LLICategory.Travel];
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
        Assert.Null(readResponse.Output);
    }

    [Fact]
    public async void LLIServiceCreateLLIShould_ThrowAnErrorIfAPropertyFieldViolateDBSchema()
    {
        // Arrange
        string testLLITitle = "Test LLI Create With Empty User Hash";

        var LLIService = new LLIService();

        var testLLI = new LLI();
        testLLI.UserHash = USER_HASH;
        testLLI.Title = "Test LLI Title";
        testLLI.Description = "Test LLI Description";
        testLLI.Categories = [LLICategory.Travel];
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
        var LLIService = new LLIService();

        var testLLI = new LLI();
        testLLI.UserHash = USER_HASH;
        testLLI.Title = testLLITitle;
        testLLI.Description = "Test LLI Description";
        testLLI.Categories = [LLICategory.Travel];
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

    [Fact]
    public async void LLIServiceGetAllLLIFromUserShould_GetAllLLIForAUser()
    {
        // Arrange
        string testLLITitle = "Test Get LLI Title";

        var LLIService = new LLIService();

        // Create 10 LLI
        int numberOfLLI = 10;

        for (int i = 1; i <= numberOfLLI; i++)
        {
            var testLLI = new LLI();
            testLLI.UserHash = USER_HASH;
            testLLI.Title = testLLITitle;
            testLLI.Description = $"Test Get LLI number {i}";
            testLLI.Categories = [LLICategory.Travel, LLICategory.Hobby];
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
        var readResponse = await LLIService.GetAllLLIFromUser(USER_HASH);

        // Assert
        Assert.True(readResponse.HasError == false);
        Assert.True(readResponse.Output != null);
        Assert.True(readResponse.Output.Count == numberOfLLI);

        // Cleanup
        var deleteDataOnlyDAO = new DeleteDataOnlyDAO();
        
        var deleteLLISql = $"DELETE FROM LLI WHERE Title=\"{testLLITitle}\";";

        await deleteDataOnlyDAO.DeleteData(deleteLLISql);
    }

    [Fact]
    public async void LLIServiceGetAllLLIFromUserShould_ThrowAnErrorIfUserHashIsEmpty()
    {
        // Arrange
        var LLIService = new LLIService();
        
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
        var LLIService = new LLIService();
        
        // Act
        var readResponse = await LLIService.GetAllLLIFromUser("NonExistentUser");

        // Assert
        Assert.True(readResponse.Output == null);
    }


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

        var LLIService = new LLIService();

        // Old LLI        
        var testOldLLI = new LLI();
        testOldLLI.UserHash = USER_HASH;
        testOldLLI.Title = testOldLLITitle;
        testOldLLI.Description = testOldLLIDescription;
        testOldLLI.Categories = [LLICategory.Travel, LLICategory.Hobby];
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
        testNewLLI.Categories = [LLICategory.Outdoor, LLICategory.Art];
        testNewLLI.Status = LLIStatus.Postponed;
        testNewLLI.Visibility = LLIVisibility.Private;
        testNewLLI.Cost = testNewLLICost;
        
        // Act
        var updateResponse = await LLIService.UpdateLLI(USER_HASH, testNewLLI);

        readResponse = await LLIService.GetAllLLIFromUser(USER_HASH);

        // Assert
        Assert.True(readResponse.Output != null);
        foreach (LLI lli in readResponse.Output)
        {
            Assert.True(lli.Title == testNewLLITitle);
            Assert.True(lli.Description == testNewLLIDescription);
            
            // Check if the two new categories match 
            Assert.True(testNewLLI.Categories.IndexOf(lli.Categories![0]) != -1); 
            Assert.True(testNewLLI.Categories.IndexOf(lli.Categories![1]) != -1);

            Assert.True(lli.Status == testNewLLI.Status);
            Assert.True(lli.Visibility == testNewLLI.Visibility);
            Assert.True(lli.Cost == testNewLLICost);
        }
        
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
        var LLIService = new LLIService();

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
        var LLIService = new LLIService();

        // New LLI
        var testNewLLI = new LLI();
        testNewLLI.Title = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.";
        
        // Act
        var updateResponse = await LLIService.UpdateLLI("UserHash", testNewLLI);

        // Assert
        Assert.True(updateResponse.HasError == true);
        Assert.True(updateResponse.ErrorMessage == "LLI Title is too long");
        Assert.Null(updateResponse.Output);
    }

    [Fact]
    public async void LLIServiceUpdateLLIShould_ThrowAnErrorIfDescriptionIsTooLong()
    {
        // Arrange
        var LLIService = new LLIService();

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
    public async void LLIServiceUpdateLLIShould_ThrowAnErrorIfDeadlineIsOutOfRange()
    {
        // Arrange
        var LLIService = new LLIService();

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
        var LLIService = new LLIService();

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
    public async void LLIServiceUpdateLLIShould_ReturnNoRowsAffectedlIfTheUserDoesNotExist()
    {
        // Arrange
        var LLIService = new LLIService();

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

        var LLIService = new LLIService();

        // Old LLI        
        var testOldLLI = new LLI();
        testOldLLI.UserHash = USER_HASH;
        testOldLLI.Title = testOldLLITitle;
        testOldLLI.Description = testOldLLIDescription;
        testOldLLI.Categories = [LLICategory.Travel];
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
    public async void LLIServiceDeleteLLIShould_DeleteALLInTheDatabase()
    {
        // Arrange
        string testLLITitle = "Test LLI Title";

        var LLIService = new LLIService();
        
        var testLLI = new LLI();
        testLLI.UserHash = USER_HASH;
        testLLI.Title = testLLITitle;
        testLLI.Description = $"Test Delete LLI";
        testLLI.Categories = [LLICategory.Travel];
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

        var LLIService = new LLIService();
        
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

        var LLIService = new LLIService();
        
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

        var LLIService = new LLIService();
        
        var testLLI = new LLI();
        testLLI.LLIID = "1";
        
        // Act
        var deleteResponse = await LLIService.DeleteLLI("Nonexistant UserHash", testLLI);

        // Assert
        Assert.True(deleteResponse.Output is not null);
        foreach (int rowsAffected in deleteResponse.Output)
        {
            Assert.True(rowsAffected == 0);
        }
    }

    [Fact]
    public async void LLIServiceDeleteLLIShould_ReturnNoRowsAffectedlIfLLIDoesNotExistt()
    {
        // Arrange

        var LLIService = new LLIService();
        
        var testLLI = new LLI();
        testLLI.LLIID = "-1";
        
        // Act
        var deleteResponse = await LLIService.DeleteLLI(USER_HASH, testLLI);

        // Assert
        Assert.True(deleteResponse.Output is not null);
        foreach (int rowsAffected in deleteResponse.Output)
        {
            Assert.True(rowsAffected == 0);
        }
    }


}