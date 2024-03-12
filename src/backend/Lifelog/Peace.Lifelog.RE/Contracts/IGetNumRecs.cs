namespace Peace.Lifelog.RE;

using DomainModels;
public interface IGetNumRecs
{
    Task<Response> getNumRecs(string userhash, int numRecs);
}
