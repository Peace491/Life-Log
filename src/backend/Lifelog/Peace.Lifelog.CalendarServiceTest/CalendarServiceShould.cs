namespace Peace.Lifelog.CalendarServiceTest;

using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Peace.Lifelog.CalendarService;
using Peace.Lifelog.CalendarService.Models;
using Peace.Lifelog.LLI;
using Peace.Lifelog.UserManagement;
using Peace.Lifelog.UserManagementTest;
using Peace.Lifelog.DataAccess;
using Peace.Lifelog.Logging;

public class CalendarServiceShould : IAsyncLifetime, IDisposable
{

    private const int MAX_EXECUTION_TIME_IN_SECONDS = 3;

    #region Init Lifelog Account and Dispose

    private static CreateDataOnlyDAO createDataOnlyDAO = new CreateDataOnlyDAO();
    private static ReadDataOnlyDAO readDataOnlyDAO = new ReadDataOnlyDAO();
    private static UpdateDataOnlyDAO updateDataOnlyDAO = new UpdateDataOnlyDAO();
    private static DeleteDataOnlyDAO deleteDataOnlyDAO = new DeleteDataOnlyDAO();
    private static LogTarget logTarget = new LogTarget(createDataOnlyDAO);
    private static Logging logging = new Logging(logTarget);
    private LLIService LLIService = new LLIService(createDataOnlyDAO, readDataOnlyDAO, updateDataOnlyDAO, deleteDataOnlyDAO, logging);

    private const string USER_ID = "TestLLIServiceAccount2";
    private string USER_HASH = "";
    private const string ROLE = "Normal";
    private string DOB = DateTime.Today.ToString("yyyy-MM-dd");
    private const string ZIP_CODE = "90704";

    public async Task InitializeAsync()
    {
        var appUserManagementService = new AppUserManagementService();
        var lifelogUserManagementService = new LifelogUserManagementService();

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
        var appUserManagementService = new AppUserManagementService();
        var testLifelogAccountRequest = new LifelogAccountRequest();
        testLifelogAccountRequest.UserId = ("UserId", USER_ID);
        var deleteAccountResponse = appUserManagementService.DeleteAccount(testLifelogAccountRequest);
    }

    #endregion

    #region Get Month Data Tests

        [Fact]
        public async void CalendarServiceShould_HaveCurrentMonthData()
        {

        // Arrange
        var calendarService = new CalendarService();
        DateTime currentDate = DateTime.Now;
        DateTime firstDayOfMonth = new DateTime(currentDate.Year, currentDate.Month, 1);

        // Act
        var correctMonthDataResponse = await calendarService.GetMonthData(USER_HASH);

        // Assert
        Assert.True(correctMonthDataResponse.HasError == false);

        

        var monthData = new MonthData();
        if (correctMonthDataResponse.Output is not null)
        {
            foreach (MonthData output in correctMonthDataResponse.Output)
            {
                monthData = output;
            }
        }

        Assert.True(monthData.Month == currentDate.Month);
        if (calendarService.MonthState.Year == DateTime.Now.Year && calendarService.MonthState.Month == DateTime.Now.Month)
        { Assert.True(monthData.CurrDay == currentDate.Day); }
        else { Assert.True(monthData.CurrDay == -1); }
        Assert.True(monthData.NumOfDayInMonth == DateTime.DaysInMonth(currentDate.Year, currentDate.Month));
        Assert.True(monthData.DayOfTheWeekFor1stDay == firstDayOfMonth.DayOfWeek.ToString());

    }

        [Fact]
        public async void CalendarServiceShould_HaveNextMonthData()
        {
        // Arrange
        var calendarService = new CalendarService();
        DateTime thisMonthDate = DateTime.Now;
        thisMonthDate = thisMonthDate.AddMonths(1).AddMonths(1);
        DateTime firstDayOfMonth = new DateTime(thisMonthDate.Year, thisMonthDate.Month, 1);

        // Act
        var nextMonthDataResponse0 = await calendarService.NextMonth(USER_HASH);
        var nextMonthDataResponse = await calendarService.NextMonth(USER_HASH);


        // Assert
        Assert.True(nextMonthDataResponse.HasError == false);


        var monthData = new MonthData();
        if (nextMonthDataResponse.Output is not null)
        {
            foreach (MonthData output in nextMonthDataResponse.Output)
            {
                monthData = output;
            }
        }

        // change to next month
        Assert.True(monthData.Month == thisMonthDate.Month);
        Assert.True(monthData.Year == thisMonthDate.Year);
        if (calendarService.MonthState.Year == DateTime.Now.Year && calendarService.MonthState.Month == DateTime.Now.Month)
        { Assert.True(monthData.CurrDay == thisMonthDate.Day); }
        else { Assert.True(monthData.CurrDay == -1); }
        Assert.True(monthData.NumOfDayInMonth == DateTime.DaysInMonth(thisMonthDate.Year, thisMonthDate.Month));
        Assert.True(monthData.DayOfTheWeekFor1stDay == firstDayOfMonth.DayOfWeek.ToString());
    }

