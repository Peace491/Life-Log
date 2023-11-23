namespace Peace.Lifelog.DataAccessTest;

using Peace.Lifelog.DataAccess;

using System.Diagnostics;

public class LogTransactionShould
{
    private const int DEFAULT_RECORD_COUNT = 1;

    [Fact]
    public async void LogTransactionShould_LogCreateTransactionInDataStore()
    {
        // Arrange
        var timer = new Stopwatch();
        var createOnlyDAO = new CreateDataOnlyDAO();
        var readOnlyDAO = new ReadDataOnlyDAO();
        var deleteOnlyDAO = new DeleteDataOnlyDAO();

        var table = "mockData";
        var createCategory = "Create";
        var createMockData = "Mock Data";

        var insertSql =  $"INSERT INTO {table} (Category, MockData) VALUES ('{createCategory}', '{createMockData}')";
        var deleteSql = $"DELETE FROM {table} WHERE Category = '{createCategory}'";

        // Act
        var createResponse = await createOnlyDAO.CreateData(insertSql); // Need to test for all behavior of string

        var readDataSql = $"SELECT * FROM Logs WHERE LogID={createResponse.LogId}";

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

        var table = "mockData";
        var readCategory = "Read Single";
        var readMockData = "Mock Data";

        var createSql =  $"INSERT INTO {table} (Category, MockData) VALUES ('{readCategory}', '{readMockData}')";
        var readDataSql = $"SELECT MockData FROM {table} WHERE Category = '{readCategory}'";
        var deleteSql = $"DELETE FROM {table} WHERE Category = '{readCategory}'";

        // Act
        var createResponse = await createOnlyDAO.CreateData(createSql); // Need to test for all behavior of string

        var readDataResponse = await readOnlyDAO.ReadData(readDataSql, DEFAULT_RECORD_COUNT); // Issue might be because create Response is not finished

        var readLogSql = $"SELECT * FROM Logs WHERE LogID={readDataResponse.LogId}";
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
        
        var table = "mockData";
        var updateCategory = "Update";
        var oldMockData = "Old Mock Data";
        var newMockData = "New Mock Data";
    
        var createSql =  $"INSERT INTO {table} (Category, MockData) VALUES ('{updateCategory}', '{oldMockData}')";
        
        var updateSql = $"UPDATE {table} SET MockData = '{newMockData}' WHERE Category = '{updateCategory}'";
        var deleteSql = $"DELETE FROM {table} WHERE Category = '{updateCategory}'";
        
        // Act
        var createResponse = await createOnlyDAO.CreateData(createSql);

        var updateResponse = await updateOnlyDAO.UpdateData(updateSql);

        var readSql = $"SELECT * FROM Logs WHERE LogID={updateResponse.LogId}";

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

        var table = "mockData";
        var deleteCategory = "Delete";
        var deleteMockData = "Mock Data";

        var createSql = $"INSERT INTO {table} (Category, MockData) VALUES ('{deleteCategory}', '{deleteMockData}')";
        
        var deleteSql = $"DELETE FROM {table} WHERE Category = '{deleteCategory}'";

        // Act
        var createResponse = await createOnlyDAO.CreateData(createSql);

        var deleteResponse = await deleteOnlyDAO.DeleteData(deleteSql);
        
        var readSql = $"SELECT * FROM Logs WHERE LogID={deleteResponse.LogId}";
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
