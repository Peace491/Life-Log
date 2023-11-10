namespace Peace.Lifelog.DataAccessTest;

using System.Diagnostics;

// TDD
// Unit testing - You define a unit of work and test that

public class DataAccessShould
{
    [Fact]
    public void DataAccessShould_CreateANewRecordInDataStore()
    {
        // Arrange: Set up before test execute
        var timer = new Stopwatch();
        var createOnlyDAO = new CreateOnlyDAO();
        var readOnlyDAO = new ReadOnlyDAO();

        var createCategory = "Create";
        var createMockData = "Create Mock Data";
        var insertSql =  $"INSERT INTO mockData (Category, MockData) VALUES {(createCategory, createMockData)}";

        // Act
        timer.Start();
        var createResponse = createOnlyDAO.CreateData(insertSql);
        timer.Stop();

        var recordId = createResponse.Output.Id;
        var readSql = $"SELECT data FROM mockData WHERE Id = {recordId}";
        var readResponse = readOnlyDAO.ReadData(readSql);

        // Assert
        Assert.True(createResponse.HasError == false);
        Assert.True(timer.Elapsed.TotalSeconds <= 3);
        Assert.True(readResponse.HasError == false);
        Assert.True(readResponse.Output[0].Category == createCategory);
        Assert.True(readResponse.Output[0].MockData == createMockData);
    }

    [Fact]
    public void DataAccessShould_ReadSingleRecordInDataStore()
    {
        // Arrange: Set up before test execute
        var timer = new Stopwatch();
        var createOnlyDAO = new CreateOnlyDAO();
        var readOnlyDAO = new ReadOnlyDAO();

        var readSingleCategory = "Read Single";
        var readSingleMockData = "Read Single Mock Data";

        var readSingleSql = $"SELECT data FROM mockData WHERE Category = {readSingleCategory}";

        // Act
        timer.Start();
        var readResponse = readOnlyDAO.ReadData(readSingleSql);
        timer.Stop();
        
        // Assert
        Assert.True(readResponse.HasError == false);
        Assert.True(readResponse.Output[0].Category == readSingleCategory);
        Assert.True(readResponse.Output[0].MockData == readSingleMockData);
        Assert.True(timer.Elapsed.TotalSeconds <= 3);
    }

    [Fact]
    public void DataAccessShould_ReadMultipleRecordsInDataStore()
    {
        // Arrange: Set up before test execute
        var timer = new Stopwatch();
        var readOnlyDAO = new ReadOnlyDAO();

        var readMultipleCategory = "Read Multiple";
        var readMultipleMockData = "Read Multiple Mock Data";

        var readMultipleSql = $"SELECT FROM mockData WHERE Category={readMultipleCategory}";
        var count = 10;
        var page = 0;

        // Act
        timer.Start();
        var readResponse = readOnlyDAO.ReadData(readMultipleSql, count, page);
        timer.Stop();
        
        // Assert
        Assert.True(readResponse.HasError == false);
        foreach (var mockObject in readResponse.Output) {
            Assert.True(mockObject.Category == readMultipleCategory);
            Assert.True(mockObject.MockData == readMultipleMockData);
        }
        Assert.True(timer.Elapsed.TotalSeconds <= 3);
    }

    [Fact]
    public void DataAccessShould_UpdateARecordInDataStore()
    {
        // Arrange: Set up before test execute
        var timer = new Stopwatch();
        var createOnlyDAO = new CreateOnlyDAO();
        var readOnlyDAO = new ReadOnlyDAO();
        var updateOnlyDAO = new UpdateOnlyDAO();
        
        var updateRecordCategory = "Update";
        var updateRecordOriginalMockData = "Update Old Mock Data";
        var updateRecordNewMockData = "Update New Mock Data";

        var createSql = $"INSERT INTO mockData (Category, MockData) VALUES {(updateRecordCategory, updateRecordOriginalMockData)}";
        var createResponse = createOnlyDAO.CreateData(createSql);
        var recordId = createResponse.Output.Id;

        var readSql = $"SELECT FROM mockData WHERE Id={recordId}";
        var updateSql = $"UPDATE mockData SET mockData = {updateRecordNewMockData}";
        
        
        // Act
        var originalReadResponse = readOnlyDAO.ReadData(readSql);
        timer.Start();
        var updateResponse = updateOnlyDAO.UpdateData(updateSql);
        timer.Stop();
        var newReadResponse = readOnlyDAO.ReadData(readSql);

        // Assert
        Assert.True(updateResponse.HasError == false);
        Assert.True(originalReadResponse.Output[0].MockData != newReadResponse.Output.MockData);
        Assert.True(timer.Elapsed.TotalSeconds <= 3);
    }
    

    [Fact]
    public void DataAccessShould_DeleteARecordInDataStore()
    {
        // Arrange: Set up before test execute
        var timer = new Stopwatch();
        var createOnlyDAO = new CreateOnlyDAO();
        var readOnlyDAO = new ReadOnlyDAO();
        var deleteOnlyDAO = new DeleteOnlyDAO();
        
        var deleteCategory = "Delete";
        var deleteMockData = "Delete Mock Data";
    
        var createSql = $"INSERT INTO mockData (Category, MockData) VALUES {(deleteCategory, deleteMockData)}";
        var createResponse = createOnlyDAO.CreateData(createSql);
        var recordId = createResponse.Response.Output.Id;

        var readSql = $"SELECT FROM mockData WHERE Id={recordId}";
        var deleteSql = $"DELETE FROM mockData WHERE Id = {recordId}";

        // Act
        timer.Start();
        var deleteResponse = deleteOnlyDAO.DeleteData(deleteSql);
        timer.Stop();
        var readResponse = readOnlyDAO.ReadData(readSql);

        // Assert
        Assert.True(deleteResponse.HasError == false);
        Assert.True(timer.Elapsed.TotalSeconds <= 3);
        Assert.True(readResponse.HasError == false);
        Assert.True(readResponse.Output == null);
    }
}