namespace Peace.Lifelog.UserManagement;

using DomainModels;
/// <summary>
/// Create Account in DB
/// </summary>
public interface ICreateAccount
{
    public Task<Response> CreateAccount(IUserAccountRequest userAccountRequest);

}
