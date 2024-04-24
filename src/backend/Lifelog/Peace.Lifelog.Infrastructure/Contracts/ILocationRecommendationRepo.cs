using DomainModels;

namespace Peace.Lifelog.Infrastructure;

public interface ILocationRecommendationRepo
{
    public Task<Response> ReadPinInDB(string PinId);
    public Task<Response> ReadLLIInDB(string LLIId);
    public Task<Response> ReadAllUserPinInDB(string UserHash);
    public Task<Response> ReadAllPinForLLIInDB(string LLIId);
    public Task<Response> GetPinId(object lat, object lng); 
    /*public Task<Response> ReadPinInDB(string PinId);
    public Task<Response> ReadLLIInDB(string LLIId);
    public Task<Response> ReadAllUserPinInDB(string UserHash);
    public Task<Response> ReadAllPinForLLIInDB(string LLIId);*/
}
