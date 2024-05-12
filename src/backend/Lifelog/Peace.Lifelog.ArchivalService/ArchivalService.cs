using DomainModels;
using Peace.Lifelog.DataAccess;
using Amazon.S3;
using Amazon.S3.Transfer;
using Peace.Lifelog.Infrastructure;
using Newtonsoft.Json;
using Ionic.Zip;

namespace Peace.Lifelog.ArchivalService;

public class ArchivalService : IArchive
{
    LifelogConfig lifelogConfig = LifelogConfig.LoadConfiguration(); 
    public async Task<Response> ArchiveFileToS3(string tableName)
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

            response = await archivalRepo.SelectArchivableLogs(tableName);
            Console.WriteLine(response.ErrorMessage);

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
            response = await archivalRepo.DeleteArchivedLogs(tableName);
            
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
        return response;
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
}
