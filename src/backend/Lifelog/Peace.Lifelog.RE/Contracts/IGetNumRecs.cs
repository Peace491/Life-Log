namespace Peace.Lifelog.RecEngineService;

using DomainModels;
public interface IGetNumRecs
{
    Task<Response> getNumRecs(string userhash, int numRecs);
}
