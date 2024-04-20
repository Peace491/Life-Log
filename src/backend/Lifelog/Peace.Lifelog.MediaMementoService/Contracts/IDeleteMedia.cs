using DomainModels; 
namespace Peace.Lifelog.MediaMementoService;

public interface IDeleteMedia
{
    Task<Response> DeleteMediaMemento(string userhash, int lliId);
}
