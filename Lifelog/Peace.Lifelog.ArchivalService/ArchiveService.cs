using DomainModels;
using System.IO;
using System.IO.Compression;
using Peace.Lifelog.DataAccess;
using Peace.Lifelog.Logging;

namespace Peace.Lifelog.ArchivalService;

public class ArchivalService : IArchive
{
    public Response Archive(DateTime dateTime)
    {
        var createOnlyDAO = new CreateDataOnlyDAO();
        var logTarget = new LogTarget(createOnlyDAO);
        var logger = new Logging.Logging(logTarget);

        var readOnlyDAO = new ReadDataOnlyDAO();
        var deleteDataDAO = new DeleteDataOnlyDAO(); 

        var response = new Response();

        response.HasError = true;
        response.Output = ["not implemented"];

        // Read Logs older than 30 days old
        var readArchivableLogs = "SELECT * FROM LifelogDB.Logs WHERE LogTimestamp < DATE_SUB(CURRENT_DATE, INTERVAL 30 DAY);";

        // Put all info from each log into a string
        var archivalString = "";
        

        // Write string result to .txt file
        var filePath = "../"; 
        var finalPath = "../";

        File.WriteAllText(filePath, archivalString);

        // Compress resulting .txt file & save to final location off db (gold plating: s3 bucket)
        CompressFile(filePath, finalPath);

        // Offload/Delete logs older than 30 days old
        var offloadArchivedLogs = "DELETE FROM LifelogDB.Logs WHERE LogTimestamp < DATE_SUB(CURRENT_DATE, INTERVAL 30 DAY);";

        // Log archival operation 
        // logger.CreateLog(TABLE, hash, level, category, message);
        
        return response;
    }
    static void CompressFile(string sourcePath, string destinationPath)
    {

    }
}
