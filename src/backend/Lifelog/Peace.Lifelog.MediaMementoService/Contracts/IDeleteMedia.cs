using DomainModels;
using Peace.Lifelog.Security;
namespace Peace.Lifelog.MediaMementoService;

public interface IDeleteMedia
{
    Task<Response> DeleteMediaMemento(int lliId, AppPrincipal? appPrincipal);
}