        [Fact]
        public async void CalendarServiceShould_HavePrevMonthData()
        {

        // Arrange
        var calendarService = new CalendarService();
        DateTime thisMonthDate = DateTime.Now;
        thisMonthDate = thisMonthDate.AddMonths(-1);
        DateTime firstDayOfMonth = new DateTime(thisMonthDate.Year, thisMonthDate.Month, 1);

        // Act
        var prevMonthDataResponse = await calendarService.PrevMonth(USER_HASH);

        // Assert
        Assert.True(prevMonthDataResponse.HasError == false);

        var monthData = new MonthData();
        if (prevMonthDataResponse.Output is not null)
        {
            foreach (MonthData output in prevMonthDataResponse.Output)
            {
                monthData = output;
            }
        }


        // change to next month
        Assert.True(monthData.Month == thisMonthDate.Month);
        Assert.True(monthData.Year == thisMonthDate.Year);
        if (calendarService.MonthState.Year == DateTime.Now.Year && calendarService.MonthState.Month == DateTime.Now.Month)
        { Assert.True(monthData.CurrDay == thisMonthDate.Day); }
        else { Assert.True(monthData.CurrDay == -1); }
        Assert.True(monthData.NumOfDayInMonth == DateTime.DaysInMonth(thisMonthDate.Year, thisMonthDate.Month));
        Assert.True(monthData.DayOfTheWeekFor1stDay == firstDayOfMonth.DayOfWeek.ToString());

    }

        [Fact]
        public async void CalendarServiceShould_HaveCurrentMonthLLI()
        {
        // Arrange

        //Create LLI for the user
        // LLI with Current Month as Deadline
        int numberOfLLI = 5;
        string testLLIInboundDeadline = "Test LLI inbound deadline";

        for (int i = 1; i <= numberOfLLI; i++)
        {
            var testLLI = new LLI();
            testLLI.UserHash = USER_HASH;
            testLLI.Title = testLLIInboundDeadline;
            testLLI.Description = $"inbound Deadline Test Get LLI number {i}";
            testLLI.Categories = [LLICategory.Travel, LLICategory.Hobby];
            testLLI.Status = LLIStatus.Active;
            testLLI.Visibility = LLIVisibility.Public;
            testLLI.Deadline = DateTime.Today.ToString("yyyy-MM-dd");
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
            testLLI.Categories = [LLICategory.Travel, LLICategory.Hobby];
            testLLI.Status = LLIStatus.Active;
            testLLI.Visibility = LLIVisibility.Public;
            var outboundDatetime = DateTime.Today.AddMonths(-1);
            testLLI.Deadline = outboundDatetime.ToString("yyyy-MM-dd");
            testLLI.Cost = 0;

            var LLIRecurrence = new LLIRecurrence();
            LLIRecurrence.Status = LLIRecurrenceStatus.On;
            LLIRecurrence.Frequency = LLIRecurrenceFrequency.Weekly;

            testLLI.Recurrence = LLIRecurrence;
            var createLLIResponse = await LLIService.CreateLLI(USER_HASH, testLLI);
        }


        var calendarService = new CalendarService();

        // Act
        var monthData = await calendarService.GetMonthLLI(USER_HASH);

        // Assert
        var currMonthDateTime = DateTime.Now;


        
        if (monthData.LLIEvent is not null)
        {
            foreach (LLI outputLLI in monthData.LLIEvent)
            {
                DateTime LLIdateTime;
                DateTime.TryParseExact(outputLLI.Deadline, "M/d/yyyy h:mm:ss tt", null, System.Globalization.DateTimeStyles.None, out LLIdateTime);

                Assert.True(currMonthDateTime.Year == LLIdateTime.Year && currMonthDateTime.Month == LLIdateTime.Month);

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
        public void CalendarServiceShould_HaveCurrentMonthPN()
        {
            // Arrange

            // Act

            // Assert
            throw new NotImplementedException();

        }


        #endregion

        [Fact]
        public async void CalendarServiceShould_GetMonthDataInLessThan3Seconds()
        {

        // Arrange
        var timer = new Stopwatch();
        var calendarService = new CalendarService();

        // Act
        timer.Start();
        var calendarResponse = await calendarService.GetMonthData(USER_HASH);
        timer.Stop();

        // Assert
        Assert.True(calendarResponse.HasError == false);
        Assert.True(timer.Elapsed.TotalSeconds <= MAX_EXECUTION_TIME_IN_SECONDS);

    }


}

