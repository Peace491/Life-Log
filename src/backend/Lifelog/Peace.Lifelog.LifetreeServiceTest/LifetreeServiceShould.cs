namespace Peace.Lifelog.LifetreeServiceShould;

using Peace.Lifelog.DataAccess;
using Peace.Lifelog.LifetreeService;
using Peace.Lifelog.Logging;
using Peace.Lifelog.LLI;
using Peace.Lifelog.PersonalNote;
using Peace.Lifelog.UserManagement;
using Peace.Lifelog.Infrastructure;
using System.Diagnostics;
using Peace.Lifelog.Security;
using Peace.Lifelog.Email;

    public class LifetreeServiceShould : IAsyncLifetime, IDisposable
    {

    private const int MAX_EXECUTION_TIME_IN_SECONDS = 3;

    #region Init Lifelog Account and Dispose

    private static CreateDataOnlyDAO createDataOnlyDAO = new CreateDataOnlyDAO();
    private static ReadDataOnlyDAO readDataOnlyDAO = new ReadDataOnlyDAO();
    private static UpdateDataOnlyDAO updateDataOnlyDAO = new UpdateDataOnlyDAO();
    private static DeleteDataOnlyDAO deleteDataOnlyDAO = new DeleteDataOnlyDAO();
    private static LogTarget logTarget = new LogTarget(createDataOnlyDAO, readDataOnlyDAO);
    private static Logging logging = new Logging(logTarget);
    private LLIService LLIService = new LLIService(createDataOnlyDAO, readDataOnlyDAO, updateDataOnlyDAO, deleteDataOnlyDAO, logging);
    private static IPersonalNoteRepo personalNoteRepo = new PersonalNoteRepo(createDataOnlyDAO, readDataOnlyDAO, updateDataOnlyDAO, deleteDataOnlyDAO);
    private PersonalNoteService PNService = new PersonalNoteService(personalNoteRepo, logging);

    private const string USER_ID = "TestLLIServiceAccount2";
    private string USER_HASH = "";
    private const string ROLE = "Normal";
    private string DOB = DateTime.Today.ToString("yyyy-MM-dd");
    private const string ZIP_CODE = "90704";

    public async Task InitializeAsync()
    {
        ICreateDataOnlyDAO createDataOnlyDAO = new CreateDataOnlyDAO();
        IReadDataOnlyDAO readDataOnlyDAO = new ReadDataOnlyDAO();
        IUpdateDataOnlyDAO updateDataOnlyDAO = new UpdateDataOnlyDAO();
        IDeleteDataOnlyDAO deleteDataOnlyDAO = new DeleteDataOnlyDAO();
        ILogTarget logTarget = new LogTarget(createDataOnlyDAO, readDataOnlyDAO);
        ILogging logger = new Logging(logTarget);
        ISaltService saltService = new SaltService();
        IHashService hashService = new HashService();
        IEmailService   emailService = new EmailService();
    
        IUserManagmentRepo userManagementRepo = new UserManagmentRepo(createDataOnlyDAO, readDataOnlyDAO, deleteDataOnlyDAO, logger);
        AppUserManagementService appUserManagementService =  new AppUserManagementService(createDataOnlyDAO, readDataOnlyDAO, updateDataOnlyDAO, deleteDataOnlyDAO, logger);
        
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
        IEmailService   emailService = new EmailService();
    
        IUserManagmentRepo userManagementRepo = new UserManagmentRepo(createDataOnlyDAO, readDataOnlyDAO, deleteDataOnlyDAO, logger);
        AppUserManagementService appUserManagementService =  new AppUserManagementService(createDataOnlyDAO, readDataOnlyDAO, updateDataOnlyDAO, deleteDataOnlyDAO, logger);
        
        var lifelogUserManagementService = new LifelogUserManagementService(userManagementRepo, appUserManagementService, saltService, emailService, hashService);
        var testLifelogAccountRequest = new LifelogAccountRequest();
        testLifelogAccountRequest.UserId = ("UserId", USER_ID);
        var deleteAccountResponse = appUserManagementService.DeleteAccount(testLifelogAccountRequest);
    }

    #endregion


        [Fact]
        public async Task LifetreeServiceShould_GiveAllCompletedLLI()
        {
        // Arrange

        //Create LLI for the user
        // LLI with Current Month as Deadline
        int numberOfLLI = 5;
        string testLLIInboundDeadline = "Test completed LLI";
        
        DateTime lliDateTime = new DateTime(2024, 1, 1);

        for (int i = 1; i <= numberOfLLI; i++)
        {
            var testLLI = new LLI();
            testLLI.UserHash = USER_HASH;
            testLLI.Title = testLLIInboundDeadline;
            testLLI.Description = $"completed Test Get LLI number {i}";
            testLLI.Category1 = LLICategory.Travel;
            testLLI.Category2 = LLICategory.Outdoor;
            testLLI.Category3 = LLICategory.Volunteering;
            testLLI.Status = LLIStatus.Completed;
            testLLI.Visibility = LLIVisibility.Public;
            testLLI.Deadline = lliDateTime.ToString("yyyy-MM-dd");
            testLLI.Cost = 0;

            var LLIRecurrence = new LLIRecurrence();
            LLIRecurrence.Status = LLIRecurrenceStatus.On;
            LLIRecurrence.Frequency = LLIRecurrenceFrequency.Weekly;

            testLLI.Recurrence = LLIRecurrence;
            var createLLIResponse = await LLIService.CreateLLI(USER_HASH, testLLI);
        }

        //Create LLI with no current month as deadline 
        string testLLIOutboundDeadline = "Test active LLI";

        for (int i = 1; i <= numberOfLLI; i++)
        {
            var testLLI = new LLI();
            testLLI.UserHash = USER_HASH;
            testLLI.Title = testLLIOutboundDeadline;
            testLLI.Description = $"Active Test Get LLI number {i}";
            testLLI.Category1 = LLICategory.Travel;
            testLLI.Category2 = LLICategory.Outdoor;
            testLLI.Category3 = LLICategory.Volunteering;
            testLLI.Status = LLIStatus.Active;
            testLLI.Visibility = LLIVisibility.Public;
            testLLI.Deadline = lliDateTime.ToString("yyyy-MM-dd");
            testLLI.Cost = 0;

            var LLIRecurrence = new LLIRecurrence();
            LLIRecurrence.Status = LLIRecurrenceStatus.On;
            LLIRecurrence.Frequency = LLIRecurrenceFrequency.Weekly;

            testLLI.Recurrence = LLIRecurrence;
            var createLLIResponse = await LLIService.CreateLLI(USER_HASH, testLLI);
        }


        var lifetreeService = new LifetreeService();

        // Act
        var completedLLIResponse = await lifetreeService.getAllCompletedLLI(USER_HASH);

        // Assert

        if (completedLLIResponse.Output is not null)
        {
            foreach (LLI outputLLI in completedLLIResponse.Output)
            {
                
                Assert.True(outputLLI.Status == LLIStatus.Completed);

            }
        }
        else
        {
            Assert.True(false);
        }

        var deleteDataOnlyDAO = new DeleteDataOnlyDAO();

        var deleteLLISql = $"DELETE FROM LLI WHERE Title=\"{testLLIInboundDeadline}\";";
        await deleteDataOnlyDAO.DeleteData(deleteLLISql);

        deleteLLISql = $"DELETE FROM LLI WHERE Title=\"{testLLIOutboundDeadline}\";";
        await deleteDataOnlyDAO.DeleteData(deleteLLISql);
    }

        [Fact]
        public async Task LifetreeServiceShould_GiveAPersonalNote()
        {
        //Arrange
        var timer = new Stopwatch();
        var lifetreeService = new LifetreeService();
        string testPersonalNoteContent = "personal note creation";

        var testPN = new PN();
        testPN.UserHash = USER_HASH;
        testPN.NoteContent = testPersonalNoteContent;
        testPN.NoteDate = DateTime.Today.ToString("yyyy-MM-dd");

        var createPersonalNoteResponse = await this.PNService.CreatePersonalNote(USER_HASH, testPN);

        //Act
        timer.Start();
        var getPNResponse = await lifetreeService.GetOnePNWithLifetree(USER_HASH, testPN);
        timer.Stop();

        // Assert
        Assert.True(getPNResponse.HasError == false);

        //cleanup
        var deleteDataOnlyDAO = new DeleteDataOnlyDAO();

        var deletePNSql = $"DELETE FROM PersonalNote WHERE NoteDate=\"{DateTime.Today.ToString("yyyy-MM-dd")}\";";
        await deleteDataOnlyDAO.DeleteData(deletePNSql);
    }

    
        [Fact]
        public async Task LifetreeServiceShould_CreateAPersonalNote()
        {

        //Arrange
        var timer = new Stopwatch();
        var lifetreeService = new LifetreeService();
        string testPersonalNoteContent = "personal note creation with calendar";

        var testPN = new PN();
        testPN.UserHash = USER_HASH;
        testPN.NoteContent = testPersonalNoteContent;
        testPN.NoteDate = DateTime.Today.ToString("yyyy-MM-dd");



        //Act
        timer.Start();
        var createPNResponse = await lifetreeService.CreatePNWithLifetree(USER_HASH, testPN);
        timer.Stop();

        // Assert
        Assert.True(createPNResponse.HasError == false);

        //cleanup
        var deleteDataOnlyDAO = new DeleteDataOnlyDAO();

        var deletePNSql = $"DELETE FROM PersonalNote WHERE NoteDate=\"{DateTime.Today.ToString("yyyy-MM-dd")}\";";
        await deleteDataOnlyDAO.DeleteData(deletePNSql);
    }
}
