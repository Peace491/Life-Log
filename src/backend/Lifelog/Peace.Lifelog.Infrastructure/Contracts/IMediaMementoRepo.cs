namespace Peace.Lifelog.Infrastructure;

using DomainModels;

public interface IMediaMementoRepo
{
    Task<Response> UploadMediaMemento(int lliId, string binary);
    Task<Response> DeleteMediaMemento(int lliId);
}
