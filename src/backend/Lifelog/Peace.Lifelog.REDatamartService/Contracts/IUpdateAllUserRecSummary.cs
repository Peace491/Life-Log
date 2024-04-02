namespace Peace.Lifelog.RecSummaryService;

using DomainModels;
public interface IUpdateAllUserRecSummary
{
    Task<Response> updateAllUserRecSummary();
}
