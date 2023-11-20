namespace Peace.Lifelog.DataAccessTest;

using Peace.Lifelog.DataAccess;

using System.Diagnostics;

public class DeleteDataOnlyDAOShould
{
    [Fact]
    public void DeleteDataOnlyDAOShould_DeleteARecordInDataStore()
    {
        // Arrange: Set up before test execute
        var timer = new Stopwatch();
        var createOnlyDAO = new CreateDataOnlyDAO();
        var readOnlyDAO = new ReadDataOnlyDAO();
        var deleteOnlyDAO = new DeleteDataOnlyDAO();
        
        var table = "mockData";
        var deleteCategory = "Delete";
        var deleteMockData = "Mock Data";
    
        var createSql =  $"INSERT INTO {table} (Category, MockData) VALUES ('{deleteCategory}', '{deleteMockData}')";
        var readSql = $"SELECT MockData FROM {table} WHERE Category = '{deleteCategory}'";
        var deleteSql = $"DELETE FROM {table} WHERE Category = '{deleteCategory}'";

        // Act
        createOnlyDAO.CreateData(createSql);

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

    [Fact]
    public void DeleteDataOnlyDAOShould_DeleteMultipleRecordsInDataStore()
    {
        // Arrange: Set up before test execute
        var timer = new Stopwatch();
        var createOnlyDAO = new CreateDataOnlyDAO();
        var readOnlyDAO = new ReadDataOnlyDAO();
        var deleteOnlyDAO = new DeleteDataOnlyDAO();
        
        var table = "mockData";
        var deleteCategory = "Delete";
        var deleteMockData = "Mock Data";
        var numberOfRecords = 20;
    
        var createSql =  $"INSERT INTO {table} (Category, MockData) VALUES ('{deleteCategory}', '{deleteMockData}')";
        var readSql = $"SELECT MockData FROM {table} WHERE Category = '{deleteCategory}'";
        var deleteSql = $"DELETE FROM {table} WHERE Category = '{deleteCategory}'";

        // Act
        for (int i = 0; i < numberOfRecords; i++)
        {
            createOnlyDAO.CreateData(createSql); 
        }

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

    [Fact]
    public void DeleteDataOnlyDAOShould_ThrowErrorOnIncorrectSQLInput()
    {
        // Arrange: Set up before test execute
        var timer = new Stopwatch();
        var deleteOnlyDAO = new DeleteDataOnlyDAO();
        
        var table = "mockData";
        var deleteCategory = "Delete";
        
    
        var deleteSql = $"DLETE FROM {table} WHERE Category = '{deleteCategory}'";

        // Act
        timer.Start();
        var deleteResponse = deleteOnlyDAO.DeleteData(deleteSql);
        timer.Stop();

        // Assert
        Assert.True(deleteResponse.HasError == true);
        Assert.Contains("You have an error in your SQL syntax", deleteResponse.ErrorMessage);
        Assert.True(timer.Elapsed.TotalSeconds <= 3);
    }

    [Fact]
    public void DeleteDataOnlyDAOShould_ThrowErrorOnEmptyInput()
    {
        // Arrange: Set up before test execute
        var timer = new Stopwatch();
        var deleteOnlyDAO = new DeleteDataOnlyDAO();        
    
        var deleteSql = $"";

        // Act
        timer.Start();
        var deleteResponse = deleteOnlyDAO.DeleteData(deleteSql);
        timer.Stop();

        // Assert
        Assert.True(deleteResponse.HasError == true);
        Assert.True(deleteResponse.ErrorMessage == "Empty Input");
        Assert.True(timer.Elapsed.TotalSeconds <= 3);
    }
}