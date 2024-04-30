using DomainModels;
using Peace.Lifelog.Security;
namespace Peace.Lifelog.MediaMementoService;

public interface IGetAllUserImages
{
    Task<Response> GetAllUserImages(string userhash, AppPrincipal? appPrincipal);
}
