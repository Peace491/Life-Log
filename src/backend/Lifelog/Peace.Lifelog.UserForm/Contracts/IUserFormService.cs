using DomainModels;
using Peace.Lifelog.Security;

namespace Peace.Lifelog.UserForm;

public interface IUserFormService
{
    public Task<Response> CreateUserForm(CreateUserFormRequest createUserFormRequest);
    public Task<Response> UpdateUserForm(UpdateUserFormRequest updateUserFormRequest);
    public Task<bool> IsUserFormCompleted(string userHash);

    public Task<Response> GetUserFormRanking(AppPrincipal principal);
}
