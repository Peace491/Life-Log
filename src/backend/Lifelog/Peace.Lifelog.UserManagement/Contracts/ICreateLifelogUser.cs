using DomainModels;
using Peace.Lifelog.UserManagementTest;

namespace Peace.Lifelog.UserManagement;

public interface ICreateLifelogUser
{
    public Task<Response> CreateLifelogUser(LifelogAccountRequest accountRequest, LifelogProfileRequest profileRequest);
}
