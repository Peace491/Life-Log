namespace Peace.Lifelog.LLI;

using DomainModels;
public interface IDeleteLLI
{
    Task<Response> DeleteLLI(string userHash, LLI lli);
}
