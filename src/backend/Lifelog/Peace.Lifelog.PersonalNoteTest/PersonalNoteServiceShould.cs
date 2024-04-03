namespace Peace.Lifelog.PersonalNoteTest;
using Peace.Lifelog.DataAccess;
using Peace.Lifelog.Infrastructure;
using Peace.Lifelog.Logging;
using Peace.Lifelog.PersonalNote;
using Peace.Lifelog.UserManagement;
using System.Diagnostics;

public class PersonalNoteServiceShould : IAsyncLifetime, IDisposable
{
    private static CreateDataOnlyDAO createDataOnlyDAO = new CreateDataOnlyDAO();
    private static ReadDataOnlyDAO readDataOnlyDAO = new ReadDataOnlyDAO();
    private static UpdateDataOnlyDAO updateDataOnlyDAO = new UpdateDataOnlyDAO();
    private static DeleteDataOnlyDAO deleteDataOnlyDAO = new DeleteDataOnlyDAO();
    private static LogTarget logTarget = new LogTarget(createDataOnlyDAO);
    private static Logging logging = new Logging(logTarget);
    private static IPersonalNoteRepo personalNoteRepo = new PersonalNoteRepo(createDataOnlyDAO, readDataOnlyDAO, updateDataOnlyDAO, deleteDataOnlyDAO);
    private PersonalNoteService personalNoteService = new PersonalNoteService(personalNoteRepo, logging);

    private const string USER_ID = "TestPersonalNoteServiceAccount";
    private string USER_HASH = "";
    private const string ROLE = "Normal";

    private string DOB = DateTime.Today.ToString("yyyy-MM-dd");
    private string DATE = DateTime.Today.ToString("yyyy-MM-dd");
    private const string ZIP_CODE = "90704";

    private Stopwatch timer = new Stopwatch();

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

