namespace Peace.Lifelog.RE;

using DomainModels;
public interface IGetRecs
{
    Task<Response> getRecs(string userhash);
}
