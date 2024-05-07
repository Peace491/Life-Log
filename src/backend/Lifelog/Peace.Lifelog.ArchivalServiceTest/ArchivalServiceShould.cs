namespace Peace.Lifelog.ArchivalServiceTest;

using System.ComponentModel.DataAnnotations;
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

        var createImmutableTriggerSql = "DELIMITER //"
            + "CREATE TRIGGER prevent_log_updates_trigger"
            + $"BEFORE UPDATE ON {TABLE}"
            + "FOR EACH ROW"
            + "BEGIN"
            +  "    SIGNAL SQLSTATE '45000'"
            +  $"    SET MESSAGE_TEXT = 'Updates to the {TABLE} table are not allowed.'"
            + ";"
            + "END;"
            + "//"
            + "DELIMITER ;";

        var _ = DDLTransactionDAO.ExecuteDDLCommand(createMockTableSql);
        var __ = DDLTransactionDAO.ExecuteDDLCommand(createImmutableTriggerSql);
    }

    // Cleanup for all tests
    public async void Dispose()
    {
        var DDLTransactionDAO = new DDLTransactionDAO();

        var deleteMockTableSql = $"DROP TABLE {TABLE}";

        await DDLTransactionDAO.ExecuteDDLCommand(deleteMockTableSql);
    }

    #region Success
    [Fact]
    public async void S3Archive_Should_Archive()
    {
        // Arrange 
        var archivalService = new ArchivalService();
        Stopwatch timer = new Stopwatch();

        // Act
        timer.Start();
        var response = await archivalService.ArchiveFileToS3();
        timer.Stop();

        // Assert 
        Assert.True(response.HasError == false);
        Assert.True(timer.ElapsedMilliseconds < MAX_EXECUTION_TIME_IN_MILLISECONDS);
    }
    [Fact]
    public async void ArchivalServiceShould_Archive()
    {
        // Arrange 
        var archivalService = new ArchivalService();
        Stopwatch timer = new Stopwatch();
        var createDataOnlyDAO = new CreateDataOnlyDAO();

        // Act

        // Create logs that are valid to be archived
        var logInsert = $"INSERT INTO {TABLE} (Timestamp, UserHash, Level, Category, Message) VALUES (DATE_SUB(NOW(), INTERVAL 100 DAY), '{TEST_HASH}', '{LEVEL}', '{CATEGORY}', '{MESSAGE}');";
        for (int i=0; i < 9; i++)
        {
            var createLogResponse = await createDataOnlyDAO.CreateData(logInsert);
        }

        // Preform  archival
        timer.Start();
        var archivalResponse = await archivalService.Archive(DateTime.Today, TABLE);
        timer.Stop();

        // Assert 
        Assert.True(archivalResponse.HasError == false);
        Assert.True(timer.ElapsedMilliseconds < MAX_EXECUTION_TIME_IN_MILLISECONDS);
    }
    [Fact]
    public async void ArchivalServiceShould_RemoveArchivedLogsFromLogTable()
    {
        // Arrange 
        var archivalService = new ArchivalService();
        Stopwatch timer = new Stopwatch();
        var createDataOnlyDAO = new CreateDataOnlyDAO();
        var readDataOnlyDAO = new ReadDataOnlyDAO();

        var allLogsSql = $"SELECT * FROM {TABLE};";

        // Act

        // Create logs that are valid to be archived
        var logInsert = $"INSERT INTO {TABLE} (Timestamp, UserHash, Level, Category, Message) VALUES (DATE_SUB(NOW(), INTERVAL 100 DAY), '{TEST_HASH}', '{LEVEL}', '{CATEGORY}', '{MESSAGE}');";
        for (int i=0; i < 9; i++)
        {
            var createLogResponse = await createDataOnlyDAO.CreateData(logInsert);
        }

        // Preform  archival
        timer.Start();
        var archivalResponse = await archivalService.Archive(DateTime.Today, TABLE);
        timer.Stop();

        var readResponse = await readDataOnlyDAO.ReadData(allLogsSql, 1000000); 

        // Assert 
        Assert.True(archivalResponse.HasError == false);
        Assert.True(timer.ElapsedMilliseconds < MAX_EXECUTION_TIME_IN_MILLISECONDS);
        Assert.True(readResponse.Output != null);
        Assert.True(readResponse.Output.Count == 1); // 1 for archival logging itself
    }
    [Fact]
    public async void ArchivalServiceShouldNot_RemoveLogsWithin30Days()
    {
        // Arrange 
        var archivalService = new ArchivalService();
        Stopwatch timer = new Stopwatch();
        var createDataOnlyDAO = new CreateDataOnlyDAO();
        var readDataOnlyDAO = new ReadDataOnlyDAO();

        var allLogsSql = $"SELECT * FROM {TABLE};";

        // Act

        // Create logs that are valid to be archived
        var logInsertInvalid = $"INSERT INTO {TABLE} (Timestamp, UserHash, Level, Category, Message) VALUES (DATE_SUB(NOW(), INTERVAL 100 DAY), '{TEST_HASH}', '{LEVEL}', '{CATEGORY}', '{MESSAGE}');";
        var logInsertValid = $"INSERT INTO {TABLE} (Timestamp, UserHash, Level, Category, Message) VALUES (NOW(), '{TEST_HASH}', '{LEVEL}', '{CATEGORY}', '{MESSAGE}');";
        for (int i=0; i < 9; i++)
        {
            var createLogResponse1 = await createDataOnlyDAO.CreateData(logInsertInvalid);
        }
        for (int i=0; i < 9; i++)
        {
            var createLogResponse2 = await createDataOnlyDAO.CreateData(logInsertValid);
        }

        // Preform  archival
        timer.Start();
        var archivalResponse = await archivalService.Archive(DateTime.Today, TABLE);
        timer.Stop();

        var readResponse = await readDataOnlyDAO.ReadData(allLogsSql, 1000000); 

        // Assert 
        Assert.True(archivalResponse.HasError == false);
        Assert.True(timer.ElapsedMilliseconds < MAX_EXECUTION_TIME_IN_MILLISECONDS);
        Assert.True(readResponse.Output != null);
        Assert.True(readResponse.Output.Count == 10);
    }
    #endregion

    #region Failure
    // On Failure
    [Fact]
    public async void ArchiveShouldNot_DeleteLogsIfEncountersError()
    {
        // Arrange 
        var archivalService = new ArchivalService();
        Stopwatch timer = new Stopwatch();
        var createDataOnlyDAO = new CreateDataOnlyDAO();
        var readDataOnlyDAO = new ReadDataOnlyDAO();

        var oldDatetime = new DateTime(2000,1,1);

        var allLogsSql = $"SELECT * FROM {TABLE};";

        // Act

        // Create logs that are valid to be archived
        var logInsert = $"INSERT INTO {TABLE} (Timestamp, UserHash, Level, Category, Message) VALUES (DATE_SUB(NOW(), INTERVAL 100 DAY), '{TEST_HASH}', '{LEVEL}', '{CATEGORY}', '{MESSAGE}');";
        for (int i=0; i < 9; i++)
        {
            var createLogResponse = await createDataOnlyDAO.CreateData(logInsert);
        }

        // Preform  archival
        timer.Start();
        var archivalResponse = await archivalService.Archive(oldDatetime, TABLE);
        timer.Stop();

        var readResponse = await readDataOnlyDAO.ReadData(allLogsSql, 1000000); 


        // Assert 
        Assert.True(archivalResponse.HasError == true);
        Assert.True(timer.ElapsedMilliseconds < MAX_EXECUTION_TIME_IN_MILLISECONDS);
        Assert.True(readResponse.Output != null);
        Assert.True(readResponse.Output.Count == 9); // Only 9 due to termination early, won't log self
    }
    [Fact]
    public async void ArchiveShouldNot_ContinueOnInvalidDatetime()
    {
        // Arrange 
        var archivalService = new ArchivalService();
        Stopwatch timer = new Stopwatch();
        var createDataOnlyDAO = new CreateDataOnlyDAO();
        var readDataOnlyDAO = new ReadDataOnlyDAO();

        var oldDatetime = new DateTime(2000,1,1);

        var allLogsSql = $"SELECT * FROM {TABLE};";

        // Act

        // Create logs that are valid to be archived
        var logInsert = $"INSERT INTO {TABLE} (Timestamp, UserHash, Level, Category, Message) VALUES (DATE_SUB(NOW(), INTERVAL 100 DAY), '{TEST_HASH}', '{LEVEL}', '{CATEGORY}', '{MESSAGE}');";
        for (int i=0; i < 9; i++)
        {
            var createLogResponse = await createDataOnlyDAO.CreateData(logInsert);
        }

        // Preform  archival
        timer.Start();
        var archivalResponse = await archivalService.Archive(oldDatetime, TABLE);
        timer.Stop();

        var readResponse = await readDataOnlyDAO.ReadData(allLogsSql, 1000000); 

        // Assert 
        Assert.True(archivalResponse.HasError == true);
        Assert.True(timer.ElapsedMilliseconds < MAX_EXECUTION_TIME_IN_MILLISECONDS);
    }
    #endregion
}