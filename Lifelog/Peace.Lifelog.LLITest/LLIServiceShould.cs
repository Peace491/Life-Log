namespace Peace.Lifelog.LLITest;

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
        string testLLITitle = "Test LLI Title";

        var LLIService = new LLIService();

        var testLLI = new LLI();
        testLLI.UserHash = USER_HASH;
        testLLI.Title = testLLITitle;
        testLLI.Description = "Test LLI Description";
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

        //Assert
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
        testLLI.Description = "Test LLI Description";
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

        //Assert
        Assert.True(createLLIResponse.HasError == true);
        Assert.Null(readResponse.Output);
    }

    [Fact]
    public async void LLIServiceShould_ThrowAnErrorIfTheTitleIsTooLong()
    {
        // Arrange
        string testLLITitle = "Test LLI Title";

        var LLIService = new LLIService();

        var testLLI = new LLI();
        testLLI.UserHash = USER_HASH;
        testLLI.Title = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.";
        testLLI.Description = "Test LLI Description";
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

        //Assert
        Assert.True(createLLIResponse.HasError == true);
        Assert.True(createLLIResponse.ErrorMessage == "LLI Title is too long");
        Assert.Null(readResponse.Output);
    }

    [Fact]
    public async void LLIServiceShould_ThrowAnErrorIfDescriptionIsTooLong()
    {
        // Arrange
        string testLLITitle = "Test LLI Title";

        var LLIService = new LLIService();

        var testLLI = new LLI();
        testLLI.UserHash = "Test Invalid User Hash";
        testLLI.Title = "Test LLI Title";
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

        //Assert
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


}