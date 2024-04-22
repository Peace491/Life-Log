using System.ComponentModel.Design;
using DomainModels;
using System.IO.Compression;
using Peace.Lifelog.DataAccess;
using Peace.Lifelog.Logging;

namespace Peace.Lifelog.ArchivalService;

public class ArchivalService : IArchive
{
    public async Task<Response> Archive(DateTime dateTime, string table)
    {
        // Init response 
        var response = new Response();
        response.HasError = true;

        if (dateTime != DateTime.Today)
        {
            response.ErrorMessage = "Invalid Date Passed to archival";
            return response;
        }

        // Init nessesary DAOS
        var createDataOnlyDAO = new CreateDataOnlyDAO();
        var readDAO = new ReadDataOnlyDAO();
        var deleteDAO = new DeleteDataOnlyDAO();

        // Init Logger keep having logger init issues
        var readDataOnlyDAO = new ReadDataOnlyDAO();
        var logTarget = new LogTarget(createDataOnlyDAO, readDataOnlyDAO);
        var logger = new Logging.Logging(logTarget);

        // Init SQL 
        string SELECT = "SELECT *"; 
        string DELETE = "DELETE "; 
        string validLogs = $"FROM {table} WHERE Timestamp < DATE_SUB(CURRENT_DATE, INTERVAL 30 DAY);";

        // Init archive filenames from input
        string dt = dateTime.ToLongDateString();

        string txtName = $"{dt}archive.txt";
        string archiveName = $"{dt}archive.zip"; 
        

        string level = "ERROR";
        string category = "View";

        try // Protect against failure
        {
            // Get the target folder
            DirectoryInfo currentDirectory = new DirectoryInfo(Environment.CurrentDirectory);
        
            for (int i = 0; i < 7; i++)
            {
                currentDirectory = currentDirectory.Parent!;
            }

            DirectoryInfo targetDirectory = new DirectoryInfo(Path.Combine(currentDirectory.FullName, "Project Documents"));
            DirectoryInfo levelDownLocation = new DirectoryInfo(Path.Combine(targetDirectory.FullName, "Archive"));

            DirectoryInfo txtLocation = new DirectoryInfo(Path.Combine(levelDownLocation.FullName, "Archive txt"));
            DirectoryInfo zipLocation = new DirectoryInfo(Path.Combine(levelDownLocation.FullName, "Archive zip"));

            // Read Logs older than 30 days old
            category = "Persistent Data Store";
            var readResponse = await readDAO.ReadData(SELECT + validLogs, 10000);

            category = "Server";
            // Write result of query to .txt file
            using (StreamWriter outputFile = new StreamWriter(Path.Combine(txtLocation.FullName, txtName)))
            {
                if (readResponse.Output != null)
                {
                    foreach (List<Object> readLogData in readResponse.Output)
                    {
                        string archiveFLine = ""; 
                        foreach (object element in readLogData)
                        {
                            archiveFLine += element.ToString() + " ";
                        } 
                        archiveFLine += ";";
                        outputFile.WriteLine(archiveFLine);
                    }
                }
                
                outputFile.Close();
            }


            // Compress resulting .txt file
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
            category = "Persistent Data Store";
            var deleteResponse = await deleteDAO.DeleteData(DELETE + validLogs);

            // Log archival operation 
            level = "Info";
            category = "View";

            response.HasError = false;
            _ = await logger.CreateLog(table, "TxT3KzlpTG0ExziT6GhXfJDStrAssjrEZjbe14UBfvU=", level, category, response.ErrorMessage);
        } 
        catch (Exception exception)
        {
            response.HasError = true;
            response.ErrorMessage = exception.GetBaseException().Message; 

            // Log on failure
            _ = await logger.CreateLog(table, "TxT3KzlpTG0ExziT6GhXfJDStrAssjrEZjbe14UBfvU=", level, category, response.ErrorMessage);
        }
        
        return response;
    }
}
