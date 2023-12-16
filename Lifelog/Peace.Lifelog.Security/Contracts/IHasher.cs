using DomainModels;

namespace Peace.Lifelog.Security;

public interface IHasher
{
    Response Hasher(string plaintext);
}
