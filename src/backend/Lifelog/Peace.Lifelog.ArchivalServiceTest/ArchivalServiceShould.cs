namespace Peace.Lifelog.ArchivalServiceTest;

using System.Diagnostics;
using System.Threading.Tasks;
using Peace.Lifelog.ArchivalService;
using Peace.Lifelog.DataAccess;
using Peace.Lifelog.Logging;
using DomainModels;
using ZstdSharp.Unsafe;

public class ArchivalServiceShould : IDisposable
{
    private const int MAX_EXECUTION_TIME_IN_MILLISECONDS = 60001;
    private const int LOG_ID_INDEX = 0;
    private const string TABLE = "MockLogs";
    private const string TEST_HASH = "TxT3KzlpTG0ExziT6GhXfJDStrAssjrEZjbe14UBfvU=";
    private const string LEVEL = "Debug";
    private const string CATEGORY = "View";
    private const string MESSAGE = "Hard Insert for testing";

    public ArchivalServiceShould()
    {
        var DDLTransactionDAO = new DDLTransactionDAO();

        var createMockTableSql = $"CREATE TABLE {TABLE} ("
            + "Id INT PRIMARY KEY AUTO_INCREMENT,"
            + "Timestamp TIMESTAMP,"
            + "UserHash VARCHAR(255),"
            + "Level VARCHAR(255),"
            + "Category VARCHAR(255),"
            + "Message TEXT"
        + ");";

        _ = DDLTransactionDAO.ExecuteDDLCommand(createMockTableSql);
    }

    // Cleanup for all tests
    public async void Dispose()
    {
        var DDLTransactionDAO = new DDLTransactionDAO();

        var deleteMockTableSql = $"DROP TABLE {TABLE}";

        await DDLTransactionDAO.ExecuteDDLCommand(deleteMockTableSql);
    }
    [Fact]
    public async Task S3Archive_Should_Archive()
    {
        // Arrange 
        var archivalService = new ArchivalService();
        Stopwatch timer = new Stopwatch();

        // Act
        timer.Start();
        var response = await archivalService.ArchiveFileToS3(TABLE);
        timer.Stop();

        // Assert 
        Assert.True(response.HasError == false);
        Assert.True(timer.ElapsedMilliseconds < MAX_EXECUTION_TIME_IN_MILLISECONDS);
    }
    [Fact]
    public async Task S3Archive_Should_RemoveArchivedLogs()
    {
        // Arrange 
        var archivalService = new ArchivalService();
        var readDataOnlyDAO = new ReadDataOnlyDAO();
        var response = new Response();

        // TODO: Write some logs to the table


        response = await archivalService.ArchiveFileToS3(TABLE);

        // Act 
        // read from mock table to check if logs are deleted

        // Assert
        Assert.True(response.HasError == false);
    }
    [Fact]
    public async Task S3Archive_Should_CleanupZipFile()
    {
        // Arrange 
        var archivalService = new ArchivalService();
        var response = new Response();

        // Act
        response = await archivalService.ArchiveFileToS3(TABLE);


        // dynamically create the zip file path
        // Assert
        // Assert.True(File.Exists(filePath));
    }
}