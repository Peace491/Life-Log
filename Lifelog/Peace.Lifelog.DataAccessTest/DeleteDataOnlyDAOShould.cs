namespace Peace.Lifelog.DataAccessTest;

using Peace.Lifelog.DataAccess;
using DomainModels;
using System.Diagnostics;

public class DeleteDataOnlyDAOShould
{
    private const int MAX_EXECUTION_TIME_IN_SECONDS = 3;
    private const int DEFAULT_NUMBER_OF_RECORDS = 20;

    [Fact]
    public async void DeleteDataOnlyDAOShould_ConnectToTheDataStore()
    {
        // Arrange
        var timer = new Stopwatch();
        var deleteOnlyDAO = new DeleteDataOnlyDAO();

        // Act
        timer.Start();
        var dbConnection = deleteOnlyDAO.ConnectToDb(); // Need to test for all behavior of string
        dbConnection.Open();
        timer.Stop();

        var connectionState = dbConnection.State;
        dbConnection.Close();

        // Assert
        Assert.True(connectionState == System.Data.ConnectionState.Open);
        Assert.True(timer.Elapsed.TotalSeconds <= MAX_EXECUTION_TIME_IN_SECONDS);
    }

    [Fact]
    public async void DeleteDataOnlyDAOShould_DeleteARecordInDataStore()
    {
        // Arrange
        var timer = new Stopwatch();
        var createOnlyDAO = new CreateDataOnlyDAO();
        var readOnlyDAO = new ReadDataOnlyDAO();
        var deleteOnlyDAO = new DeleteDataOnlyDAO();

        var table = "mockData";
        var deleteCategory = "Delete";
        var deleteMockData = "Mock Data";

        var createSql = $"INSERT INTO {table} (Category, MockData) VALUES ('{deleteCategory}', '{deleteMockData}')";
        var readSql = $"SELECT MockData FROM {table} WHERE Category = '{deleteCategory}'";
        var deleteSql = $"DELETE FROM {table} WHERE Category = '{deleteCategory}'";

        // Act
        var createResponse = await createOnlyDAO.CreateData(createSql);

        timer.Start();
        var deleteResponse = await deleteOnlyDAO.DeleteData(deleteSql);
        timer.Stop();
        var readResponse = await readOnlyDAO.ReadData(readSql);

        // Assert
        Assert.True(deleteResponse.HasError == false);
        Assert.True(timer.Elapsed.TotalSeconds <= MAX_EXECUTION_TIME_IN_SECONDS);
        Assert.True(readResponse.HasError == false);
        Assert.True(readResponse.Output == null);

        // Cleanup
        var logTransaction = new LogTransaction();
        await logTransaction.DeleteDataAccessTransactionLog(createResponse.LogId);
        await logTransaction.DeleteDataAccessTransactionLog(readResponse.LogId);
        await logTransaction.DeleteDataAccessTransactionLog(deleteResponse.LogId);
    }

    [Fact]
    public async void DeleteDataOnlyDAOShould_DeleteMultipleRecordsInDataStore()
    {
        // Arrange
        var timer = new Stopwatch();
        var createOnlyDAO = new CreateDataOnlyDAO();
        var readOnlyDAO = new ReadDataOnlyDAO();
        var deleteOnlyDAO = new DeleteDataOnlyDAO();

        var table = "mockData";
        var deleteCategory = "Delete";
        var deleteMockData = "Mock Data";

        var createSql = $"INSERT INTO {table} (Category, MockData) VALUES ('{deleteCategory}', '{deleteMockData}')";
        var readSql = $"SELECT MockData FROM {table} WHERE Category = '{deleteCategory}'";
        var deleteSql = $"DELETE FROM {table} WHERE Category = '{deleteCategory}'";

        // Act
        List<Response> createResponses = new List<Response>();
        for (int i = 0; i < DEFAULT_NUMBER_OF_RECORDS; i++)
        {
            var createResponse = await createOnlyDAO.CreateData(createSql);
            createResponses.Add(createResponse);
        }

        timer.Start();
        var deleteResponse = await deleteOnlyDAO.DeleteData(deleteSql);
        timer.Stop();
        var readResponse = await readOnlyDAO.ReadData(readSql);

        // Assert
        Assert.True(deleteResponse.HasError == false);
        Assert.True(timer.Elapsed.TotalSeconds <= MAX_EXECUTION_TIME_IN_SECONDS);
        Assert.True(readResponse.HasError == false);
        Assert.True(readResponse.Output == null);

        // Cleanup
        var logTransaction = new LogTransaction();
        foreach (Response createResponse in createResponses)
        {
            await logTransaction.DeleteDataAccessTransactionLog(createResponse.LogId);
        }
        await logTransaction.DeleteDataAccessTransactionLog(readResponse.LogId);
        await logTransaction.DeleteDataAccessTransactionLog(deleteResponse.LogId);
    }

    [Fact]
    public async void DeleteDataOnlyDAOShould_ThrowErrorOnIncorrectSQLInput()
    {
        // Arrange
        var timer = new Stopwatch();
        var deleteOnlyDAO = new DeleteDataOnlyDAO();

        var table = "mockData";
        var deleteCategory = "Delete";

        var deleteSql = $"DLETE FROM {table} WHERE Category = '{deleteCategory}'";

        // Act
        timer.Start();
        var deleteResponse = await deleteOnlyDAO.DeleteData(deleteSql);
        timer.Stop();

        // Assert
        Assert.True(deleteResponse.HasError == true);
        Assert.Contains("You have an error in your SQL syntax", deleteResponse.ErrorMessage);
        Assert.True(timer.Elapsed.TotalSeconds <= MAX_EXECUTION_TIME_IN_SECONDS);

        // Cleanup
        var logTransaction = new LogTransaction();
        await logTransaction.DeleteDataAccessTransactionLog(deleteResponse.LogId);
    }

    [Fact]
    public async void DeleteDataOnlyDAOShould_ThrowErrorOnEmptyInput()
    {
        // Arrange
        var timer = new Stopwatch();
        var deleteOnlyDAO = new DeleteDataOnlyDAO();

        var deleteSql = $"";

        // Act
        timer.Start();
        var deleteResponse = await deleteOnlyDAO.DeleteData(deleteSql);
        timer.Stop();

        // Assert
        Assert.True(deleteResponse.HasError == true);
        Assert.True(deleteResponse.ErrorMessage == "Empty Input");
        Assert.True(timer.Elapsed.TotalSeconds <= MAX_EXECUTION_TIME_IN_SECONDS);

        // Cleanup
        var logTransaction = new LogTransaction();
        await logTransaction.DeleteDataAccessTransactionLog(deleteResponse.LogId);
    }
}
