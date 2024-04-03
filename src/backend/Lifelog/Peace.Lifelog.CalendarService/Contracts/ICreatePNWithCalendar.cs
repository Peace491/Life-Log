namespace Peace.Lifelog.CalendarService;

using DomainModels;
using Peace.Lifelog.PersonalNote;

public interface ICreatePNWithCalendar
{
    Task<Response> CreatePNWithCalendar(string userHash, PN pn);

}