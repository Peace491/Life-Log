using DomainModels;

namespace Peace.Lifelog.UserManagement;

public interface ILifelogUserManagementService : ICreateLifelogUser, IDeleteLifelogUser, IModifyLifelogUser, IRecoverLifelogUser, IDeletePersonalIdentifiableInformation, IViewPersonalIdentifiableInformation
{
    public Task<string> GetUserIdFromUserHash(string userHash);
    public Task<Response> DeletePersonalIdentifiableInformation(string userHash);
    public Task<Response> ViewPersonalIdentifiableInformation(string userHash);
}
