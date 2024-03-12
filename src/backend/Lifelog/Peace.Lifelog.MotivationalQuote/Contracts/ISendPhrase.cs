namespace Peace.Lifelog.MotivationalQuote;

using DomainModels;

public interface ISendPhrase
{
    Task<Response> SendPhrase(Phrase phrase);
    
}