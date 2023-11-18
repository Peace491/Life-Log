namespace Peace.Lifelog.DataAccessTest;

using Peace.Lifelog.DataAccess;

using System.Diagnostics;

public class UpdateDataOnlyDAOShould
{
    [Fact]
    public void UpdateDataOnlyDAOShould_UpdateARecordInDataStore()
    {
        // Arrange: Set up before test execute
        var timer = new Stopwatch();
        var createOnlyDAO = new CreateDataOnlyDAO();
        var readOnlyDAO = new ReadDataOnlyDAO();
        var updateOnlyDAO = new UpdateDataOnlyDAO();
        var deleteOnlyDAO = new DeleteDataOnlyDAO();
        
        var table = "mockData";
        var updateCategory = "Update";
        var oldMockData = "Old Mock Data";
        var newMockData = "New Mock Data";
    
        var createSql =  $"INSERT INTO {table} VALUES ('{updateCategory}', '{oldMockData}')";
        var readSql = $"SELECT {table} FROM {table} WHERE Category = '{updateCategory}'";
        var updateSql = $"UPDATE {table} SET MockData = '{newMockData}'";
        var deleteSql = $"DELETE FROM {table} WHERE Category = '{updateCategory}'";
        
        
        // Act
        createOnlyDAO.CreateData(createSql);

        var originalReadResponse = readOnlyDAO.ReadData(readSql);

        timer.Start();
        var updateResponse = updateOnlyDAO.UpdateData(updateSql);
        timer.Stop();

        var newReadResponse = readOnlyDAO.ReadData(readSql);

        // Assert
        Assert.True(updateResponse.HasError == false);
        Assert.True(originalReadResponse.Output != newReadResponse.Output);
        foreach (List<Object> newReadResponseData in newReadResponse.Output)
        {
            // Use the MySqlDataReader
            Assert.True(newReadResponseData[0].ToString() == newMockData);

        }
        Assert.True(timer.Elapsed.TotalSeconds <= 3);

        // Cleanup
        deleteOnlyDAO.DeleteData(deleteSql);
    }

    [Fact]
    public void UpdateDataOnlyDAOShould_UpdateMultipleRecordsInDataStore()
    {
        // Arrange: Set up before test execute
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
    
        var createSql =  $"INSERT INTO {table} VALUES ('{updateCategory}', '{oldMockData}')";
        var readSql = $"SELECT {table} FROM {table} WHERE Category = '{updateCategory}'";
        var updateSql = $"UPDATE {table} SET MockData = '{newMockData}'";
        var deleteSql = $"DELETE FROM {table} WHERE Category = '{updateCategory}'";
        
        
        // Act
        for (int i = 0; i < numberOfRecords; i++)
        {
            createOnlyDAO.CreateData(createSql); 
        }

        var originalReadResponse = readOnlyDAO.ReadData(readSql);

        timer.Start();
        var updateResponse = updateOnlyDAO.UpdateData(updateSql);
        timer.Stop();

        var newReadResponse = readOnlyDAO.ReadData(readSql);

        // Assert
        Assert.True(updateResponse.HasError == false);
        Assert.True(originalReadResponse.Output != newReadResponse.Output);
        foreach (List<Object> newReadResponseData in newReadResponse.Output)
        {
            // Use the MySqlDataReader
            Assert.True(newReadResponseData[0].ToString() == newMockData);

        }
        Assert.True(timer.Elapsed.TotalSeconds <= 3);

        // Cleanup
        deleteOnlyDAO.DeleteData(deleteSql);
    }

    [Fact]
    public void UpdateDataOnlyDAOShould_ReturnNullIfNoRecordIsFoundInDataStore()
    {
        // Arrange: Set up before test execute
        var timer = new Stopwatch();
        var updateOnlyDAO = new UpdateDataOnlyDAO();
        
        var table = "mockData";
        var updateMockData = "Mock Data";
    
        var updateSql = $"UPDATE {table} SET MockData = '{updateMockData}'";
        
        
        // Act
        timer.Start();
        var updateResponse = updateOnlyDAO.UpdateData(updateSql);
        timer.Stop();

        // Assert
        Assert.True(updateResponse.HasError == false);
        Assert.True(timer.Elapsed.TotalSeconds <= 3);
    }

    [Fact]
    public void UpdateDataOnlyDAOShould_ThrowErrorOnIncorrectSQLInput()
    {
        // Arrange: Set up before test execute
        var timer = new Stopwatch();
        var updateOnlyDAO = new UpdateDataOnlyDAO();
        
        var table = "mockData";
        var updateMockData = "Mock Data";
    
        var incorrectUpdateSql = $"UDATE {table} SET MockData = '{updateMockData}'";
        
        
        // Act
        timer.Start();
        var updateResponse = updateOnlyDAO.UpdateData(incorrectUpdateSql);
        timer.Stop();

        // Assert
        Assert.True(updateResponse.HasError == true);
        Assert.Contains("You have an error in your SQL syntax", updateResponse.ErrorMessage);
        Assert.True(timer.Elapsed.TotalSeconds <= 3);
    }

    [Fact]
    public void UpdateDataOnlyDAOShould_ThrowErrorOnEmptyInput()
    {
        // Arrange: Set up before test execute
        var timer = new Stopwatch();
        var updateOnlyDAO = new UpdateDataOnlyDAO();
    
        var incorrectUpdateSql = $"";
        
        
        // Act
        timer.Start();
        var updateResponse = updateOnlyDAO.UpdateData(incorrectUpdateSql);
        timer.Stop();

        // Assert
        Assert.True(updateResponse.HasError == true);
        Assert.True(updateResponse.ErrorMessage == "Empty Input");
        Assert.True(timer.Elapsed.TotalSeconds <= 3);
    }

}