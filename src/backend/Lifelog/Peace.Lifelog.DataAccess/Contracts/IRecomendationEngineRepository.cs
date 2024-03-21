using DomainModels;

namespace Peace.Lifelog.DataAccess;

public interface IRecomendationEngineRepository
{
    Task<Response> GetNumRecs(string userHash, int numRecs, CancellationToken cancellationToken = default);
}
