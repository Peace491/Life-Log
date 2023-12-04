namespace Peace.Lifelog.DataAccessTest;

using DomainModels;
using Peace.Lifelog.DataAccess;

using System.Diagnostics;

public class ReadDataOnlyDAOShould
{
    private const int MAX_EXECUTION_TIME_IN_SECONDS = 3;
    private const int DEFAULT_RECORD_COUNT = 1;
    private const int DEFAULT_PAGE_NUMBER = 0;
    private const int DEFAULT_NUMBER_OF_RECORDS = 20;

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
        Assert.True(timer.Elapsed.TotalSeconds <= MAX_EXECUTION_TIME_IN_SECONDS);
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
        var deleteSql = $"DELETE FROM {table} WHERE Category = '{readCategory}' AND Id <> 0";

        // Act
        var createResponse = await createOnlyDAO.CreateData(createSql); // Need to test for all behavior of string

        timer.Start();
        var readResponse = await readOnlyDAO.ReadData(readSql, DEFAULT_RECORD_COUNT); // Issue might be because create Response is not finished
        timer.Stop();
        
        // Assert
        Assert.True(readResponse.HasError == false);
        Assert.True(readResponse.Output.Count == DEFAULT_RECORD_COUNT);
        foreach (List<Object> readResponseData in readResponse.Output)
        {
            Assert.True(readResponseData[0].ToString() == readMockData);
        }
        Assert.True(timer.Elapsed.TotalSeconds <= MAX_EXECUTION_TIME_IN_SECONDS);

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

        var createSql =  $"INSERT INTO {table} (Category, MockData) VALUES ('{readCategory}', '{readMockData}')";
        var readSql = $"SELECT MockData FROM {table} WHERE Category = '{readCategory}'";
        var deleteSql = $"DELETE FROM {table} WHERE Category = '{readCategory}' AND Id <> 0";

        List<Response> createResponses = new List<Response>();

        // Act
        for (int i = 0; i < DEFAULT_NUMBER_OF_RECORDS; i++)
        {
            var createResponse = await createOnlyDAO.CreateData(createSql); 
            createResponses.Add(createResponse);
        }

        timer.Start();
        var readResponse = await readOnlyDAO.ReadData(readSql, DEFAULT_NUMBER_OF_RECORDS, DEFAULT_PAGE_NUMBER);
        timer.Stop();
        
        // Assert
        Assert.True(readResponse.HasError == false);
        Assert.True(readResponse.Output.Count == DEFAULT_NUMBER_OF_RECORDS);
        foreach (List<Object> readResponseData in readResponse.Output)
        {
            Assert.True(readResponseData[0].ToString() == readMockData);
        }
        Assert.True(timer.Elapsed.TotalSeconds <= MAX_EXECUTION_TIME_IN_SECONDS);

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
    
        // ... (previous methods)

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
        Assert.True(timer.Elapsed.TotalSeconds <= MAX_EXECUTION_TIME_IN_SECONDS);

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
        Assert.True(timer.Elapsed.TotalSeconds <= MAX_EXECUTION_TIME_IN_SECONDS);

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
        Assert.True(timer.Elapsed.TotalSeconds <= MAX_EXECUTION_TIME_IN_SECONDS);

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
        Assert.True(timer.Elapsed.TotalSeconds <= MAX_EXECUTION_TIME_IN_SECONDS);

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
        var readResponse = await readOnlyDAO.ReadData(readSql, DEFAULT_RECORD_COUNT, invalidPage);
        timer.Stop();
        
        // Assert
        Assert.True(readResponse.HasError == true);
        Assert.True(readResponse.ErrorMessage == "Invalid Page");
        Assert.True(timer.Elapsed.TotalSeconds <= MAX_EXECUTION_TIME_IN_SECONDS);

        // Cleanup
        var logTransaction = new LogTransaction();
        await logTransaction.DeleteDataAccessTransactionLog(readResponse.LogId);
    }

}
