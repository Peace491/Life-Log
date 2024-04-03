using DomainModels;

namespace Peace.Lifelog.Infrastructure;

public interface IUserFormRepo
{
    public Task<Response> CreateUserFormInDB(
        string userHash,
        int mentalHealthRating, int physicalHealthRating, int outdoorRating,
        int sportRating, int artRating, int hobbyRating,
        int thrillRating, int travelRating, int volunteeringRating,
        int foodRating
    );
    public Task<Response> ReadUserFormCompletionStatusInDB(string userHash);
    public Task<Response> ReadUserFormCategoriesRankingInDB(string userHash);
    public Task<Response> UpdateUserFormInDB(
        string userHash,
        int mentalHealthRating, int physicalHealthRating, int outdoorRating,
        int sportRating, int artRating, int hobbyRating,
        int thrillRating, int travelRating, int volunteeringRating,
        int foodRating
    );
}
