namespace Peace.Lifelog.LLITest;

using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using DomainModels;
using Peace.Lifelog.DataAccess;
using Peace.Lifelog.LLI;
using Peace.Lifelog.UserManagement;
using Peace.Lifelog.UserManagementTest;

public class LLIServiceShould : IAsyncLifetime, IDisposable
{
    private const string USER_ID = "TestLLIServiceAccount";
    private string USER_HASH = "";
    private const string MFA_ID = "TestLLIServiceMFA";
    private const string ROLE = "Normal";

    private string DOB = DateTime.Today.ToString("yyyy-MM-dd");
    private string DEADLINE = DateTime.Today.ToString("yyyy-MM-dd");
    private const string ZIP_CODE = "92612";

    public async Task InitializeAsync()
    {
        // TODO: Fix one Lifelog User is implemented
        
        var lifelogUserManagementService = new LifelogUserManagementService();

        var testLifelogAccountRequest = new LifelogAccountRequest();
        testLifelogAccountRequest.UserId = ("UserId", USER_ID);
        testLifelogAccountRequest.MfaId = ("MfaId", MFA_ID);
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
    public async void LLIServiceShould_CreateAnLLIInTheDatabase()
    {
        // Arrange
        string testLLITitle = "Test Create LLI Title";

        var LLIService = new LLIService();

        var testLLI = new LLI();
        testLLI.UserHash = USER_HASH;
        testLLI.Title = testLLITitle;
        testLLI.Description = "Test Create LLI Description";
        testLLI.Category = LLICategory.Travel;
        testLLI.Status = LLIStatus.Active;
        testLLI.Visibility = LLIVisibility.Public;
        testLLI.Deadline = DEADLINE;
        testLLI.Cost = 0;
        
        var LLIRecurrence = new LLIRecurrence();
        LLIRecurrence.Status = LLIRecurrenceStatus.On;
        LLIRecurrence.Frequency = LLIRecurrenceFrequency.Weekly;

        testLLI.Recurrence = LLIRecurrence;

        // Act
        var createLLIResponse = await LLIService.CreateLLI(testLLI);

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
    public async void LLIServiceShould_ThrowAnErrorIfTheUserHashIsInvalid()
    {
        // Arrange
        string testLLITitle = "Test LLI Title";

        var LLIService = new LLIService();

        var testLLI = new LLI();
        testLLI.UserHash = "Test Invalid User Hash";
        testLLI.Title = testLLITitle;
        testLLI.Description = "Test Create LLI Description";
        testLLI.Category = LLICategory.Travel;
        testLLI.Status = LLIStatus.Active;
        testLLI.Visibility = LLIVisibility.Public;
        testLLI.Deadline = DEADLINE;
        testLLI.Cost = 0;
        
        var LLIRecurrence = new LLIRecurrence();
        LLIRecurrence.Status = LLIRecurrenceStatus.On;
        LLIRecurrence.Frequency = LLIRecurrenceFrequency.Weekly;

        testLLI.Recurrence = LLIRecurrence;

        // Act
        var createLLIResponse = await LLIService.CreateLLI(testLLI);

        var readDataOnlyDAO = new ReadDataOnlyDAO();
        var readLLISql = $"SELECT LLIId FROM LLI WHERE Title=\"{testLLITitle}\"";
        var readResponse = await readDataOnlyDAO.ReadData(readLLISql);

        // Assert
        Assert.True(createLLIResponse.HasError == true);
        Assert.Null(readResponse.Output);
    }

    [Fact]
    public async void LLIServiceShould_ThrowAnErrorIfTheTitleIsTooLong()
    {
        // Arrange
        string testLLITitle = "Test Create LLI Title";

        var LLIService = new LLIService();

        var testLLI = new LLI();
        testLLI.UserHash = USER_HASH;
        testLLI.Title = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.";
        testLLI.Description = "Test Create LLI Description";
        testLLI.Category = LLICategory.Travel;
        testLLI.Status = LLIStatus.Active;
        testLLI.Visibility = LLIVisibility.Public;
        testLLI.Deadline = DEADLINE;
        testLLI.Cost = 0;
        
        var LLIRecurrence = new LLIRecurrence();
        LLIRecurrence.Status = LLIRecurrenceStatus.On;
        LLIRecurrence.Frequency = LLIRecurrenceFrequency.Weekly;

        testLLI.Recurrence = LLIRecurrence;

        // Act
        var createLLIResponse = await LLIService.CreateLLI(testLLI);

        var readDataOnlyDAO = new ReadDataOnlyDAO();
        var readLLISql = $"SELECT LLIId FROM LLI WHERE Title=\"{testLLITitle}\"";
        var readResponse = await readDataOnlyDAO.ReadData(readLLISql);

        // Assert
        Assert.True(createLLIResponse.HasError == true);
        Assert.True(createLLIResponse.ErrorMessage == "LLI Title is too long");
        Assert.Null(readResponse.Output);
    }

    [Fact]
    public async void LLIServiceShould_ThrowAnErrorIfDescriptionIsTooLong()
    {
        // Arrange
        string testLLITitle = "Test Create LLI Title";

        var LLIService = new LLIService();

        var testLLI = new LLI();
        testLLI.UserHash = "Test Invalid User Hash";
        testLLI.Title = "Test Create LLI Title";
        testLLI.Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.";
        testLLI.Category = LLICategory.Travel;
        testLLI.Status = LLIStatus.Active;
        testLLI.Visibility = LLIVisibility.Public;
        testLLI.Deadline = DEADLINE;
        testLLI.Cost = 0;
        
        var LLIRecurrence = new LLIRecurrence();
        LLIRecurrence.Status = LLIRecurrenceStatus.On;
        LLIRecurrence.Frequency = LLIRecurrenceFrequency.Weekly;

        testLLI.Recurrence = LLIRecurrence;

        // Act
        var createLLIResponse = await LLIService.CreateLLI(testLLI);

        var readDataOnlyDAO = new ReadDataOnlyDAO();
        var readLLISql = $"SELECT LLIId FROM LLI WHERE Title=\"{testLLITitle}\"";
        var readResponse = await readDataOnlyDAO.ReadData(readLLISql);

        // Assert
        Assert.True(createLLIResponse.HasError == true);
        Assert.True(createLLIResponse.ErrorMessage == "LLI Description is too long");
        Assert.Null(readResponse.Output);
    }

    // [Fact]
    // public async void LLIServiceShould_ThrowAnErrorIfDeadlineIsOutOfRange()
    // {
    //     // Arrange
    //     string testLLITitle = "Test LLI Title";

    //     var LLIService = new LLIService();

    //     var testLLI = new LLI();
    //     testLLI.UserHash = "Test Invalid User Hash";
    //     testLLI.Title = "Test LLI Title";
    //     testLLI.Description = "Test LLI Description";
    //     testLLI.Category = LLICategory.Travel;
    //     testLLI.Status = LLIStatus.Active;
    //     testLLI.Visibility = LLIVisibility.Public;
    //     testLLI.Deadline = "1800-11-11";
    //     testLLI.Cost = 0;

    //     var LLIRecurrence = new LLIRecurrence();
    //     LLIRecurrence.Status = LLIRecurrenceStatus.On;
    //     LLIRecurrence.Frequency = LLIRecurrenceFrequency.Weekly;

    //     testLLI.Recurrence = LLIRecurrence;

    //     // Act
    //     var createLLIResponse = await LLIService.CreateLLI(testLLI);

    //     var readDataOnlyDAO = new ReadDataOnlyDAO();
    //     var readLLISql = $"SELECT LLIId FROM LLI WHERE Title=\"{testLLITitle}\"";
    //     var readResponse = await readDataOnlyDAO.ReadData(readLLISql);

    //     //Assert
    //     Assert.True(createLLIResponse.HasError == true);
    //     Assert.True(createLLIResponse.ErrorMessage == "LLI Date is out of range");
    //     Assert.Null(readResponse.Output);
    // }

    [Fact]
    public async void LLIServiceShould_GetAllLLIForAUser()
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
            testLLI.Category = LLICategory.Travel;
            testLLI.Status = LLIStatus.Active;
            testLLI.Visibility = LLIVisibility.Public;
            testLLI.Deadline = DEADLINE;
            testLLI.Cost = 0;
            
            var LLIRecurrence = new LLIRecurrence();
            LLIRecurrence.Status = LLIRecurrenceStatus.On;
            LLIRecurrence.Frequency = LLIRecurrenceFrequency.Weekly;

            testLLI.Recurrence = LLIRecurrence;
            var createLLIResponse = await LLIService.CreateLLI(testLLI);
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
    public async void LLIServiceShould_UpdateALLIInTheDatabase()
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
        testOldLLI.Category = LLICategory.Travel;
        testOldLLI.Status = LLIStatus.Active;
        testOldLLI.Visibility = LLIVisibility.Public;
        testOldLLI.Deadline = DEADLINE;
        testOldLLI.Cost = testOldLLICost;
        
        var LLIRecurrence = new LLIRecurrence();
        LLIRecurrence.Status = LLIRecurrenceStatus.On;
        LLIRecurrence.Frequency = LLIRecurrenceFrequency.Weekly;

        testOldLLI.Recurrence = LLIRecurrence;
        var createLLIResponse = await LLIService.CreateLLI(testOldLLI);

        // New LLI
        var testNewLLI = new LLI();
        testNewLLI.Title = testNewLLITitle;
        testNewLLI.Description = testNewLLIDescription;
        testNewLLI.Category = LLICategory.Outdoor;
        testNewLLI.Status = LLIStatus.Postponed;
        testNewLLI.Visibility = LLIVisibility.Private;
        testNewLLI.Cost = testNewLLICost;
        
        // Act
        var updateResponse = await LLIService.UpdateLLI(USER_HASH, testOldLLI, testNewLLI);

        var readResponse = await LLIService.GetAllLLIFromUser(USER_HASH);

        // Assert
        Assert.True(readResponse.Output != null);
        foreach (LLI lli in readResponse.Output)
        {
            Assert.True(lli.Title == testNewLLITitle);
            Assert.True(lli.Description == testNewLLIDescription);
            Assert.True(lli.Category == testNewLLI.Category);
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
    public async void LLIServiceShould_DeleteALLInTheDatabase()
    {
        // Arrange
        string testLLITitle = "Test LLI Title";

        var LLIService = new LLIService();
        
        var testLLI = new LLI();
        testLLI.UserHash = USER_HASH;
        testLLI.Title = testLLITitle;
        testLLI.Description = $"Test Delete LLI";
        testLLI.Category = LLICategory.Travel;
        testLLI.Status = LLIStatus.Active;
        testLLI.Visibility = LLIVisibility.Public;
        testLLI.Deadline = DEADLINE;
        testLLI.Cost = 0;
        
        var LLIRecurrence = new LLIRecurrence();
        LLIRecurrence.Status = LLIRecurrenceStatus.On;
        LLIRecurrence.Frequency = LLIRecurrenceFrequency.Weekly;

        testLLI.Recurrence = LLIRecurrence;
        var createLLIResponse = await LLIService.CreateLLI(testLLI);
        
        
        // Act
        var deleteResponse = await LLIService.DeleteLLI(USER_HASH, testLLI);

        var readResponse = await LLIService.GetAllLLIFromUser(USER_HASH);

        // Assert
        Assert.True(deleteResponse.HasError == false);
        Assert.True(readResponse.Output == null);
    }


}