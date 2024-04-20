using DomainModels;
namespace Peace.Lifelog.MediaMementoService;

public interface IUploadMedia
{
    Task<Response> UploadMediaMemento(string userhash, int lliId, byte[] binary);
}
