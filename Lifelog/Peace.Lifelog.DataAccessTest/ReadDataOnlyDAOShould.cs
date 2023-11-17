namespace Peace.Lifelog.DataAccessTest;

using Peace.Lifelog.DataAccess;

using System.Diagnostics;

public class ReadDataOnlyDAOShould
{
    [Fact]
    public void ReadDataOnlyDAOShould_ReadSingleRecordInDataStore()
    {
        // Arrange: Set up before test execute
        var timer = new Stopwatch();
        var createOnlyDAO = new CreateDataOnlyDAO();
        var readOnlyDAO = new ReadDataOnlyDAO();
        var deleteOnlyDAO = new DeleteDataOnlyDAO();

        var table = "mockData";
        var readCategory = "Read";
        var readMockData = "Mock Data";

        var insertSql =  $"INSERT INTO {table} VALUES ('{readCategory}', '{readMockData}')";
        var readSql = $"SELECT MockData FROM {table} WHERE Category = '{readCategory}'";
        var deleteSql = $"DELETE FROM {table} WHERE Category = '{readCategory}'";

        // Act
        var createResponse = createOnlyDAO.CreateData(insertSql); // Need to test for all behavior of string

        timer.Start();
        var readResponse = readOnlyDAO.ReadData(readSql);
        timer.Stop();
        
        // Assert
        Assert.True(readResponse.HasError == false);
        
        foreach (List<Object> readResponseData in readResponse.Output)
        {
            // Use the MySqlDataReader
            Assert.True(readResponseData[0].ToString() == readMockData);

        }

        Assert.True(timer.Elapsed.TotalSeconds <= 3);

        // Cleanup
        deleteOnlyDAO.DeleteData(deleteSql);
    }

    [Fact]
    public void ReadDataOnlyDAOShould_ReadMultipleRecordsInDataStore()
    {
        // Arrange: Set up before test execute
        var timer = new Stopwatch();
        var createOnlyDAO = new CreateDataOnlyDAO();
        var readOnlyDAO = new ReadDataOnlyDAO();
        var deleteOnlyDAO = new DeleteDataOnlyDAO();

        var table = "mockData";
        var readCategory = "Read";
        var readMockData = "Mock Data";
        var numberOfRecords = 20;

        var insertSql =  $"INSERT INTO {table} VALUES ('{readCategory}', '{readMockData}')";
        var readSql = $"SELECT MockData FROM {table} WHERE Category = '{readCategory}'";
        var deleteSql = $"DELETE FROM {table} WHERE Category = '{readCategory}'";

        // Act
        for (int i = 0; i < numberOfRecords; i++)
        {
            createOnlyDAO.CreateData(insertSql); 
        }

        timer.Start();
        var readResponse = readOnlyDAO.ReadData(readSql, numberOfRecords, 0);
        timer.Stop();
        
        // Assert
        Assert.True(readResponse.HasError == false);

        Assert.True(readResponse.Output.Count == numberOfRecords);
        
        foreach (List<Object> readResponseData in readResponse.Output)
        {
            // Use the MySqlDataReader
            Assert.True(readResponseData[0].ToString() == readMockData);

        }

        Assert.True(timer.Elapsed.TotalSeconds <= 3);

        // Cleanup
        deleteOnlyDAO.DeleteData(deleteSql);
    }
    
}