using DomainModels;
namespace Peace.Lifelog.MediaMementoService;

public interface IGetAllUserImages
{
    Task<Response> GetAllUserImages(string userhash);
}
