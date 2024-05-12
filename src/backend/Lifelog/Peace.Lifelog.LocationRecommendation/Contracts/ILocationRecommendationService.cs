using DomainModels;

namespace Peace.Lifelog.LocationRecommendation;

public interface ILocationRecommendationService
{
    public Task<Response> GetRecommendation(string UserHash); //GetRecommendationRequest getRecommendationRequest
    public Task<Response> ViewRecommendation(ViewRecommendationRequest viewRecommendationRequest);
}
