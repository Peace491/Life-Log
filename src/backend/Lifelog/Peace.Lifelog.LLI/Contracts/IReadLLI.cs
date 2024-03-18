namespace Peace.Lifelog.LLI;

using DomainModels;

public interface IReadLLI
{
    Task<Response> GetAllLLIFromUser(string userHash);
}
