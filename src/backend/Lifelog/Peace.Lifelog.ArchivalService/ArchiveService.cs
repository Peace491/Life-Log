using System.ComponentModel.Design;
using DomainModels;
using System.IO.Compression;
using Peace.Lifelog.DataAccess;
using Peace.Lifelog.Logging;

namespace Peace.Lifelog.ArchivalService;

public class ArchivalService : IArchive
{
    public async Task<Response> Archive(DateTime dateTime)
    {
        // Init response 
        var response = new Response();
        response.HasError = true;

        // Init nessesary DAOS
        var readDAO = new ReadDataOnlyDAO();
        var deleteDAO = new DeleteDataOnlyDAO();

        // Init Logger keep having logger init issues
        var createOnlyDAO = new CreateDataOnlyDAO();
        var logTarget = new LogTarget(createOnlyDAO);
        // var logger = new Logging(logTarget);

        // Init SQL 
        string SELECT = "SELECT "; 
        string DELETE = "DELETE "; 
        string validLogs = "* FROM Logs WHERE LogTimestamp > DATE_SUB(CURRENT_DATE, INTERVAL 30 DAY);";

        // Get the target folder
        DirectoryInfo currentDirectory = new DirectoryInfo(Environment.CurrentDirectory);
        for (int i = 0; i < 7; i++)
        {
            currentDirectory = currentDirectory.Parent;
        }
        DirectoryInfo targetDirectory = new DirectoryInfo(Path.Combine(currentDirectory.FullName, "Project Documents"));
        DirectoryInfo levelDownLocation = new DirectoryInfo(Path.Combine(targetDirectory.FullName, "Archive"));

        DirectoryInfo txtLocation = new DirectoryInfo(Path.Combine(levelDownLocation.FullName, "Archive txt"));
        DirectoryInfo zipLocation = new DirectoryInfo(Path.Combine(levelDownLocation.FullName, "Archive zip"));



        // Get date for archive txt and zip name
        DateTime today = DateTime.Today;
        string year = today.Year.ToString();
        string month = today.Month.ToString("00");

        string txtName = $"{year}_{month}archive.txt";
        string archiveName = $"{year}_{month}archive.zip"; 

        // Read Logs older than 30 days old
        var readResponse = await readDAO.ReadData(SELECT + validLogs, 10000);
       // var path = Path.Combine(txtLocation.FullName, txtName);

        // Write result to .txt file
        // using (StreamWriter sw = File.CreateText(path))
        // /Users/jackpickle/dev/repos/Life-Log/Project Documents/Archive
        // /Users/jackpickle/dev/repos/Life-Log/src/backend/Project Documents/Archive/2024_02archive.txt

        using (StreamWriter outputFile = new StreamWriter(Path.Combine(txtLocation.FullName, txtName)))
        {
            foreach (List<Object> readLogData in readResponse.Output)
            {
                string archiveFLine = ""; 
                foreach (object element in readLogData)
                {
                    archiveFLine += element.ToString() + " ";
                } 
                outputFile.WriteLine(archiveFLine);
            }
            outputFile.Close();
        }
        

        // Compress resulting .txt file

        // Read txt
        using (FileStream sourceFileStream = File.OpenRead(Path.Combine(txtLocation.FullName, txtName)))
        {
            // access archive loc
            using (FileStream compressedFileStream = File.Create(Path.Combine(zipLocation.FullName, archiveName)))
            {
                // uze gzipstream to compress file to archive location stream
                using (GZipStream gzipStream = new GZipStream(compressedFileStream, CompressionMode.Compress))
                {
                    sourceFileStream.CopyTo(gzipStream);
                }
            }
        }     

        // Offload/Delete logs older than 30 days old
        // var deleteResponse = await readDAO.ReadData(DELETE + validLogs);

        // TODO: Log archival operation 

        response.HasError = false; 
        return response;
    }
}
