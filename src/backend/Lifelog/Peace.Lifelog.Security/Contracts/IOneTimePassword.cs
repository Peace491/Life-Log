using DomainModels;
namespace Peace.Lifelog.Security;

public interface IOneTimePassword
{
    Response generateOneTimePassword(string userHash);
}
