namespace Peace.Lifelog.RecEngineService;

using DomainModels;
using Peace.Lifelog.Security;

public interface IGetNumRecs
{
    Task<Response> getNumRecs(AppPrincipal appPrincipal, int numRecs);
}
