using DomainModels;
using Peace.Lifelog.UserManagementTest;

namespace Peace.Lifelog.UserManagement;

public interface IModifyLifelogUser
{
    public Task<Response> ModifyLifelogUser(LifelogProfileRequest profileRequest);
}
