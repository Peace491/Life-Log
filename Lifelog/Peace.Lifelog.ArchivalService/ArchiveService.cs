using DomainModels;

namespace Peace.Lifelog.ArchivalService;

public class ArchivalService : IArchive
{
    public Response Archive(DateTime dateTime)
    {
        var response = new Response();

        response.HasError = true;
        response.Output = ["not implemented"];

        // Read Logs older than 30 days old

        // Write result to .txt file

        // Compress resulting .txt file

        // Write to s3 bucket

        // Offload/Delete logs older than 30 days old

        // Log archival operation 

        return response;
    }
}
