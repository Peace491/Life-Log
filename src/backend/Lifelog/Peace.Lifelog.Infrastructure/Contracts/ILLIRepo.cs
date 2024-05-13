using DomainModels;

namespace Peace.Lifelog.Infrastructure;

public interface ILLIRepo
{
    public Task<Response> CreateLLI(string userHash, LLIDB lli);
    public Task<Response> ReadAllLLI(string userHash);
    public Task<Response> ReadLLICompletionStatus(string userHash, string title, string? lliId);
    public Task<Response> ReadMostCommonLLICategory();
    public Task<Response> ReadNumberOfLLI(int period);
    public Task<Response> ReadMostExpensiveLLI(int period);
    public Task<Response> UpdateLLIRecurrenceStatus(string userHash);
    public Task<Response> UpdateLLI(string userHash, LLIDB lli);
    public Task<Response> DeleteLLI(string userHash, string lliId);

}
