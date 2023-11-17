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
    
        var insertSql =  $"INSERT INTO {table} VALUES ('{deleteCategory}', '{deleteMockData}')";
        var readSql = $"SELECT MockData FROM {table} WHERE Category = '{deleteCategory}'";
        var deleteSql = $"DELETE FROM {table} WHERE Category = '{deleteCategory}'";

        // Act
        createOnlyDAO.CreateData(insertSql);

        timer.Start();
        var deleteResponse = deleteOnlyDAO.DeleteData(deleteSql);
        timer.Stop();
        var readResponse = readOnlyDAO.ReadData(readSql);

        // Assert
        Assert.True(deleteResponse.HasError == false);
        // Assert.True(timer.Elapsed.TotalSeconds <= 3);
        Assert.True(readResponse.HasError == false);
        Assert.True(readResponse.Output == null);
    }
}