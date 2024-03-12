using DomainModels;
namespace Peace.Lifelog.Security;

public interface IOTPService
{
    Task<Response> generateOneTimePassword(string userHash);
}
