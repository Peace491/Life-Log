using DomainModels; 
namespace Peace.Lifelog.MediaMementoService;

public interface IDeleteMedia
{
    Task<Response> DeleteMediaMemento(int lliId);
}
