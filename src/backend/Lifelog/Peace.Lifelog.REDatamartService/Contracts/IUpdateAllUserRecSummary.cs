namespace Peace.Lifelog.RecSummaryService;

using DomainModels;
using Peace.Lifelog.Security;
public interface IUpdateAllUserRecSummary
{
    Task<Response> UpdateAllUserRecSummary(AppPrincipal principal);
}
