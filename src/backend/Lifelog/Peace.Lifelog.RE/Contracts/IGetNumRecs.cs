namespace Peace.Lifelog.RecEngineService;

using DomainModels;
public interface IRecEngineService
{
    Task<Response> getNumRecs(string userhash, int numRecs);
}
