namespace Peace.Lifelog.LifelogReminderTest;

using System.Diagnostics;
using Peace.Lifelog.DataAccess;
using Peace.Lifelog.Infrastructure;
using Peace.Lifelog.LifelogReminder;
using Peace.Lifelog.Logging;
using Peace.Lifelog.Security;
using Peace.Lifelog.UserManagement;

public class PinServiceShould : IAsyncLifetime, IDisposable
{
    private static CreateDataOnlyDAO createDataOnlyDAO = new CreateDataOnlyDAO();
    private static ReadDataOnlyDAO readDataOnlyDAO = new ReadDataOnlyDAO();
    private static UpdateDataOnlyDAO updateDataOnlyDAO = new UpdateDataOnlyDAO();
    private static DeleteDataOnlyDAO deleteDataOnlyDAO = new DeleteDataOnlyDAO();
    private static LogTarget logTarget = new LogTarget(createDataOnlyDAO, readDataOnlyDAO);
    private static Logging logging = new Logging(logTarget);
    private static ILifelogAuthService lifelogAuthService = new LifelogAuthService();
    private static ILifelogReminderRepo lifelogReminderRepo = new LifelogReminderRepo(createDataOnlyDAO, readDataOnlyDAO, updateDataOnlyDAO, deleteDataOnlyDAO);
    private LifelogReminderService lifelogReminderService = new LifelogReminderService(lifelogReminderRepo, lifelogAuthService, logging);
    private const string USER_ID = "devinkothari02@gmail.com";
    private string USER_HASH = "";
    private const string ROLE = "Normal";

    private string DOB = DateTime.Today.ToString("yyyy-MM-dd");
    private const string ZIP_CODE = "90701";
    //double check principal declratoin
    private AppPrincipal? PRINCIPAL = new AppPrincipal();
    private string Content = "Active";
    private string Frequency = "Weekly";
    private Stopwatch timer = new Stopwatch();

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
    public async Task SendEmail_ShouldSendEmailToUser()
    {
        //arrange
        ReminderFormData testForm = new ReminderFormData();
        testForm.UserHash = USER_HASH;

        //act
        //var createUserInDBResponse = await lifelogReminderRepo.AddUserHashAndDate(USER_HASH);
        var sendEmailResponse = await lifelogReminderService.SendReminderEmail(testForm);

        //
        Assert.True(sendEmailResponse.Output!.Count == 1);
    }
}