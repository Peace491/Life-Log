namespace Peace.Lifelog.MotivationalQuote;

using DomainModels;

public interface ICheckQuote
{
    Task<Response> CheckQuote(Phrase phrase);
    
}