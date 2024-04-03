using DomainModels;
using Peace.Lifelog.Security;

namespace Peace.Lifelog.UserForm;

public interface IUserFormValidation
{
    public Response ValidateUserFormRequest(
        Response response, IUserFormRequest userFormRequest, string requestType
    );

}
