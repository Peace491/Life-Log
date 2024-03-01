namespace Peace.Lifelog.MotivationalQuote;

using DomainModels;

public interface IGetPhrase
{
    Task<Response> GetPhrase(Phrase phrase);
    Task<Response> CheckTime(Phrase phrase);
    Task<Response> CheckPhrase(Phrase phrase);
    Task<Response> CheckQuote(Phrase phrase);
    Task<Response> CheckAuthor(Phrase phrase)
    Task<Response> SendPhrase(Phrase phrase)
    
}