namespace Peace.Lifelog.RecSummaryService;

using DomainModels;
public interface IUpdateUserRecSummary
{
    Task<Response> updateUserRecSummary(string userHash);
}
