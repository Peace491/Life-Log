namespace Peace.Lifelog.CalendarService;

using DomainModels;
using Peace.Lifelog.PersonalNote;

public interface IGetMonthPN
{
    Task<Response> GetMonthPN(string userHash, int month, int year);

}