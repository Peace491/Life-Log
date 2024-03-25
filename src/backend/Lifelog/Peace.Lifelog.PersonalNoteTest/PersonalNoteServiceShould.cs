namespace Peace.Lifelog.PersonalNoteTest;
using Peace.Lifelog.DataAccess;
using Peace.Lifelog.Logging;
using Peace.Lifelog.PersonalNote;
using Peace.Lifelog.UserManagement;
using Peace.Lifelog.UserManagementTest;
using System.Diagnostics;

public class PersonalNoteServiceShould : IAsyncLifetime, IDisposable
{
    private static CreateDataOnlyDAO createDataOnlyDAO = new CreateDataOnlyDAO();
    private static ReadDataOnlyDAO readDataOnlyDAO = new ReadDataOnlyDAO();
    private static UpdateDataOnlyDAO updateDataOnlyDAO = new UpdateDataOnlyDAO();
    private static DeleteDataOnlyDAO deleteDataOnlyDAO = new DeleteDataOnlyDAO();
    private static LogTarget logTarget = new LogTarget(createDataOnlyDAO);
    private static Logging logging = new Logging(logTarget);

    private PersonalNoteService personalNoteService = new PersonalNoteService(createDataOnlyDAO, readDataOnlyDAO, updateDataOnlyDAO, deleteDataOnlyDAO, logging);

    private const string USER_ID = "TestPersonalNoteServiceAccount";
    private string USER_HASH = "";
    private const string ROLE = "Normal";

    private string DOB = DateTime.Today.ToString("yyyy-MM-dd");
    private string DATE = DateTime.Today.ToString("yyyy-MM-dd");
    private const string ZIP_CODE = "90704";

    public async Task InitializeAsync()
    {
        // TODO: Fix one Lifelog User is implemented

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
    public async void PersonalNoteServiceCreatePersonalNoteShould_CreateAnPersonalNoteInTheDatabase()
    {
        // Arrange
        var timer = new Stopwatch();
        string testPersonalNoteContent = "Test the personal note creation";

        var testPN = new PN();
        testPN.UserHash = USER_HASH;
        testPN.NoteContent = testPersonalNoteContent;
        testPN.NoteDate = DATE;

        // Act
        timer.Start();
        var createPersonalNoteResponse = await personalNoteService.CreatePersonalNote(USER_HASH, testPN);
        timer.Stop();
        var readDataOnlyDAO = new ReadDataOnlyDAO();
        var readPersonalNoteSql = $"SELECT UserHash FROM PersonalNote WHERE NoteContent=\"{testPersonalNoteContent}\"";
        var readResponse = await readDataOnlyDAO.ReadData(readPersonalNoteSql);

        // Assert
        Assert.True(createPersonalNoteResponse.HasError == false);
        Assert.True(timer.Elapsed.TotalSeconds <= 3);
        Assert.NotNull(readResponse.Output);
        Assert.True(readResponse.Output.Count == 1);


        // Cleanup
        var deleteDataOnlyDAO = new DeleteDataOnlyDAO();

        var deletePersonalNoteSql = $"DELETE FROM PersonalNote WHERE NoteDate=\"{DATE}\";";

        await deleteDataOnlyDAO.DeleteData(deletePersonalNoteSql);

    }
}