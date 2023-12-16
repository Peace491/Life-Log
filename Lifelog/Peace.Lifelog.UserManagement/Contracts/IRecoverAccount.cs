namespace Peace.Lifelog.UserManagement;

using DomainModels;

public interface IRecoverAccount
{
    public Task<Response> RecoverMfaAccount(IMultifactorAccountRequest accountRequest);

    public Task<Response> RecoverStatusAccount(IStatusAccountRequest accountRequest);

}
