namespace Peace.Lifelog.CalendarService;

using DomainModels;

public interface IGetMonthData
{
    Task<Response> GetMonthData(string userHash);
}