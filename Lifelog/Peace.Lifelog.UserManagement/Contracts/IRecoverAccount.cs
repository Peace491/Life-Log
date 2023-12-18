namespace Peace.Lifelog.UserManagement;

using DomainModels;
/// <summary>
/// Recover account in DB using MFA and Status
/// </summary>
public interface IRecoverAccount
{
    public Task<Response> RecoverMfaAccount(IMultifactorAccountRequest accountRequest);

    public Task<Response> RecoverStatusAccount(IStatusAccountRequest accountRequest);

}
