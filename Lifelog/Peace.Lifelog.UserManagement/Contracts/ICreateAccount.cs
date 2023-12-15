namespace Peace.Lifelog.UserManagement;

using DomainModels;

public interface ICreateAccount
{
    public Task<Response> CreateAccount(BaseUserAccount userAccount);

}
