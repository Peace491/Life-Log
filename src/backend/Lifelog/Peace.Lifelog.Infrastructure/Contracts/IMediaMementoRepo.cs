namespace Peace.Lifelog.Infrastructure;

using DomainModels;

public interface IMediaMementoRepo
{
    Task<Response> UploadMediaMemento(int lliId, byte[] binary);
    Task<Response> DeleteMediaMemento(int lliId);
    Task<Response> GetAllUserImages(string userhash);
    Task<Response> UploadMediaMementosFromCSV(List<List<string>> csv);
}
