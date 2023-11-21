namespace Peace.Lifelog.DataAccessTest;

using Peace.Lifelog.DataAccess;

using System.Diagnostics;

public class CreateDataOnlyDAOShould
{
    [Fact]
    public void CreateDataOnlyDAOShould_ConnectToTheDataStore()
    {
        // Arrange
        var timer = new Stopwatch();
        var createOnlyDAO = new CreateDataOnlyDAO();

        // Act
        timer.Start();
        var dbConnection = createOnlyDAO.ConnectToDb(); // Need to test for all behavior of string
        dbConnection.Open();
        timer.Stop();

        var connectionState = dbConnection.State;
        dbConnection.Close();

        // Assert
        Assert.True(connectionState == System.Data.ConnectionState.Open);
        Assert.True(timer.Elapsed.TotalSeconds <= 3);
    }

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

        var insertSql =  $"INSERT INTO {table} (Category, MockData) VALUES ('{createCategory}', '{createMockData}')";
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

    [Fact]
    public void CreateDataOnlyDAOShould_ThrowErrorOnIncorrectSQLInput()
    {
        // Arrange
        var timer = new Stopwatch();
        var createOnlyDAO = new CreateDataOnlyDAO();

        var table = "mockData";
        var createCategory = "Create";
        var createMockData = "Mock Data";

        var incorrectInsertSql =  $"INSRT INTO {table} (Category, MockData) VALUES ('{createCategory}', '{createMockData}')";

        // Act
        timer.Start();
        var createResponse = createOnlyDAO.CreateData(incorrectInsertSql);
        timer.Stop();

        // Assert
        Assert.True(createResponse.HasError == true);
        Assert.Contains("You have an error in your SQL syntax", createResponse.ErrorMessage);
        Assert.True(timer.Elapsed.TotalSeconds <= 3);
    }

    [Fact]
    public void CreateDataOnlyDAOShould_ThrowErrorOnEmptyInput()
    {
        // Arrange
        var timer = new Stopwatch();
        var createOnlyDAO = new CreateDataOnlyDAO();

        var insertSql =  "";

        // Act
        timer.Start();
        var createResponse = createOnlyDAO.CreateData(insertSql);
        timer.Stop();

        // Assert
        Assert.True(createResponse.HasError == true);
        Assert.True(createResponse.ErrorMessage == "Empty Input");
        Assert.True(timer.Elapsed.TotalSeconds <= 3);
    }
}