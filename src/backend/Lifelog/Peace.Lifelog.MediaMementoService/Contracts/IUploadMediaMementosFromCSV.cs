namespace Peace.Lifelog.MediaMementoService;
using DomainModels;
using Peace.Lifelog.Security;

public interface IUploadMediaMementosFromCSV
{
    Task<Response> UploadMediaMementosFromCSV(string userHash, List<List<string>> CSVMatrix, AppPrincipal? appPrincipal);
}
