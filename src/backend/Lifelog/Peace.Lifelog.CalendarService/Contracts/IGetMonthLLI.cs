namespace Peace.Lifelog.CalendarService;

using Peace.Lifelog.CalendarService.Models;
using DomainModels;

public interface IGetMonthLLI
{
    Task<MonthData> GetMonthLLI(string userHash);
}