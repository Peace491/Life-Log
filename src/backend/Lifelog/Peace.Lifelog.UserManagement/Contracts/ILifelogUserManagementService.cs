using DomainModels;

using Peace.Lifelog.Security;
using Peace.Lifelog.UserManagementTest;
namespace Peace.Lifelog.UserManagement;

public interface ILifelogUserManagementService : ICreateLifelogUser, IDeleteLifelogUser, IModifyLifelogUser, IRecoverLifelogUser, IViewPersonalIdentifiableInformation
{
    public Task<string> GetUserIdFromUserHash(string userHash);
    public Task<Response> ViewPersonalIdentifiableInformation(string userHash);
    public Task<Response> GetAllNormalUsers(AppPrincipal principal);
    public Task<Response> GetAllNonRootUsers(AppPrincipal principal);
    public Task<Response> UpdateRoleToAdmin(AppPrincipal principal, string userId);
    public Task<Response> UpdateRoleToNormal(AppPrincipal principal, string userId);
    public Task<Response> CheckSuccessfulReg(LifelogAccountRequest acc, LifelogProfileRequest profile);
}
