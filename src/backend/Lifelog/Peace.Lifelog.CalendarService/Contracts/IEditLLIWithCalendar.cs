namespace Peace.Lifelog.CalendarService;

using DomainModels;
using Peace.Lifelog.LLI;

public interface IEditLLIWithCalendar
{
    Task<Response> EditLLIWithCalendar(string userHash, LLI lli);
}