    #region Create Note Tests
    [Fact]
    public async Task PersonalNoteServiceCreatePersonalNoteShould_CreateAnPersonalNoteInTheDatabase()
    {
        // Arrange

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

    [Fact]
    public async Task PersonalNoteServiceCreatePersonalNoteShouldShould_ThrowAnErrorIfTheUserHashIsEmpty()
    {
        // Arrange

        string testPersonalNoteContent = "Test the personal note creation";
        string invalidUserHash = "";

        var testPN = new PN();
        testPN.UserHash = invalidUserHash;
        testPN.NoteContent = testPersonalNoteContent;
        testPN.NoteDate = DATE;

        // Act
        timer.Start();
        var createPersonalNoteResponse = await personalNoteService.CreatePersonalNote(invalidUserHash, testPN);
        timer.Stop();
        var readDataOnlyDAO = new ReadDataOnlyDAO();
        var readPersonalNoteSql = $"SELECT UserHash FROM PersonalNote WHERE NoteContent=\"{testPersonalNoteContent}\"";
        var readResponse = await readDataOnlyDAO.ReadData(readPersonalNoteSql);

        // Assert
        Assert.True(createPersonalNoteResponse.ErrorMessage == "User Hash must not be empty");
        Assert.True(createPersonalNoteResponse.HasError == true);
        Assert.Null(readResponse.Output);

    }

    [Fact]
    public async Task PersonalNoteServiceCreatePersonalNoteShouldShould_ThrowAnErrorIfTheDateIsInvalid()
    {
        // Arrange

        string testPersonalNoteContent = "Test the personal note creation";

        var testPN = new PN();
        testPN.UserHash = USER_HASH;
        testPN.NoteContent = testPersonalNoteContent;
        testPN.NoteDate = DateTime.Now.Date.AddDays(1).ToString(); // Invaid day: A date in the future

        // Act
        timer.Start();
        var createPersonalNoteResponse = await personalNoteService.CreatePersonalNote(USER_HASH, testPN);
        timer.Stop();
        var readDataOnlyDAO = new ReadDataOnlyDAO();
        var readPersonalNoteSql = $"SELECT UserHash FROM PersonalNote WHERE NoteContent=\"{testPersonalNoteContent}\"";
        var readResponse = await readDataOnlyDAO.ReadData(readPersonalNoteSql);

        // Assert
        Assert.True(createPersonalNoteResponse.ErrorMessage == "The Date is Invalid");
        Assert.True(createPersonalNoteResponse.HasError == true);
        Assert.Null(readResponse.Output);

    }

    [Fact]
    public async Task PersonalNoteServiceCreatePersonalNoteShouldShould_ThrowAnErrorIfNoteContentIsTooLong()
    {
        var testPersonalNoteContent = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum. Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.";
        // Arrange
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
        Assert.True(createPersonalNoteResponse.HasError == true);
        Assert.True(createPersonalNoteResponse.ErrorMessage == "The Personal Note content is too long");
        Assert.Null(readResponse.Output);

    }

    [Fact]
    public async Task PersonalNoteServiceCreatePersonalNoteShouldShould_ThrowAnErrorIfNoteContentContainsNonAlphanumericvalues()
    {
        var testPersonalNoteContent = "Lorem ipsum dolor sit amet, @ consectetur adipiscing elit.";
        // Arrange
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
        Assert.True(createPersonalNoteResponse.HasError == true);
        Assert.True(createPersonalNoteResponse.ErrorMessage == "The personal note content has invalid nonalphanumeric characters");
        Assert.Null(readResponse.Output);

    }


    #endregion

    #region Delete Note Tests
    [Fact]
    public async Task PersonalNoteServiceDeletePersonalNoteShould_DeleteAnPersonalNoteInTheDatabase()
    {
        // Arrange

        string testPersonalNoteContent = "Test the personal note deletion";

        var testPN = new PN();
        testPN.UserHash = USER_HASH;
        testPN.NoteContent = testPersonalNoteContent;
        testPN.NoteDate = DATE;

        // Act
        var createPersonalNoteResponse = await personalNoteService.CreatePersonalNote(USER_HASH, testPN);
        var readResponse = await personalNoteService.ViewPersonalNote(USER_HASH, testPN);

        if (readResponse.Output != null)
        {
            foreach (PN personalNote in readResponse.Output)
            {
                testPN = personalNote;
            }
        }
        timer.Start();
        var deletePersonalNoteResponse = await personalNoteService.DeletePersonalNote(USER_HASH, testPN.NoteId);
        timer.Stop();
        var readDataOnlyDAO = new ReadDataOnlyDAO();
        var readPersonalNoteSql = $"SELECT UserHash FROM PersonalNote WHERE NoteContent=\"{testPersonalNoteContent}\"";
        readResponse = await readDataOnlyDAO.ReadData(readPersonalNoteSql);

        // Assert
        Assert.True(createPersonalNoteResponse.HasError == false);
        Assert.True(deletePersonalNoteResponse.HasError == false);
        Assert.True(timer.Elapsed.TotalSeconds <= 3);
        Assert.True(readResponse.Output == null);

    }

    [Fact]
    public async Task PersonalNoteServiceDeletePersonalNoteShould_ThrowAnErrorIfTheUserHashIsEmpty()
    {
        // Arrange

        string testPersonalNoteContent = "Test the personal note deletion";
        string invalidUserHash = "";

        var testPN = new PN();
        testPN.UserHash = invalidUserHash;
        testPN.NoteContent = testPersonalNoteContent;
        testPN.NoteDate = DATE;

        // Act
        timer.Start();
        var deletePersonalNoteResponse = await personalNoteService.DeletePersonalNote(invalidUserHash, testPN.NoteId);
        timer.Stop();
        var readDataOnlyDAO = new ReadDataOnlyDAO();
        var readPersonalNoteSql = $"SELECT UserHash FROM PersonalNote WHERE NoteContent=\"{testPersonalNoteContent}\"";
        var readResponse = await readDataOnlyDAO.ReadData(readPersonalNoteSql);

        // Assert
        Assert.True(deletePersonalNoteResponse.HasError == true);
        Assert.True(deletePersonalNoteResponse.ErrorMessage == "User Hash must not be empty");
        Assert.Null(readResponse.Output);

    }

    #endregion

    #region View Note Tests
    [Fact]
    public async Task PersonalNoteServiceViewPersonalNoteShould_GetAPersonalNoteForAnUser()
    {
        // Arrange

        string testPersonalNoteContent = "Test the personal note viewing method";

        var testPN = new PN();
        testPN.UserHash = USER_HASH;
        testPN.NoteContent = testPersonalNoteContent;
        testPN.NoteDate = DATE;

        // Act
        var createPersonalNoteResponse = await personalNoteService.CreatePersonalNote(USER_HASH, testPN);
        timer.Start();
        var viewPersonalNoteResponse = await personalNoteService.ViewPersonalNote(USER_HASH, testPN);
        timer.Stop();


        // Assert
        Assert.True(createPersonalNoteResponse.HasError == false);
        Assert.True(viewPersonalNoteResponse.HasError == false);
        Assert.NotNull(viewPersonalNoteResponse.Output);
        Assert.True(viewPersonalNoteResponse.Output.Count == 1);
        PN firstObject = (PN)viewPersonalNoteResponse.Output.First();
        Assert.True(firstObject.NoteContent == testPN.NoteContent);
        Assert.True(timer.Elapsed.TotalSeconds <= 3);



        // Cleanup
        var deleteDataOnlyDAO = new DeleteDataOnlyDAO();

        var deletePersonalNoteSql = $"DELETE FROM PersonalNote WHERE NoteDate=\"{DATE}\";";

        await deleteDataOnlyDAO.DeleteData(deletePersonalNoteSql);

    }

    [Fact]
    public async Task PersonalNoteServiceViewPersonalNoteShould_ThrowAnErrorIfTheUserHashIsEmpty()
    {
        // Arrange

        string testPersonalNoteContent = "Test the personal note viewing method";
        string invalidUserHash = "";

        var testPN = new PN();
        testPN.UserHash = invalidUserHash;
        testPN.NoteContent = testPersonalNoteContent;
        testPN.NoteDate = DATE;

        // Act
        timer.Start();
        var viewPersonalNoteResponse = await personalNoteService.ViewPersonalNote(invalidUserHash, testPN);
        timer.Stop();


        // Assert
        Assert.True(viewPersonalNoteResponse.HasError == true);
        Assert.True(viewPersonalNoteResponse.ErrorMessage == "User Hash must not be empty");

    }

    #endregion

    #region Edit/update Note Tests
    [Fact]
    public async Task PersonalNoteServiceUpdatePersonalNoteShould_UpdateANoteInTheDatabase()
    {
        // Arrange
        string testOldPersonalNoteContent = "Test the personal note update";
        string testNewPersonalNoteContent = "Updated";

        // Old Note        
        var testOldPN = new PN();
        testOldPN.UserHash = USER_HASH;
        testOldPN.NoteContent = testOldPersonalNoteContent;
        testOldPN.NoteDate = DATE;

        // New Note
        var testNewPN = new PN();
        testNewPN.UserHash = USER_HASH;
        testNewPN.NoteContent = testNewPersonalNoteContent;
        testNewPN.NoteDate = DATE;

        //updated Note
        var updatedPN = new PN();

        var createResponse = await personalNoteService.CreatePersonalNote(USER_HASH, testOldPN);

        var readResponse = await personalNoteService.ViewPersonalNote(USER_HASH, testOldPN);

        if (readResponse.Output != null)
        {
            foreach (PN personalNote in readResponse.Output)
            {
                testNewPN.NoteId = personalNote.NoteId;
            }
        }


        // Act

        timer.Start();
        var updateResponse = await personalNoteService.UpdatePersonalNote(USER_HASH, testNewPN);
        timer.Stop();

        readResponse = await personalNoteService.ViewPersonalNote(USER_HASH, testNewPN);
        if (readResponse.Output != null)
        {
            foreach (PN personalNote in readResponse.Output)
            {
                updatedPN = personalNote;
            }
        }
        // Assert
        Assert.True(readResponse.Output != null);
        Assert.True(updatedPN.NoteContent == testNewPersonalNoteContent);
        Assert.True(DateTime.Parse(updatedPN.NoteDate) == DateTime.Parse(testNewPN.NoteDate));
        Assert.True(timer.Elapsed.TotalSeconds < 3);

        // Cleanup
        var deleteDataOnlyDAO = new DeleteDataOnlyDAO();

        var deletePersonalNoteSql = $"DELETE FROM PersonalNote WHERE NoteId=\"{updatedPN.NoteId}\";";

        await deleteDataOnlyDAO.DeleteData(deletePersonalNoteSql);
    }

    [Fact]
    public async Task PersonalNoteServiceUpdatePersonalNoteShould_ThrowAnErrorIfTheUserHashIsEmpty()
    {
        // Arrange
        string testOldPersonalNoteContent = "Test the personal note update";
        string testNewPersonalNoteContent = "Updated";
        string invalidUserHash = "";

        // Old Note        
        var testOldPN = new PN();
        testOldPN.UserHash = USER_HASH;
        testOldPN.NoteContent = testOldPersonalNoteContent;
        testOldPN.NoteDate = DATE;

        // New Note
        var testNewPN = new PN();
        testNewPN.UserHash = invalidUserHash;
        testNewPN.NoteContent = testNewPersonalNoteContent;
        testNewPN.NoteDate = DATE;

        //updated Note
        var updatedPN = new PN();

        var createResponse = await personalNoteService.CreatePersonalNote(USER_HASH, testOldPN);

        var readResponse = await personalNoteService.ViewPersonalNote(USER_HASH, testOldPN);

        if (readResponse.Output != null)
        {
            foreach (PN personalNote in readResponse.Output)
            {
                testNewPN.NoteId = personalNote.NoteId;
            }
        }


        // Act

        timer.Start();
        var updateResponse = await personalNoteService.UpdatePersonalNote(invalidUserHash, testNewPN);
        timer.Stop();

        // Assert
        Assert.True(updateResponse.HasError == true);
        Assert.True(updateResponse.ErrorMessage == "User Hash must not be empty");

        // Cleanup
        var deleteDataOnlyDAO = new DeleteDataOnlyDAO();

        var deletePersonalNoteSql = $"DELETE FROM PersonalNote WHERE NoteId=\"{updatedPN.NoteId}\";";

        await deleteDataOnlyDAO.DeleteData(deletePersonalNoteSql);
    }


    [Fact]
    public async Task PersonalNoteServiceUpdatePersonalNoteShould_ThrowAnErrorIfTheDateIsInvalid()
    {
        // Arrange
        string testOldPersonalNoteContent = "Test the personal note update";
        string testNewPersonalNoteContent = "Updated";

        // Old Note        
        var testOldPN = new PN();
        testOldPN.UserHash = USER_HASH;
        testOldPN.NoteContent = testOldPersonalNoteContent;
        testOldPN.NoteDate = DATE;

        // New Note
        var testNewPN = new PN();
        testNewPN.UserHash = USER_HASH;
        testNewPN.NoteContent = testNewPersonalNoteContent;
        testNewPN.NoteDate = DateTime.Now.Date.AddDays(1).ToString();

        //updated Note
        var updatedPN = new PN();

        var createResponse = await personalNoteService.CreatePersonalNote(USER_HASH, testOldPN);

        var readResponse = await personalNoteService.ViewPersonalNote(USER_HASH, testOldPN);

        if (readResponse.Output != null)
        {
            foreach (PN personalNote in readResponse.Output)
            {
                testNewPN.NoteId = personalNote.NoteId;
            }
        }


        // Act

        timer.Start();
        var updateResponse = await personalNoteService.UpdatePersonalNote(USER_HASH, testNewPN);
        timer.Stop();

        // Assert
        Assert.True(readResponse.Output != null);
        Assert.True(updateResponse.HasError == true);
        Assert.True(updateResponse.ErrorMessage == "The Date is Invalid");
        Assert.True(timer.Elapsed.TotalSeconds < 3);

        // Cleanup
        var deleteDataOnlyDAO = new DeleteDataOnlyDAO();

        var deletePersonalNoteSql = $"DELETE FROM PersonalNote WHERE NoteId=\"{testNewPN.NoteId}\";";

        await deleteDataOnlyDAO.DeleteData(deletePersonalNoteSql);
    }

    [Fact]
    public async Task PersonalNoteServiceUpdatePersonalNoteShould_ThrowAnErrorIfNoteContentIsTooLong()
    {
        // Arrange
        string testOldPersonalNoteContent = "Test the personal note update";
        var testNewPersonalNoteContent = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum. Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.";

        // Old Note        
        var testOldPN = new PN();
        testOldPN.UserHash = USER_HASH;
        testOldPN.NoteContent = testOldPersonalNoteContent;
        testOldPN.NoteDate = DATE;

        // New Note
        var testNewPN = new PN();
        testNewPN.UserHash = USER_HASH;
        testNewPN.NoteContent = testNewPersonalNoteContent;
        testNewPN.NoteDate = DATE;

        //updated Note
        var updatedPN = new PN();

        var createResponse = await personalNoteService.CreatePersonalNote(USER_HASH, testOldPN);

        var readResponse = await personalNoteService.ViewPersonalNote(USER_HASH, testOldPN);

        if (readResponse.Output != null)
        {
            foreach (PN personalNote in readResponse.Output)
            {
                testNewPN.NoteId = personalNote.NoteId;
            }
        }


        // Act

        timer.Start();
        var updateResponse = await personalNoteService.UpdatePersonalNote(USER_HASH, testNewPN);
        timer.Stop();

        // Assert
        Assert.True(readResponse.Output != null);
        Assert.True(updateResponse.HasError == true);
        Assert.True(updateResponse.ErrorMessage == "The Personal Note content is too long");
        Assert.True(timer.Elapsed.TotalSeconds < 3);

        // Cleanup
        var deleteDataOnlyDAO = new DeleteDataOnlyDAO();

        var deletePersonalNoteSql = $"DELETE FROM PersonalNote WHERE NoteId=\"{testNewPN.NoteId}\";";

        await deleteDataOnlyDAO.DeleteData(deletePersonalNoteSql);

    }

    [Fact]
    public async Task PersonalNoteServiceUpdatePersonalNoteShould_ThrowAnErrorIfNoteContentContainsNonAlphanumericvalues()
    {
        // Arrange
        string testOldPersonalNoteContent = "Test the personal note update";
        var testNewPersonalNoteContent = "Lorem ipsum dolor sit amet, @ consectetur adipiscing elit.";

        // Old Note        
        var testOldPN = new PN();
        testOldPN.UserHash = USER_HASH;
        testOldPN.NoteContent = testOldPersonalNoteContent;
        testOldPN.NoteDate = DATE;

        // New Note
        var testNewPN = new PN();
        testNewPN.UserHash = USER_HASH;
        testNewPN.NoteContent = testNewPersonalNoteContent;
        testNewPN.NoteDate = DATE;

        //updated Note
        var updatedPN = new PN();

        var createResponse = await personalNoteService.CreatePersonalNote(USER_HASH, testOldPN);

        var readResponse = await personalNoteService.ViewPersonalNote(USER_HASH, testOldPN);

        if (readResponse.Output != null)
        {
            foreach (PN personalNote in readResponse.Output)
            {
                testNewPN.NoteId = personalNote.NoteId;
            }
        }


        // Act

        timer.Start();
        var updateResponse = await personalNoteService.UpdatePersonalNote(USER_HASH, testNewPN);
        timer.Stop();

        // Assert
        Assert.True(readResponse.Output != null);
        Assert.True(updateResponse.HasError == true);
        Assert.True(updateResponse.ErrorMessage == "The personal note content has invalid nonalphanumeric characters");
        Assert.True(timer.Elapsed.TotalSeconds < 3);

        // Cleanup
        var deleteDataOnlyDAO = new DeleteDataOnlyDAO();

        var deletePersonalNoteSql = $"DELETE FROM PersonalNote WHERE NoteId=\"{testNewPN.NoteId}\";";

        await deleteDataOnlyDAO.DeleteData(deletePersonalNoteSql);

    }
    #endregion

}