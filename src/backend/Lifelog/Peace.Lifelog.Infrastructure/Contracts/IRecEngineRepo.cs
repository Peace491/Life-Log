namespace Peace.Lifelog.Infrastructure;
using DomainModels;

public interface IRecEngineRepo
{
    Task<Response> GetNumRecs(string userHash, int numRecs, CancellationToken cancellationToken = default);
}
