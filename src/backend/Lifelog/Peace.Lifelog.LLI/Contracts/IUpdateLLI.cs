namespace Peace.Lifelog.LLI;

using DomainModels;

public interface IUpdateLLI
{
    Task<Response> UpdateLLI(string userHash, LLI lli);

}
