namespace Peace.Lifelog.LLI;

using DomainModels;

public interface ICreateLLI
{
    Task<Response> CreateLLI(string userHash, LLI lli);
}
