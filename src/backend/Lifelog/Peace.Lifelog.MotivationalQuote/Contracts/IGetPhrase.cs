namespace Peace.Lifelog.MotivationalQuote;

using DomainModels;

public interface IGetPhrase
{
    Task<Response> GetPhrase();
}