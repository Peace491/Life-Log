namespace Peace.Lifelog.CalendarService;

using DomainModels;

public interface IPrevMonth
{
    Task<Response> PrevMonth(string userHash);
}