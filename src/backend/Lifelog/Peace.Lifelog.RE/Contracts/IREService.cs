namespace Peace.Lifelog.RE;

using DomainModels;
public interface IReService
{
    Task<Response> getNumRecs(string userhash, int numRecs);
}
