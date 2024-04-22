using DomainModels;

namespace Peace.Lifelog.LocationRecommendation;

public interface ILocationRecommendationService
{
    public Task<Response> GetRecommendation(GetRecommendationRequest getRecommendationRequest);
    public Task<Response> ViewRecommendation(ViewRecommendationRequest viewRecommendationRequest);
    public Task<Response> ViewPin(ViewPinRequest viewPinRequest);
    public Task<Response> UpdateLog(UpdateLogRequest updateLogRequest);
}
