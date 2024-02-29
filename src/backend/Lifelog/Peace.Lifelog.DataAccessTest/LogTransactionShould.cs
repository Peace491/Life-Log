namespace Peace.Lifelog.DataAccessTest;

using DomainModels;
using Peace.Lifelog.DataAccess;
using Peace.Lifelog.UserManagement;
using Peace.Lifelog.UserManagementTest;
using System.Diagnostics;
using System.Threading.Tasks;

public class LogTransactionShould : IAsyncLifetime, IDisposable 
{
    private const int DEFAULT_RECORD_COUNT = 1;

    private const string TABLE = "logTransactionMockData";

    private LifelogAccountRequest LIFELOG_ACCOUNT_REQUEST = new LifelogAccountRequest();

    private const string USER_ID = "TestLLIServiceAccount";
    private string USER_HASH = "";
    private const string MFA_ID = "TestLLIServiceMFA";
    private const string ROLE = "Normal";

    private LifelogProfileRequest LIFELOG_PROFILE_REQUEST = new LifelogProfileRequest();

    private string DOB = DateTime.Today.ToString("yyyy-MM-dd");
    private string DEADLINE = DateTime.Today.ToString("yyyy-MM-dd");
    private const string ZIP_CODE = "92612";

    public async Task InitializeAsync()
    {
        var DDLTransactionDAO = new DDLTransactionDAO();

        var createMockTableSql = $"CREATE TABLE {TABLE} ("
            + "Id INT AUTO_INCREMENT,"
            + "Category VARCHAR(255),"
            + "MockData TEXT,"
            + "PRIMARY KEY (Id, Category)"
        + ");";

        await DDLTransactionDAO.ExecuteDDLCommand(createMockTableSql);

        // Create Test User Account
        
        var lifelogUserManagementService = new LifelogUserManagementService();

        LIFELOG_ACCOUNT_REQUEST.UserId = ("UserId", USER_ID);
        LIFELOG_ACCOUNT_REQUEST.Role = ("Role", ROLE);

        LIFELOG_PROFILE_REQUEST.DOB = ("DOB", DOB);
        LIFELOG_PROFILE_REQUEST.ZipCode = ("ZipCode", ZIP_CODE);


        var createAccountResponse = await lifelogUserManagementService.CreateLifelogUser(LIFELOG_ACCOUNT_REQUEST, LIFELOG_PROFILE_REQUEST);
        string test = "";
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }

    // Cleanup for all tests
    public async void Dispose()
    {
        var lifelogUserManagementService = new LifelogUserManagementService();
        var deleteAccountResponse = lifelogUserManagementService.DeleteLifelogUser(LIFELOG_ACCOUNT_REQUEST, LIFELOG_PROFILE_REQUEST);

        var DDLTransactionDAO = new DDLTransactionDAO();

        var deleteMockTableSql = $"DROP TABLE {TABLE}";

        await DDLTransactionDAO.ExecuteDDLCommand(deleteMockTableSql);
    }

    [Fact]
    public async void LogTransactionShould_LogCreateTransactionInDataStore()
    {
        // Arrange
        var timer = new Stopwatch();
        var createOnlyDAO = new CreateDataOnlyDAO();
        var readOnlyDAO = new ReadDataOnlyDAO();
        var deleteOnlyDAO = new DeleteDataOnlyDAO();

        var createCategory = "Create";
        var createMockData = "Mock Data";

        var insertSql =  $"INSERT INTO {TABLE} (Category, MockData) VALUES ('{createCategory}', '{createMockData}')";
        var deleteSql = $"DELETE FROM {TABLE} WHERE Category = '{createCategory}'";

        // Act
        var createResponse = await createOnlyDAO.CreateData(insertSql); // Need to test for all behavior of string

        var readDataSql = $"SELECT * FROM Logs WHERE Id={createResponse.LogId}";

        var readResponse = await readOnlyDAO.ReadData(readDataSql);

        // Assert
        Assert.True(readResponse.HasError == false);
        Assert.True(readResponse.Output.Count == 1);

        // Cleanup
        var deleteResponse = await deleteOnlyDAO.DeleteData(deleteSql);

        var logTransaction = new LogTransaction();
        await logTransaction.DeleteDataAccessTransactionLog(createResponse.LogId);
        await logTransaction.DeleteDataAccessTransactionLog(readResponse.LogId);
        await logTransaction.DeleteDataAccessTransactionLog(deleteResponse.LogId);
    }

