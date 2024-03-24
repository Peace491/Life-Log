namespace Peace.Lifelog.CalendarService;

using DomainModels;
using System;




public class CalendarService : IGetMonthData, IGetMonthLLI, IPrevMonth, INextMonth
{

    public DateTime MonthState;

    public Response PrevMonth()
    {
        throw new NotImplementedException();
    }

    public Response NextMonth()
    {
        throw new NotImplementedException();
    }

    public Response GetMonthData()
    {
        throw new NotImplementedException();
    }

    public Response GetMonthLLI() 
    {
    
        throw new NotImplementedException();
    }


}
