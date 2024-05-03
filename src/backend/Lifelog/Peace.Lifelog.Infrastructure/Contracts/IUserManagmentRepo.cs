using DomainModels;

namespace Peace.Lifelog.Infrastructure;

public interface IUserManagmentRepo
{
    public Task<Response> DeletePersonalIdentifiableInformation(string userHash);
    public Task<Response> ViewPersonalIdentifiableInformation(string userHash);
}
