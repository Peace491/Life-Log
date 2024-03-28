using DomainModels;

namespace Peace.Lifelog.DataAccess;

public interface IMotivationalQuoteRepository
{
    Task<Response> CheckTodayQuote();
    Task<Response> GetTodayQuote(string todayID);
    Task<Response> LastEntryOfTheDay();
    Task<Response> LastEntryQuote(string currentID);
    Task<Response> IsLastID();
    Task<Response> NewEntryQuote(int currentIDNum);
    Task<Response> InsertQuote(string newID);
    Task<Response> PlaceholderRetrieve();
    Task<Response> QuoteChecker(string quote);
    Task<Response> AuthorChecker(string author);
    Task<Response> MonthChecker(string currentID);
}