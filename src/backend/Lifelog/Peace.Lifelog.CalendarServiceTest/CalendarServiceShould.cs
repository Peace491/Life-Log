namespace Peace.Lifelog.CalendarServiceTest;

using Peace.Lifelog.CalendarService;
using Peace.Lifelog.DataAccess;
using Peace.Lifelog.Infrastructure;
using Peace.Lifelog.LLI;
using Peace.Lifelog.Logging;
using Peace.Lifelog.PersonalNote;
using Peace.Lifelog.UserManagement;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Peace.Lifelog.Security;
using Peace.Lifelog.Email;

public class CalendarServiceShould : IAsyncLifetime, IDisposable
{

    private const int MAX_EXECUTION_TIME_IN_SECONDS = 3;

    #region Init Lifelog Account and Dispose

    private static CreateDataOnlyDAO createDataOnlyDAO = new CreateDataOnlyDAO();
    private static ReadDataOnlyDAO readDataOnlyDAO = new ReadDataOnlyDAO();
    private static UpdateDataOnlyDAO updateDataOnlyDAO = new UpdateDataOnlyDAO();
    private static DeleteDataOnlyDAO deleteDataOnlyDAO = new DeleteDataOnlyDAO();
    private static LogTarget logTarget = new LogTarget(createDataOnlyDAO, readDataOnlyDAO);
    private static Logging logging = new Logging(logTarget);
    private static LLIRepo lliRepo = new LLIRepo(createDataOnlyDAO, readDataOnlyDAO, updateDataOnlyDAO, deleteDataOnlyDAO);
    private LLIService LLIService = new LLIService(lliRepo, logging);
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
    
        IUserManagmentRepo userManagementRepo = new UserManagmentRepo(createDataOnlyDAO, readDataOnlyDAO, updateDataOnlyDAO, deleteDataOnlyDAO, logger);
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
    
         IUserManagmentRepo userManagementRepo = new UserManagmentRepo(createDataOnlyDAO, readDataOnlyDAO, updateDataOnlyDAO, deleteDataOnlyDAO, logger);
        AppUserManagementService appUserManagementService =  new AppUserManagementService(createDataOnlyDAO, readDataOnlyDAO, updateDataOnlyDAO, deleteDataOnlyDAO, logger);
        
