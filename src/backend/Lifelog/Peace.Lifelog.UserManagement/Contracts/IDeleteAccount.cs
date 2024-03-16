namespace Peace.Lifelog.UserManagement;

using DomainModels;
/// <summary>
/// Delete account in DB
/// </summary>
public interface IDeleteAccount
{
    public Task<Response> DeleteAccount(IUserAccountRequest userAccountRequest);

}
