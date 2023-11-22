namespace Peace.Lifelog.DataAccessTest;

using DomainModels;
using Peace.Lifelog.DataAccess;

using System.Diagnostics;

public class ReadDataOnlyDAOShould
{
    [Fact]
    public async void ReadDataOnlyDAOShould_ConnectToTheDataStore()
    {
        // Arrange
        var timer = new Stopwatch();
        var readOnlyDAO = new ReadDataOnlyDAO();

        // Act
        timer.Start();
        var dbConnection = readOnlyDAO.ConnectToDb(); // Need to test for all behavior of string
        dbConnection.Open();
        timer.Stop();

        var connectionState = dbConnection.State;
        dbConnection.Close();

        // Assert
        Assert.True(connectionState == System.Data.ConnectionState.Open);
        Assert.True(timer.Elapsed.TotalSeconds <= 3);
    }

    [Fact]
    public async void ReadDataOnlyDAOShould_ReadSingleRecordInDataStore()
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
        var readSql = $"SELECT MockData FROM {table} WHERE Category = '{readCategory}'";
        var deleteSql = $"DELETE FROM {table} WHERE Category = '{readCategory}'";

        // Act
        var createResponse = await createOnlyDAO.CreateData(createSql); // Need to test for all behavior of string

        timer.Start();
        var readResponse = await readOnlyDAO.ReadData(readSql, 1); // Issue might be because create Response is not finished
        timer.Stop();
        
        // Assert
        Assert.True(readResponse.HasError == false);
        Assert.True(readResponse.Output.Count == 1);
        foreach (List<Object> readResponseData in readResponse.Output)
        {
            Assert.True(readResponseData[0].ToString() == readMockData);
        }
        Assert.True(timer.Elapsed.TotalSeconds <= 3);

        // Cleanup
        var deleteResponse = await deleteOnlyDAO.DeleteData(deleteSql);

