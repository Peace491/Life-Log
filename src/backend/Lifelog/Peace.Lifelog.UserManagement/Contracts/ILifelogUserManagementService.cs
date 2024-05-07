using DomainModels;

namespace Peace.Lifelog.UserManagement;

public interface ILifelogUserManagementService : ICreateLifelogUser, IDeleteLifelogUser, IModifyLifelogUser, IRecoverLifelogUser, IViewPersonalIdentifiableInformation
{
    public Task<string> GetUserIdFromUserHash(string userHash);
    public Task<Response> ViewPersonalIdentifiableInformation(string userHash);
}
