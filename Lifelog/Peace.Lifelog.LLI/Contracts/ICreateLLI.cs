namespace Peace.Lifelog.LLI;

using DomainModels;

public interface ICreateLLI
{
    Task<Response> CreateLLI(LLI lli);
}
