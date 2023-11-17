namespace Peace.Lifelog.DataAccessTest;

using Peace.Lifelog.DataAccess;

using System.Diagnostics;

public class CreateDataOnlyDAOShould
{
    [Fact]
    public void CreateDataOnlyDAOShould_CreateANewRecordInDataStore()
    {
        // Arrange
        var timer = new Stopwatch();
        var createOnlyDAO = new CreateDataOnlyDAO();
        var readOnlyDAO = new ReadDataOnlyDAO();
        var deleteOnlyDAO = new DeleteDataOnlyDAO();

        var table = "mockData";
        var createCategory = "Create";
        var createMockData = "Mock Data";
        // Need to create table

        var insertSql =  $"INSERT INTO {table} VALUES ('{createCategory}', '{createMockData}')";
        var readSql = $"SELECT MockData FROM {table} WHERE Category = '{createCategory}'";
        var deleteSql = $"DELETE FROM {table} WHERE Category = '{createCategory}'";

        // Act
        timer.Start();
        var createResponse = createOnlyDAO.CreateData(insertSql); // Need to test for all behavior of string
        timer.Stop();

        var readResponse = readOnlyDAO.ReadData(readSql);

        // Assert
        Assert.True(createResponse.HasError == false);
        Assert.True(timer.Elapsed.TotalSeconds <= 3);
        Assert.True(readResponse.HasError == false);

        // Cleanup
        deleteOnlyDAO.DeleteData(deleteSql);
    }
}