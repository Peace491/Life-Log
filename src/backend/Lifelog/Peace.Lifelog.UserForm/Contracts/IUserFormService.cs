using DomainModels;
using Peace.Lifelog.Security;

namespace Peace.Lifelog.UserForm;

public interface IUserFormService
{
    public Task<Response> createUserForm(
        CreateUserFormRequest createUserFormRequest
    );
    public Task<Response> updateUserForm();
}
