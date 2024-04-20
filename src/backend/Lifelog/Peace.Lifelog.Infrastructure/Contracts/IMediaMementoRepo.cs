namespace Peace.Lifelog.Infrastructure;

using DomainModels;

public interface IMediaMementoRepo
{
    Task<Response> UploadMediaMemento(int lliId, byte[] binary);
    Task<Response> DeleteMediaMemento(int lliId);
}
