namespace Peace.Lifelog.UserManagement;
using DomainModels;

public interface IDeletePersonalIdentifiableInformation
{
    public Task<Response> DeletePersonalIdentifiableInformation(string userHash);
}
