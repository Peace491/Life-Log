using DomainModels;

namespace Peace.Lifelog.Security;

public interface IHasher
{
    Task<Response> Hasher(string plaintext);
}
