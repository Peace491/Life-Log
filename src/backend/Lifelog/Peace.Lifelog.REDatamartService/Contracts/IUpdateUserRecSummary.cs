namespace Peace.Lifelog.RecSummaryService;

using DomainModels;
using Peace.Lifelog.Security;

public interface IUpdateUserRecSummary
{
    Task<Response> UpdateUserRecSummary(AppPrincipal principal);
}
