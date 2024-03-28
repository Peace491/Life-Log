namespace Peace.Lifelog.REDatamartService;

using DomainModels;
public interface IREDatamart
{
    Task<Response> updateRecommendationDataMartForUser(string userHash);
    Task<Response> updateRecommendationDataMartForAllUsers();
}
