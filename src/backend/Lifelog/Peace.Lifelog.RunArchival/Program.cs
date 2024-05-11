using Peace.Lifelog.ArchivalService;

class Program
{
    static async Task Main(string[] args)
    {
        // Assume 'ArchivalService' implements a method 'ArchiveFileToS3'
        var service = new ArchivalService();
        Console.WriteLine("Archiving Logs to S3...");
        _ = await service.ArchiveFileToS3("logs");
        Console.WriteLine("Logs archived successfully!");
    }
}