        var lifelogUserManagementService = new LifelogUserManagementService(userManagementRepo, appUserManagementService, saltService, emailService, hashService);
        var testLifelogAccountRequest = new LifelogAccountRequest();
        testLifelogAccountRequest.UserId = ("UserId", USER_ID);
        var deleteAccountResponse = appUserManagementService.DeleteAccount(testLifelogAccountRequest);
    }

    #endregion

    #region Get Month Data Tests

    [Fact]
    public async void CalendarServiceShould_HaveCurrentMonthLLI()
    {
        // Arrange

        //Create LLI for the user
        // LLI with Current Month as Deadline
        int numberOfLLI = 5;
        string testLLIInboundDeadline = "Test LLI inbound deadline";
        DateTime inboundDatetime = new DateTime(2024, 3, 1);
        DateTime outboundDateTime = new DateTime(2024, 1, 1);

        for (int i = 1; i <= numberOfLLI; i++)
        {
            var testLLI = new LLI();
            testLLI.UserHash = USER_HASH;
            testLLI.Title = testLLIInboundDeadline;
            testLLI.Description = $"inbound Deadline Test Get LLI number {i}";
            testLLI.Category1 = LLICategory.Travel;
            testLLI.Category2 = LLICategory.Outdoor;
            testLLI.Category3 = LLICategory.Volunteering;
            testLLI.Status = LLIStatus.Active;
            testLLI.Visibility = LLIVisibility.Public;
            testLLI.Deadline = inboundDatetime.ToString("yyyy-MM-dd");
            testLLI.Cost = 0;

            var LLIRecurrence = new LLIRecurrence();
            LLIRecurrence.Status = LLIRecurrenceStatus.On;
            LLIRecurrence.Frequency = LLIRecurrenceFrequency.Weekly;

            testLLI.Recurrence = LLIRecurrence;
            var createLLIResponse = await LLIService.CreateLLI(USER_HASH, testLLI);
        }

        //Create LLI with no current month as deadline 
        string testLLIOutboundDeadline = "Test LLI outbound deadline";

        for (int i = 1; i <= numberOfLLI; i++)
        {
            var testLLI = new LLI();
            testLLI.UserHash = USER_HASH;
            testLLI.Title = testLLIOutboundDeadline;
            testLLI.Description = $"outbound Deadline Test Get LLI number {i}";
            testLLI.Category1 = LLICategory.Travel;
            testLLI.Category2 = LLICategory.Outdoor;
            testLLI.Category3 = LLICategory.Volunteering;
            testLLI.Status = LLIStatus.Active;
            testLLI.Visibility = LLIVisibility.Public;
            testLLI.Deadline = outboundDateTime.ToString("yyyy-MM-dd");
            testLLI.Cost = 0;

            var LLIRecurrence = new LLIRecurrence();
            LLIRecurrence.Status = LLIRecurrenceStatus.On;
            LLIRecurrence.Frequency = LLIRecurrenceFrequency.Weekly;

            testLLI.Recurrence = LLIRecurrence;
            var createLLIResponse = await LLIService.CreateLLI(USER_HASH, testLLI);
        }


        var calendarService = new CalendarService();

        // Act
        var monthLLIResponse = await calendarService.GetMonthLLI(USER_HASH, inboundDatetime.Month, inboundDatetime.Year);

        // Assert

        if (monthLLIResponse.Output is not null)
        {
            foreach (LLI outputLLI in monthLLIResponse.Output)
            {
                DateTime LLIdateTime;
                DateTime.TryParseExact(outputLLI.Deadline, "M/d/yyyy h:mm:ss tt", null, System.Globalization.DateTimeStyles.None, out LLIdateTime);

                Assert.True(inboundDatetime.Year == LLIdateTime.Year && inboundDatetime.Month == LLIdateTime.Month);

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
    public async void CalendarServiceShould_HaveCurrentMonthPN()
    {
        // Arrange

        //Create LLI for the user
        // LLI with Current Month as Deadline
        int numberOfPN = 5;
        string testPersonalNoteContent = "personal note creation with inbound date";
        DateTime inboundDatetime = new DateTime(2024, 3, 1);
        DateTime outboundDateTime = new DateTime(2024, 1, 1);

        for (int i = 1; i <= numberOfPN; i++)
        {

            var testPN = new PN();
            testPN.UserHash = USER_HASH;
            testPN.NoteContent = testPersonalNoteContent + $"{i}";
            testPN.NoteDate = inboundDatetime.ToString("yyyy-MM-dd");

            var createPersonalNoteResponse = await this.PNService.CreatePersonalNote(USER_HASH, testPN);
        }

        //Create LLI with no current month as deadline 

        string testPersonalNoteContent2 = "personal note creation with outbound date";

        for (int i = 1; i <= numberOfPN; i++)
        {

            var testPN = new PN();
            testPN.UserHash = USER_HASH;
            testPN.NoteContent = testPersonalNoteContent2 + $"{i}";
            testPN.NoteDate = outboundDateTime.ToString("yyyy-MM-dd");

            var createPersonalNoteResponse = await this.PNService.CreatePersonalNote(USER_HASH, testPN);
        }


        var calendarService = new CalendarService();

        // Act
        var monthPNResponse = await calendarService.GetMonthPN(USER_HASH, inboundDatetime.Month, inboundDatetime.Year);

        // Assert

        if (monthPNResponse.Output is not null)
        {
            foreach (PN outputPN in monthPNResponse.Output)
            {
                DateTime PNdateTime;

                DateTime.TryParseExact(outputPN.NoteDate, "M/d/yyyy h:mm:ss tt", null, System.Globalization.DateTimeStyles.None, out PNdateTime);

                Assert.True(inboundDatetime.Year == PNdateTime.Year && inboundDatetime.Month == PNdateTime.Month);

            }
        }
        else
        {
            Assert.True(false);
        }

        var deleteDataOnlyDAO = new DeleteDataOnlyDAO();

        var deletePNSql = $"DELETE FROM PersonalNote WHERE NoteDate=\"{inboundDatetime.ToString("yyyy-MM-dd")}\";";
        await deleteDataOnlyDAO.DeleteData(deletePNSql);

        var deletePNSql2 = $"DELETE FROM PersonalNote WHERE NoteDate=\"{outboundDateTime.ToString("yyyy-MM-dd")}\";";
        await deleteDataOnlyDAO.DeleteData(deletePNSql2);


    }


    #endregion


    [Fact]
    public async void CalendarServiceShould_CreateLLIWithCalendar()
    {
        //Arrange
        var timer = new Stopwatch();
        var calendarService = new CalendarService();
        var testTitle = "Test Title";

        var testLLI = new LLI();
        testLLI.UserHash = USER_HASH;
        testLLI.Title = testTitle;
        testLLI.Description = $"test description";
        testLLI.Category1 = LLICategory.Travel;
        testLLI.Category2 = LLICategory.Outdoor;
        testLLI.Category3 = LLICategory.Volunteering;
        testLLI.Status = LLIStatus.Active;
        testLLI.Visibility = LLIVisibility.Public;
        testLLI.Deadline = DateTime.Today.ToString("yyyy-MM-dd");
        testLLI.Cost = 0;

        var LLIRecurrence = new LLIRecurrence();
        LLIRecurrence.Status = LLIRecurrenceStatus.On;
        LLIRecurrence.Frequency = LLIRecurrenceFrequency.Weekly;

        testLLI.Recurrence = LLIRecurrence;



        //Act
        timer.Start();
        var validCreateLLIResponse = await calendarService.CreateLLIWithCalendar(USER_HASH, testLLI);
        timer.Stop();

        // Assert
        Assert.True(timer.Elapsed.TotalSeconds <= MAX_EXECUTION_TIME_IN_SECONDS);
        Assert.True(validCreateLLIResponse.HasError == false);

        //Cleanup

        var deleteDataOnlyDAO = new DeleteDataOnlyDAO();

        var deleteLLISql = $"DELETE FROM LLI WHERE Title=\"{testTitle}\";";
        await deleteDataOnlyDAO.DeleteData(deleteLLISql);
    }


    [Fact]
    public async void CalendarServiceShould_GetOnePNWithCalendar()
    {
        //Arrange
        var timer = new Stopwatch();
        var calendarService = new CalendarService();
        string testPersonalNoteContent = "personal note creation";

        var testPN = new PN();
        testPN.UserHash = USER_HASH;
        testPN.NoteContent = testPersonalNoteContent;
        testPN.NoteDate = DateTime.Today.ToString("yyyy-MM-dd");

        var createPersonalNoteResponse = await this.PNService.CreatePersonalNote(USER_HASH, testPN);

        //Act
        timer.Start();
        var getPNResponse = await calendarService.GetOnePNWithCalendar(USER_HASH, testPN);
        timer.Stop();

        // Assert
        Assert.True(getPNResponse.HasError == false);

        //cleanup
        var deleteDataOnlyDAO = new DeleteDataOnlyDAO();

        var deletePNSql = $"DELETE FROM PersonalNote WHERE NoteDate=\"{DateTime.Today.ToString("yyyy-MM-dd")}\";";
        await deleteDataOnlyDAO.DeleteData(deletePNSql);

    }

    [Fact]
    public async void CalendarServiceShould_CreatePNWithCalendar()
    {
        //Arrange
        var timer = new Stopwatch();
        var calendarService = new CalendarService();
        string testPersonalNoteContent = "personal note creation with calendar";

        var testPN = new PN();
        testPN.UserHash = USER_HASH;
        testPN.NoteContent = testPersonalNoteContent;
        testPN.NoteDate = DateTime.Today.ToString("yyyy-MM-dd");



        //Act
        timer.Start();
        var createPNResponse = await calendarService.CreatePNWithCalendar(USER_HASH, testPN);
        timer.Stop();

        // Assert
        Assert.True(createPNResponse.HasError == false);

        //cleanup
        var deleteDataOnlyDAO = new DeleteDataOnlyDAO();

        var deletePNSql = $"DELETE FROM PersonalNote WHERE NoteDate=\"{DateTime.Today.ToString("yyyy-MM-dd")}\";";
        await deleteDataOnlyDAO.DeleteData(deletePNSql);
    }

}



