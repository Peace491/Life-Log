namespace Peace.Lifelog.CalendarService;

using DomainModels;

public interface IGetMonthLLI
{
    Task<Response> GetMonthLLI(string userHash, int month, int year);
}