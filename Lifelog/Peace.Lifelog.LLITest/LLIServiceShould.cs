namespace Peace.Lifelog.LLITest;

using System.Collections;
using System.Runtime.Serialization;
using DomainModels;
using Peace.Lifelog.DataAccess;
using Peace.Lifelog.LLI;
using Peace.Lifelog.Security;
using Peace.Lifelog.UserManagement;
using Peace.Lifelog.UserManagementTest;

public class LLIServiceShould : IDisposable
{
    private const string USER_ID = "TestLLIServiceAccount";
    private string USER_HASH = "";
    private string CREATION_DATE = DateTime.Today.ToString("yyyy-MM-dd");

    private string DOB = DateTime.Today.ToString("yyyy-MM-dd");
    private string DEADLINE = DateTime.Today.ToString("yyyy-MM-dd");
    private const string SALT = "skfmdoef";
    private const string MFA_ID = "TestLLIServiceMFA";
    private const string ROLE = "Normal";
    private const string ZIP_CODE = "92612";

    

    public LLIServiceShould()
    {
        // TODO: Fix one Lifelog User is implemented

        // Create Lifelog User Hash
        var hashService = new HashService();


        var hashResponse = hashService.Hasher(USER_ID + "skfmdoef");

        if (hashResponse.Output is not null)
        {
            foreach (String hashOutput in hashResponse.Output)
        {
            USER_HASH = hashOutput;
        }

        }
        
        var appUserManagementService = new AppUserManagementService();

        var testLifelogAccountRequest = new LifelogAccountRequest();

        testLifelogAccountRequest.UserId = ("UserId", USER_ID);
        testLifelogAccountRequest.UserHash = ("UserHash", USER_HASH);
        testLifelogAccountRequest.CreationDate = ("CreationDate", CREATION_DATE);
        testLifelogAccountRequest.Salt = ("Salt", SALT);
        testLifelogAccountRequest.MfaId = ("MfaId", MFA_ID);
        testLifelogAccountRequest.Role = ("Role", ROLE);

        var createAccountResponse = appUserManagementService.CreateAccount(testLifelogAccountRequest);

        var createDataOnlyDAO = new CreateDataOnlyDAO();

        

        var createUserHashSql = $"INSERT INTO LifelogUserHash (UserId, UserHash) VALUES (\"{USER_ID}\", \"{USER_HASH}\");";
        Task<Response> createUserHashResponse = createDataOnlyDAO.CreateData(createUserHashSql);

        // Create Lifelog User Profile
        var createLifelogProfileSql = $"INSERT INTO LifelogProfile VALUES (\"{USER_HASH}\", \"{DOB}\", \"{ZIP_CODE}\");";
        var createLifelogProfileResponse = createDataOnlyDAO.CreateData(createLifelogProfileSql);

    }

    public void Dispose()
    {
        var deleteDataOnlyDAO = new DeleteDataOnlyDAO();

        var deleteLifelogProfileSql = $"DELETE FROM LifelogProfile WHERE UserHash = \"{USER_HASH}\";";
        var deleteUserHashSql = $"DELETE FROM LifelogUserHash WHERE UserId = \"{USER_ID}\"";

        var deleteProfileResponse = deleteDataOnlyDAO.DeleteData(deleteLifelogProfileSql);
        var deleteUserHashResponse = deleteDataOnlyDAO.DeleteData(deleteUserHashSql);


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
}