    [Fact]
    public async void LogTransactionShould_LogReadTransactionInDataStore()
    {
        // Arrange
    
        var timer = new Stopwatch();
        var createOnlyDAO = new CreateDataOnlyDAO();
        var readOnlyDAO = new ReadDataOnlyDAO();
        var deleteOnlyDAO = new DeleteDataOnlyDAO();

        var readCategory = "Read Single";
        var readMockData = "Mock Data";

        var createSql =  $"INSERT INTO {TABLE} (Category, MockData) VALUES ('{readCategory}', '{readMockData}')";
        var readDataSql = $"SELECT MockData FROM {TABLE} WHERE Category = '{readCategory}'";
        var deleteSql = $"DELETE FROM {TABLE} WHERE Category = '{readCategory}'";

        // Act
        var createResponse = await createOnlyDAO.CreateData(createSql); // Need to test for all behavior of string

        var readDataResponse = await readOnlyDAO.ReadData(readDataSql, DEFAULT_RECORD_COUNT); // Issue might be because create Response is not finished

        var readLogSql = $"SELECT * FROM Logs WHERE Id={readDataResponse.LogId}";
        var readLogResponse = await readOnlyDAO.ReadData(readLogSql);
        
        // Assert
        Assert.True(readLogResponse.HasError == false);
        Assert.True(readLogResponse.Output.Count == DEFAULT_RECORD_COUNT);

        // Cleanup
        var deleteResponse = await deleteOnlyDAO.DeleteData(deleteSql);

        var logTransaction = new LogTransaction();
        await logTransaction.DeleteDataAccessTransactionLog(createResponse.LogId);
        await logTransaction.DeleteDataAccessTransactionLog(readDataResponse.LogId);
        await logTransaction.DeleteDataAccessTransactionLog(readLogResponse.LogId);
        await logTransaction.DeleteDataAccessTransactionLog(deleteResponse.LogId);
    }

    [Fact]
    public async void LogTransactionShould_LogUpdateTransactionInDataStore()
    {
        // Arrange
        var timer = new Stopwatch();
        var createOnlyDAO = new CreateDataOnlyDAO();
        var readOnlyDAO = new ReadDataOnlyDAO();
        var updateOnlyDAO = new UpdateDataOnlyDAO();
        var deleteOnlyDAO = new DeleteDataOnlyDAO();
        
        var updateCategory = "Update";
        var oldMockData = "Old Mock Data";
        var newMockData = "New Mock Data";
    
        var createSql =  $"INSERT INTO {TABLE} (Category, MockData) VALUES ('{updateCategory}', '{oldMockData}')";
        
        var updateSql = $"UPDATE {TABLE} SET MockData = '{newMockData}' WHERE Category = '{updateCategory}'";
        var deleteSql = $"DELETE FROM {TABLE} WHERE Category = '{updateCategory}'";
        
        // Act
        var createResponse = await createOnlyDAO.CreateData(createSql);

        var updateResponse = await updateOnlyDAO.UpdateData(updateSql);

        var readSql = $"SELECT * FROM Logs WHERE Id={updateResponse.LogId}";

        var readResponse = await readOnlyDAO.ReadData(readSql);

        // Assert
        Assert.True(readResponse.HasError == false);
        Assert.True(readResponse.Output.Count == DEFAULT_RECORD_COUNT);

        // Cleanup
        var deleteResponse = await deleteOnlyDAO.DeleteData(deleteSql);

        var logTransaction = new LogTransaction();
        await logTransaction.DeleteDataAccessTransactionLog(createResponse.LogId);
        await logTransaction.DeleteDataAccessTransactionLog(updateResponse.LogId);
        await logTransaction.DeleteDataAccessTransactionLog(readResponse.LogId);
        await logTransaction.DeleteDataAccessTransactionLog(deleteResponse.LogId);
    }

    [Fact]
    public async void LogTransactionShould_LogDeleteTransactionInDataStore()
    {
        // Arrange
        var timer = new Stopwatch();
        var createOnlyDAO = new CreateDataOnlyDAO();
        var readOnlyDAO = new ReadDataOnlyDAO();
        var deleteOnlyDAO = new DeleteDataOnlyDAO();

        var deleteCategory = "Delete";
        var deleteMockData = "Mock Data";

        var createSql = $"INSERT INTO {TABLE} (Category, MockData) VALUES ('{deleteCategory}', '{deleteMockData}')";
        
        var deleteSql = $"DELETE FROM {TABLE} WHERE Category = '{deleteCategory}'";

        // Act
        var createResponse = await createOnlyDAO.CreateData(createSql);

        var deleteResponse = await deleteOnlyDAO.DeleteData(deleteSql);
        
        var readSql = $"SELECT * FROM Logs WHERE Id={deleteResponse.LogId}";
        var readResponse = await readOnlyDAO.ReadData(readSql);

        // Assert
        Assert.True(readResponse.HasError == false);
        Assert.True(readResponse.Output.Count == DEFAULT_RECORD_COUNT);

        // Cleanup
        var logTransaction = new LogTransaction();
        await logTransaction.DeleteDataAccessTransactionLog(createResponse.LogId);
        await logTransaction.DeleteDataAccessTransactionLog(readResponse.LogId);
        await logTransaction.DeleteDataAccessTransactionLog(deleteResponse.LogId);
    }
}
