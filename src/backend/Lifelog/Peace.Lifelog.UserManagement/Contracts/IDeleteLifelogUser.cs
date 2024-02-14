using DomainModels;
using Peace.Lifelog.UserManagementTest;

namespace Peace.Lifelog.UserManagement;

public interface IDeleteLifelogUser
{
    public Task<Response> DeleteLifelogUser(LifelogAccountRequest accountRequest, LifelogProfileRequest profileRequest);
}
