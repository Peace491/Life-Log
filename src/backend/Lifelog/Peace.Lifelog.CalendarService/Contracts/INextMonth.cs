namespace Peace.Lifelog.CalendarService;

using DomainModels;

public interface INextMonth
{
    Task<Response> NextMonth(string userHash);

}