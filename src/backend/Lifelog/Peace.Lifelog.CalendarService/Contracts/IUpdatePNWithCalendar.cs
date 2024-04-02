namespace Peace.Lifelog.CalendarService;

using DomainModels;
using Peace.Lifelog.PersonalNote;

public interface IUpdatePNWithCalendar
{
    Task<Response> UpdatePNWithCalendar(string userHash, PN pn);

}