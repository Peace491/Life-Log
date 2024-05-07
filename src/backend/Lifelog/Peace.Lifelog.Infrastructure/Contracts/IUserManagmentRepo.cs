using DomainModels;

namespace Peace.Lifelog.Infrastructure;

public interface IUserManagmentRepo
{
    public Task<Response> GetUserHashFromUserId(string userId);
    public Task<Response> GetUserIdFromUserHash(string userHash);
    public Task<Response> CreateRecSummaryForUser(string userHash);
    public Task<Response> DeletePersonalIdentifiableInformation(string userHash);
    public Task<Response> ViewPersonalIdentifiableInformation(string userHash);
}
