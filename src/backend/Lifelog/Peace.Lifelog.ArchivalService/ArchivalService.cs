using System.ComponentModel.Design;
using DomainModels;
using System.IO;
using Peace.Lifelog.DataAccess;
using Peace.Lifelog.Logging;
using Amazon.S3;
using Amazon.S3.Transfer;
using Peace.Lifelog.Infrastructure;
using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;
using Ionic.Zip;

using System;
using System.IO;
using System.Text.Json;
using System.Threading;

namespace Peace.Lifelog.ArchivalService;

public class ArchivalService : IArchive
{
    // need to configify these
    LifelogConfig lifelogConfig = LifelogConfig.LoadConfiguration(); 
    public async Task<Response> ArchiveFileToS3()
    {
        var response = new Response();
        try
        {
            // Create an instance of AmazonS3Client with your AWS credentials
            var amazonS3Client = new AmazonS3Client(lifelogConfig.AccessKeyId, lifelogConfig.SecretAccessKey, Amazon.RegionEndpoint.USWest1);

            // Create TransferUtility instance
            var transferUtility = new TransferUtility(amazonS3Client);

            var dateTime = DateTime.Now;
            string dt = dateTime.ToLongDateString();

            var archivalRepo = new ArchiveRepo(new ReadDataOnlyDAO(), new DeleteDataOnlyDAO());

            response = await archivalRepo.SelectArchivableLogs();

            if (response.HasError)
            {
                throw new Exception("Error selecting logs to archive");
            }

            // Compose the logs to a file
            string fPath = await ComposeLogsToFileAsync(response);

            // Zip the file
            string stringWithoutLastFourCharacters = fPath.Substring(0, fPath.Length - 4);
            string zipPath = stringWithoutLastFourCharacters + ".zip";

            // Delete the existing zip file if it exists
            if (File.Exists(zipPath))
            {
                File.Delete(zipPath);
            }

            string zipFilePath = Path.ChangeExtension(fPath, ".zip");
                using (ZipFile zip = new ZipFile())
                {
                    // Add the text file to the zip correctly
                    zip.AddFile(fPath, "");
                    zip.Save(zipFilePath);
                }

            string txtName = $"{dt}archive.zip"; // Change the file name to reflect the zip format
                                                 // Create the archival file
            string s3Key = "lifelog-archive/" + txtName;

            // Upload the zip file to S3
            await transferUtility.UploadAsync(zipFilePath, lifelogConfig.S3BucketName, s3Key);

            // Delete the local zip file after uploading
            File.Delete(zipPath);

            // Delete the local txt file after uploading
            File.Delete(fPath);

            // Delete the logs from the database
            response = await archivalRepo.DeleteArchivedLogs();
            
            if (response.HasError)
            {
                throw new Exception("Error deleting logs after archiving");
            }
        }
        catch (AmazonS3Exception ex)
        {
            // Handle S3 exception
            Console.WriteLine($"Error uploading file to S3: {ex.Message}");
            throw;
        }
        catch (Exception ex)
        {
            // Handle other exceptions
            Console.WriteLine($"Error uploading file to S3: {ex.Message}");
            throw;
        }
        return new Response();
    }
    public async Task<string> ComposeLogsToFileAsync(Response response)
    {
        string directory = AppDomain.CurrentDomain.BaseDirectory;
        string filePath = Path.Combine(directory, "logs.txt");

        using (StreamWriter writer = File.CreateText(filePath))
        {
            if (response.Output != null)
            {
                foreach (var outputItem in response.Output)
                {
                    await writer.WriteLineAsync(JsonConvert.SerializeObject(outputItem));
                }
            }
        }

        return filePath;
    }
    public string ComposeLogsToFile(Response response)
    {
        string directory = AppDomain.CurrentDomain.BaseDirectory;
        string filePath = Path.Combine(directory, "logs.txt");

        using (StreamWriter writer = File.CreateText(filePath))
        {
            if (response.Output != null)
            {
                writer.WriteLineAsync("Logs:");
                foreach (var outputItem in response.Output)
                {
                    writer.WriteLineAsync(JsonConvert.SerializeObject(outputItem));
                }
            }
        }
        return filePath;
    }

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
                    // using (GZipStream gzipStream = new GZipStream(compressedFileStream, CompressionMode.Compress))
                    // {
                    //     sourceFileStream.CopyTo(gzipStream);
                    // }
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
