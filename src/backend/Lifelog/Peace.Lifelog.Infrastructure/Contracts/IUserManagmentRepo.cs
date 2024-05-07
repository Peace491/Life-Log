using DomainModels;

namespace Peace.Lifelog.Infrastructure;

public interface IUserManagmentRepo
{
    public Task<Response> GetUserHashFromUserId(string userId);
    public Task<Response> GetUserIdFromUserHash(string userHash);
    public Task<Response> CreateLifelogUserRoleInDB(string userId, string role);
    public Task<Response> CreateLifelogUserOTPInDB(string userHash);
    public Task<Response> CreateLifelogAuthenticationInDB(string userId, string userHash, string role);
    public Task<Response> GetAccountRecoveryRequestRoot();
    public Task<Response> GetAccountRecoveryRequestNotRoot();
    public Task<Response> CreateAccountRecoveryRequest(string userId);
    public Task<Response> DeleteAccountRecoveryRequest(string userId);
    public Task<Response> CreateRecSummaryForUser(string userHash);
    public Task<Response> DeletePersonalIdentifiableInformation(string userHash);
    public Task<Response> ViewPersonalIdentifiableInformation(string userHash);
}