        var logTransaction = new LogTransaction();
        await logTransaction.DeleteDataAccessTransactionLog(createResponse.LogId);
        await logTransaction.DeleteDataAccessTransactionLog(readResponse.LogId);
        await logTransaction.DeleteDataAccessTransactionLog(deleteResponse.LogId);
    }

    [Fact]
    public async void ReadDataOnlyDAOShould_ReadMultipleRecordsInDataStore()
    {
        // Arrange
        var timer = new Stopwatch();
        var createOnlyDAO = new CreateDataOnlyDAO();
        var readOnlyDAO = new ReadDataOnlyDAO();
        var deleteOnlyDAO = new DeleteDataOnlyDAO();

        var table = "mockData";
        var readCategory = "Read Multiple";
        var readMockData = "Mock Data";
        var numberOfRecords = 20;

        var createSql =  $"INSERT INTO {table} (Category, MockData) VALUES ('{readCategory}', '{readMockData}')";
        var readSql = $"SELECT MockData FROM {table} WHERE Category = '{readCategory}'";
        var deleteSql = $"DELETE FROM {table} WHERE Category = '{readCategory}'";

        List<Response> createResponses = new List<Response>();

        // Act
        for (int i = 0; i < numberOfRecords; i++)
        {
            var createResponse = await createOnlyDAO.CreateData(createSql); 
            createResponses.Add(createResponse);
        }

        timer.Start();
        var readResponse = await readOnlyDAO.ReadData(readSql, numberOfRecords, 0);
        timer.Stop();
        
        // Assert
        Assert.True(readResponse.HasError == false);
        Assert.True(readResponse.Output.Count == numberOfRecords);
        foreach (List<Object> readResponseData in readResponse.Output)
        {
            Assert.True(readResponseData[0].ToString() == readMockData);
        }
        Assert.True(timer.Elapsed.TotalSeconds <= 3);

        // Cleanup
        var deleteResponse = await deleteOnlyDAO.DeleteData(deleteSql);

        var logTransaction = new LogTransaction();

        await logTransaction.DeleteDataAccessTransactionLog(readResponse.LogId);

        foreach (Response createResponse in createResponses)
        {
            await logTransaction.DeleteDataAccessTransactionLog(createResponse.LogId);
        }

        await logTransaction.DeleteDataAccessTransactionLog(deleteResponse.LogId);
    }
    
    [Fact]
    public async void ReadDataOnlyDAOShould_ReturnNullIfNoRecordIsFoundInDataStore()
    {
        // Arrange
        var timer = new Stopwatch();
        var readOnlyDAO = new ReadDataOnlyDAO();

        var table = "mockData";
        var readCategory = "Read Null";

        var readSql = $"SELECT MockData FROM {table} WHERE Category = '{readCategory}'";

        // Act
        timer.Start();
        var readResponse = await readOnlyDAO.ReadData(readSql);
        timer.Stop();
        
        // Assert
        Assert.True(readResponse.HasError == false);
        Assert.True(readResponse.Output == null);
        Assert.True(timer.Elapsed.TotalSeconds <= 3);

        // Cleanup
        var logTransaction = new LogTransaction();
        await logTransaction.DeleteDataAccessTransactionLog(readResponse.LogId);
    }

    [Fact]
    public async void ReadDataOnlyDAOShould_ThrowErrorOnIncorrectSQLInput()
    {
        // Arrange: Set up before test execute
        var timer = new Stopwatch();
        var readOnlyDAO = new ReadDataOnlyDAO();

        var table = "mockData";
        var readCategory = "Read";

        var readSql = $"SLECT MockData FROM {table} WHERE Category = '{readCategory}'";

        // Act
        timer.Start();
        var readResponse = await readOnlyDAO.ReadData(readSql);
        timer.Stop();
        
        // Assert
        Assert.True(readResponse.HasError == true);
        Assert.Contains("You have an error in your SQL syntax", readResponse.ErrorMessage);
        Assert.True(timer.Elapsed.TotalSeconds <= 3);

        // Cleanup
        var logTransaction = new LogTransaction();
        await logTransaction.DeleteDataAccessTransactionLog(readResponse.LogId);
    }

    [Fact]
    public async void ReadDataOnlyDAOShould_ThrowErrorOnEmptyInput()
    {
        // Arrange
        var timer = new Stopwatch();
        var readOnlyDAO = new ReadDataOnlyDAO();

        var readSql = $"";

        // Act
        timer.Start();
        var readResponse = await readOnlyDAO.ReadData(readSql);
        timer.Stop();
        
        // Assert
        Assert.True(readResponse.HasError == true);
        Assert.True(readResponse.ErrorMessage == "Empty Input");
        Assert.True(timer.Elapsed.TotalSeconds <= 3);

        // Cleanup
        var logTransaction = new LogTransaction();
        await logTransaction.DeleteDataAccessTransactionLog(readResponse.LogId);
    }

    [Fact]
    public async void ReadDataOnlyDAOShould_ThrowErrorOnInvalidCountInput()
    {
        // Arrange
        var timer = new Stopwatch();
        var readOnlyDAO = new ReadDataOnlyDAO();

        var table = "mockData";
        var readCategory = "Read";

        var readSql = $"SELECT MockData FROM {table} WHERE Category = '{readCategory}'";

        var invalidCount = -1;

        // Act
        timer.Start();
        var readResponse = await readOnlyDAO.ReadData(readSql, invalidCount);
        timer.Stop();
        
        // Assert
        Assert.True(readResponse.HasError == true);
        Assert.True(readResponse.ErrorMessage == "Invalid Count");
        Assert.True(timer.Elapsed.TotalSeconds <= 3);

        // Cleanup
        var logTransaction = new LogTransaction();
        await logTransaction.DeleteDataAccessTransactionLog(readResponse.LogId);
    }

    [Fact]
    public async void ReadDataOnlyDAOShould_ThrowErrorOnInvalidPageInput()
    {
        // Arrange
        var timer = new Stopwatch();
        var readOnlyDAO = new ReadDataOnlyDAO();

        var table = "mockData";
        var readCategory = "Read";

        var readSql = $"SELECT MockData FROM {table} WHERE Category = '{readCategory}'";

        var invalidPage = -1;

        // Act
        timer.Start(); 
        var readResponse = await readOnlyDAO.ReadData(readSql, 1, invalidPage);
        timer.Stop();
        
        // Assert
        Assert.True(readResponse.HasError == true);
        Assert.True(readResponse.ErrorMessage == "Invalid Page");
        Assert.True(timer.Elapsed.TotalSeconds <= 3);

        // Cleanup
        var logTransaction = new LogTransaction();
        await logTransaction.DeleteDataAccessTransactionLog(readResponse.LogId);
    }
}