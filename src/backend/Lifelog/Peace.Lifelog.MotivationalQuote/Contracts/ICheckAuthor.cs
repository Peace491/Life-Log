namespace Peace.Lifelog.MotivationalQuote;

using DomainModels;

public interface ICheckAuthor
{
    Task<Response> CheckAuthor(Phrase phrase);
}