namespace Peace.Lifelog.CalendarService;

using DomainModels;
using Peace.Lifelog.LLI;

public interface ICreateLLIWithCalendar
{
    Task<Response> CreateLLIWithCalendar(string userHash, LLI lli);

}