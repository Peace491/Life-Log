using DomainModels;

namespace Peace.Lifelog.Infrastructure;

public interface ILocationRecommendationRepo
{
    public Task<Response> ReadAllUserPinInDB(string UserHash);
    public Task<Response> GetPinId(object lat, object lng); 
}
