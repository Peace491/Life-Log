namespace Peace.Lifelog.LifetreeServiceShould;

using Peace.Lifelog.DataAccess;
using Peace.Lifelog.LifetreeService;
using Peace.Lifelog.Logging;
using Peace.Lifelog.LLI;
using Peace.Lifelog.PersonalNote;
using Peace.Lifelog.UserManagement;
using Peace.Lifelog.Infrastructure;


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


        [Fact]
        public void LifetreeServiceShould_GiveAllCompletedLLI()
        {
        throw new NotImplementedException();
        }

        [Fact]
        public void LifetreeServiceShould_GiveAPersonalNote()
        {
        throw new NotImplementedException();
        }

    
        [Fact]
        public void LifetreeServiceShould_CreateAPersonalNote()
        {
        throw new NotImplementedException();
        }
}
