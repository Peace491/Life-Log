namespace Peace.Lifelog.UserManagement;

using DomainModels;

public interface IDeleteAccount
{
    public Task<Response> DeleteAccount(IUserAccountRequest userAccountRequest);

}
