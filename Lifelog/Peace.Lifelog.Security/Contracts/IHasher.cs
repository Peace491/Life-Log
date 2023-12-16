using DomainModels;

namespace Peace.Lifelog.Security;

public interface IHasher
{
    string Hasher(string plaintext);
}
