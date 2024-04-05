namespace Peace.Lifelog.RecEngineService;

using DomainModels;
using Peace.Lifelog.Security;

public interface IRecNumLLI
{
    Task<Response> RecNumLLI(AppPrincipal appPrincipal, int numRecs);
}
