namespace Peace.Lifelog.MotivationalQuote;

using DomainModels;

public interface ICheckTime
{
    Task<Response> CheckTime(Phrase phrase);
    
}