namespace Peace.Lifelog.ArchivalServiceTest;

using System.Diagnostics;
using System.Threading.Tasks;
using Peace.Lifelog.ArchivalService;
using Peace.Lifelog.DataAccess;
using Peace.Lifelog.Logging;
using ZstdSharp.Unsafe;

public class ArchivalServiceShould
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
            + "LogID INT PRIMARY KEY AUTO_INCREMENT,"
            + "LogTimestamp TIMESTAMP,"
            + "LogUserHash VARCHAR(255),"
            + "LogLevel VARCHAR(255),"
            + "LogCategory VARCHAR(255),"
            + "LogMessage TEXT"
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

        DDLTransactionDAO.ExecuteDDLCommand(createMockTableSql);
        DDLTransactionDAO.ExecuteDDLCommand(createImmutableTriggerSql);
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
    public async void ArchivalServiceShould_Archive()
    {
        
        // Arrange 
        var archivalService = new ArchivalService();
        Stopwatch timer = new Stopwatch();
        var createOnlyDAO = new CreateDataOnlyDAO();

        // Act
        var logInsert = $"INSERT INTO {TABLE} (LogTimestamp, LogUserHash, LogLevel, LogCategory, LogMessage) VALUES (DATE_SUB(NOW(), INTERVAL 100 DAY), '{TEST_HASH}', '{LEVEL}', '{CATEGORY}', '{MESSAGE}');";
        var createLogResponse = await createOnlyDAO.CreateData(logInsert);
        timer.Start();
        var archivalResponse = await archivalService.Archive(DateTime.Today, TABLE);
        timer.Stop();

        // Assert 
        Assert.True(archivalResponse.HasError == false);
        Assert.True(timer.ElapsedMilliseconds < MAX_EXECUTION_TIME_IN_MILLISECONDS);
        Dispose();
    }
    [Fact]
    public async void ArchivalServiceShould_RemoveArchivedLogsFromLogTable()
    {

    }
    #endregion

    #region Failure
    // On Failure
    [Fact]
    public void ArchiveShouldNot_DeleteLogsIfEncountersError()
    {

    }
    [Fact]
    public void ArchiveShouldNot_ContinueOnInvalidDatetime()
    {

    }
    [Fact]
    public void ArchiveShouldNot_ContinueOnDbInaccessable()
    {

    }
    [Fact]
    public void ArchiveShouldNot_ContinueOnS3InstanceInaccessable()
    {

    }
    [Fact]
    public void ArchiveShouldNot_ContinueOnUnableToWriteToFile()
    {

    }
    #endregion
}