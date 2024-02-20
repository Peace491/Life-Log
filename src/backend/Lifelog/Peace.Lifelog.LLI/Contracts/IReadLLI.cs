namespace Peace.Lifelog.LLI;

using DomainModels;

public interface IReadLLI
{
    Task<Response> GetAllLLIFromUser(string userHash, int pageNumber = 0);
    Task<Response> GetSingleLLIFromUser(string userHash, LLI lli);
}
