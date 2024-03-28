namespace Peace.Lifelog.DataAccess;

using DomainModels;

public interface ISummaryRepository
{
    Task<Response> GetAllUserHash(CancellationToken cancellationToken = default);
    Task<Response> GetUserForm(string userHash, CancellationToken cancellationToken = default);
    Task<Response> GetNumUserLLI(string userHash, int? limit, CancellationToken cancellationToken = default);
    Task<Response> GetMostPopularCategory(CancellationToken cancellationToken = default);
    Task<Response> UpdateUserDataMart(string userHash, string category1, string category2, CancellationToken cancellationToken = default);

}
