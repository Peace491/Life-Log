namespace Peace.Lifelog.ArchivalServiceTest;

using System.Threading.Tasks;
using Peace.Lifelog.ArchivalService;
using Peace.Lifelog.DataAccess;
using Peace.Lifelog.Logging;
public class ArchivalServiceShould
{
    private const int MAX_EXECUTION_TIME_IN_SECONDS = 3001;
    private const int LOG_ID_INDEX = 0;
    private const string TABLE = "Logs";

    private const string TEST_HASH = "TxT3KzlpTG0ExziT6GhXfJDStrAssjrEZjbe14UBfvU=";

    #region Success
    // On Success
    [Fact]
    public async void ArchivalServiceShould_SelectLogsOlderThan30DaysOld()
    {
        /* // Arrange 
        string testLevel = "Debug";
        string testCategory = "View";
        string? testMessage = null;

        // Need to initlaize all types of DAO for checking accuracy and cleanup.
        var createOnlyDAO = new CreateDataOnlyDAO();
        var logTarget = new LogTarget(createOnlyDAO);
        var logger = new Logging(logTarget);
        var readOnlyDAO = new ReadDataOnlyDAO();
        var deleteDataDAO = new DeleteDataOnlyDAO(); 

        var createLogResponse1 = await logger.CreateLog(TABLE, TEST_HASH, testLevel, testCategory, testMessage);
        var createLogResponse2 = await logger.CreateLog(TABLE, TEST_HASH, testLevel, testCategory, testMessage);
        var createLogResponse3 = await logger.CreateLog(TABLE, TEST_HASH, testLevel, testCategory, testMessage);
        
        string readLogs = $"SELECT * FROM {TABLE} WHERE LogTimestamp > DATE_SUB(CURRENT_DATE, INTERVAL 30 DAY);";
        
        var readLogResponse = await readOnlyDAO.ReadData(readLogs, 1000000);

        string archiveString = ""; 

        using (StreamWriter outputFile = new StreamWriter(Path.Combine(docPath, "WriteLines.txt")))
        {

                outputFile.WriteLine(line);
        }
        foreach (List<Object> readLogData in readLogResponse.Output)
        {
            foreach (object element in readLogData)
            {
                archiveString += element.ToString();
            }
                
        }


        Assert.True(true); */
    }
    [Fact]
    public async void ArchivalServiceShould_DeleteArchivedLogsFromLogTable()
    {
        var archivalService = new ArchivalService();

        var archivalResponse = await archivalService.Archive(DateTime.Today);

        Assert.True(archivalResponse.HasError == false);
    }
    [Fact]
    public void ArchivalServiceShould_UploadCompressedFiletoS3Bucket()
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