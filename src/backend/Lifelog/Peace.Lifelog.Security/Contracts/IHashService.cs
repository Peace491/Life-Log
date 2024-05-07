using DomainModels;

namespace Peace.Lifelog.Security;
/// <summary>
/// IHasher Interface
/// </summary>
public interface IHashService
{
    /// <summary>
    /// Will hash string input plaintext
    /// </summary>
    /// <param name="plaintext"></param>
    /// <returns></returns>
    Response Hasher(string plaintext);
}
