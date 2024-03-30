using DomainModels;
using Peace.Lifelog.Security;

namespace Peace.Lifelog.UserForm;

public interface IUserFormValidation
{
    public Response validateCreateUserFormRequest(
        Response respones, CreateUserFormRequest createUserFormRequest
    );

}
