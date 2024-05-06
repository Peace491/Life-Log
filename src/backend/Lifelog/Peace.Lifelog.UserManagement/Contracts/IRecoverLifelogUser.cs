namespace Peace.Lifelog.UserManagement;

using DomainModels;
using Peace.Lifelog.Security;
using Peace.Lifelog.UserManagementTest;

/// <summary>
/// Recover account in DB using Status
/// </summary>
public interface IRecoverLifelogUser
{
    public Task<Response> CreateRecoveryAccountRequest(LifelogAccountRequest lifelogAccountRequest);
    public Task<Response> GetRecoveryAccountRequests(AppPrincipal principal);
    public Task<Response> RecoverLifelogUser(AppPrincipal principal, LifelogAccountRequest lifelogAccountRequest);

}
