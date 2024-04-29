namespace Peace.Lifelog.MediaMementoService;
using DomainModels;

public interface IUploadMediaMementosFromCSV
{
    Task<Response> UploadMediaMementosFromCSV(string userHash, List<List<string>> CSVMatrix);
}
