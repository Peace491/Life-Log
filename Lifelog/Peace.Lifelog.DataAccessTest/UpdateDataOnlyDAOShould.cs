namespace Peace.Lifelog.DataAccessTest;

using Peace.Lifelog.DataAccess;

using DomainModels;

using System.Diagnostics;

public class UpdateDataOnlyDAOShould
{
    [Fact]
    public async void UpdateDataOnlyDAOShould_ConnectToTheDataStore()
    {
        // Arrange
        var timer = new Stopwatch();
        var updateOnlyDAO = new UpdateDataOnlyDAO();

        // Act
        timer.Start();
        var dbConnection = updateOnlyDAO.ConnectToDb(); // Need to test for all behavior of string
        dbConnection.Open();
        timer.Stop();

        var connectionState = dbConnection.State;
        dbConnection.Close();

        // Assert
        Assert.True(connectionState == System.Data.ConnectionState.Open);
        Assert.True(timer.Elapsed.TotalSeconds <= 3);
    }

    [Fact]
    public async void UpdateDataOnlyDAOShould_UpdateARecordInDataStore()
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
        var readSql = $"SELECT {table} FROM {table} WHERE Category = '{updateCategory}'";
        var updateSql = $"UPDATE {table} SET MockData = '{newMockData}' WHERE Category = '{updateCategory}'";
        var deleteSql = $"DELETE FROM {table} WHERE Category = '{updateCategory}'";
        
        
        // Act
        var createResponse = await createOnlyDAO.CreateData(createSql);

        var originalReadResponse = await readOnlyDAO.ReadData(readSql);

        timer.Start();
        var updateResponse = await updateOnlyDAO.UpdateData(updateSql);
        timer.Stop();

        var newReadResponse = await readOnlyDAO.ReadData(readSql);

        // Assert
        Assert.True(updateResponse.HasError == false);
        Assert.True(originalReadResponse.Output != newReadResponse.Output);
        foreach (List<Object> newReadResponseData in newReadResponse.Output)
        {
            Assert.True(newReadResponseData[0].ToString() == newMockData);

        }
        Assert.True(timer.Elapsed.TotalSeconds <= 3);

        // Cleanup
        var deleteResponse = await deleteOnlyDAO.DeleteData(deleteSql);

        var logTransaction = new LogTransaction();
        logTransaction.DeleteDataAccessTransactionLog(createResponse.LogId);
        logTransaction.DeleteDataAccessTransactionLog(updateResponse.LogId);
        logTransaction.DeleteDataAccessTransactionLog(originalReadResponse.LogId);
        logTransaction.DeleteDataAccessTransactionLog(newReadResponse.LogId);
        logTransaction.DeleteDataAccessTransactionLog(deleteResponse.LogId);
    }

    [Fact]
    public async void UpdateDataOnlyDAOShould_UpdateMultipleRecordsInDataStore()
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
        var numberOfRecords = 20;
    
        var createSql =  $"INSERT INTO {table} (Category, MockData) VALUES ('{updateCategory}', '{oldMockData}')";
        var readSql = $"SELECT {table} FROM {table} WHERE Category = '{updateCategory}'";
        var updateSql = $"UPDATE {table} SET MockData = '{newMockData}' WHERE Category = '{updateCategory}'";
        var deleteSql = $"DELETE FROM {table} WHERE Category = '{updateCategory}'";
        
        // Act
        List<Response> createResponses = new List<Response>();
        for (int i = 0; i < numberOfRecords; i++)
        {
            var createResponse = await createOnlyDAO.CreateData(createSql); 
            createResponses.Add(createResponse);
        }

        var originalReadResponse = await readOnlyDAO.ReadData(readSql);

        timer.Start();
        var updateResponse = await updateOnlyDAO.UpdateData(updateSql);
        timer.Stop();

        var newReadResponse = await readOnlyDAO.ReadData(readSql);

        // Assert
        Assert.True(updateResponse.HasError == false);
        Assert.True(originalReadResponse.Output != newReadResponse.Output);
        foreach (List<Object> newReadResponseData in newReadResponse.Output)
        {
            Assert.True(newReadResponseData[0].ToString() == newMockData);

        }
        Assert.True(timer.Elapsed.TotalSeconds <= 3);

        // Cleanup
        var deleteResponse = await deleteOnlyDAO.DeleteData(deleteSql);

        var logTransaction = new LogTransaction();
        foreach (Response createResponse in createResponses)
        {
            logTransaction.DeleteDataAccessTransactionLog(createResponse.LogId);
        }
        logTransaction.DeleteDataAccessTransactionLog(updateResponse.LogId);
        logTransaction.DeleteDataAccessTransactionLog(originalReadResponse.LogId);
        logTransaction.DeleteDataAccessTransactionLog(newReadResponse.LogId);
        logTransaction.DeleteDataAccessTransactionLog(deleteResponse.LogId);
    }

    [Fact]
    public async void UpdateDataOnlyDAOShould_ReturnNullIfNoRecordIsFoundInDataStore()
    {
        // Arrange
        var timer = new Stopwatch();
        var updateOnlyDAO = new UpdateDataOnlyDAO();
        
        var table = "mockData";
        var updateMockData = "Mock Data";
    
        var updateSql = $"UPDATE {table} SET MockData = '{updateMockData}'";
        
        // Act
        timer.Start();
        var updateResponse = await updateOnlyDAO.UpdateData(updateSql);
        timer.Stop();

        // Assert
        Assert.True(updateResponse.HasError == false);
        Assert.True(timer.Elapsed.TotalSeconds <= 3);

        // Cleanup
        var logTransaction = new LogTransaction();
        logTransaction.DeleteDataAccessTransactionLog(updateResponse.LogId);
    }

    [Fact]
    public async void UpdateDataOnlyDAOShould_ThrowErrorOnIncorrectSQLInput()
    {
        // Arrange
        var timer = new Stopwatch();
        var updateOnlyDAO = new UpdateDataOnlyDAO();
        
        var table = "mockData";
        var updateMockData = "Mock Data";
    
        var incorrectUpdateSql = $"UDATE {table} SET MockData = '{updateMockData}'";
        
        // Act
        timer.Start();
        var updateResponse = await updateOnlyDAO.UpdateData(incorrectUpdateSql);
        timer.Stop();

        // Assert
        Assert.True(updateResponse.HasError == true);
        Assert.Contains("You have an error in your SQL syntax", updateResponse.ErrorMessage);
        Assert.True(timer.Elapsed.TotalSeconds <= 3);

        // Cleanup
        var logTransaction = new LogTransaction();
        logTransaction.DeleteDataAccessTransactionLog(updateResponse.LogId);
    }

    [Fact]
    public async void UpdateDataOnlyDAOShould_ThrowErrorOnEmptyInput()
    {
        // Arrange
        var timer = new Stopwatch();
        var updateOnlyDAO = new UpdateDataOnlyDAO();
    
        var incorrectUpdateSql = $"";
        
        // Act
        timer.Start();
        var updateResponse = await updateOnlyDAO.UpdateData(incorrectUpdateSql);
        timer.Stop();

        // Assert
        Assert.True(updateResponse.HasError == true);
        Assert.True(updateResponse.ErrorMessage == "Empty Input");
        Assert.True(timer.Elapsed.TotalSeconds <= 3);

        // Cleanup
        var logTransaction = new LogTransaction();
        logTransaction.DeleteDataAccessTransactionLog(updateResponse.LogId);
    }

}