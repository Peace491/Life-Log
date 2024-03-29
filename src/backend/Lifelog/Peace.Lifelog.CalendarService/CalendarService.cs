namespace Peace.Lifelog.CalendarService;

using DomainModels;
using Peace.Lifelog.CalendarService.Models;
using Peace.Lifelog.DataAccess;
using Peace.Lifelog.LLI;
using Peace.Lifelog.Logging;
using System;



public class CalendarService : IGetMonthData, IGetMonthLLI, IPrevMonth, INextMonth
{

    public DateTime MonthState = DateTime.Now;

    // contructor to create LLIService
    private CreateDataOnlyDAO createDataOnlyDAO;
    private ReadDataOnlyDAO readDataOnlyDAO;
    private UpdateDataOnlyDAO updateDataOnlyDAO;
    private DeleteDataOnlyDAO deleteDataOnlyDAO;
    private LogTarget logTarget;
    private Logging logging;
    private LLIService lliService;
    public CalendarService()
    {
        this.createDataOnlyDAO = new CreateDataOnlyDAO();
        this.readDataOnlyDAO = new ReadDataOnlyDAO();
        this.updateDataOnlyDAO = new UpdateDataOnlyDAO();
        this.deleteDataOnlyDAO = new DeleteDataOnlyDAO();
        this.logTarget = new LogTarget(this.createDataOnlyDAO);
        this.logging = new Logging(this.logTarget);
        this.lliService = new LLIService(this.createDataOnlyDAO, this.readDataOnlyDAO, this.updateDataOnlyDAO, this.deleteDataOnlyDAO, this.logging);

    }

    public async Task<Response> PrevMonth(string userHash)
    {
        

        MonthState = MonthState.AddMonths(-1);
        var prevMonthResponse = await GetMonthData(userHash);

        return prevMonthResponse;
    }

    public async Task<Response> NextMonth(string userHash)
    {
        

        MonthState = MonthState.AddMonths(1);
        var nextMonthResponse = await GetMonthData(userHash);

        return nextMonthResponse;
    }

    public async Task<Response> GetMonthData(string userHash)
    {
        var getMonthResponse = new Response();
        // get PN data

        var monthData = new MonthData();
        var LLIMonthData = await GetMonthLLI(userHash);
        monthData.LLIEvent = LLIMonthData.LLIEvent;

        
        monthData.Month = MonthState.Month;
        monthData.Year = MonthState.Year;

        if (MonthState.Year == DateTime.Now.Year && MonthState.Month == DateTime.Now.Month)
        { monthData.CurrDay = DateTime.Now.Day; }
        else { monthData.CurrDay = -1; }

        monthData.NumOfDayInMonth = DateTime.DaysInMonth(MonthState.Year, MonthState.Month);

        DateTime firstDayOfMonth = new DateTime(MonthState.Year, MonthState.Month, 1);
        monthData.DayOfTheWeekFor1stDay = firstDayOfMonth.DayOfWeek.ToString();


        getMonthResponse.Output = new List<object>();
        if (getMonthResponse.Output is not null)
        {
            getMonthResponse.Output.Add(monthData);
        }
        


        getMonthResponse.HasError = false;

        return getMonthResponse;
    }

    public async Task<MonthData> GetMonthLLI(string userHash) 
    {
        
        var monthData = new MonthData();
        monthData.LLIEvent = new List<LLI>();

        var getLLIResponse = await lliService.GetAllLLIFromUser(userHash);


        if (getLLIResponse.Output is not null)
        {
            foreach (LLI LLI in getLLIResponse.Output.Cast<LLI>())
            {

                DateTime LLIdateTime;
                DateTime.TryParseExact(LLI.Deadline, "M/d/yyyy h:mm:ss tt", null, System.Globalization.DateTimeStyles.None, out LLIdateTime);

                
                if (LLIdateTime.Year == MonthState.Year && LLIdateTime.Month == MonthState.Month)
                {
                    
                    monthData.LLIEvent.Add(LLI);
                }
            }
        }
        
        

        // return monthData

        return monthData;

    }

    public async Task<Response> CreateLLIWithCalendar(string userHash, LLI lli)
    {
        var createLLIResponse = await lliService.CreateLLI(userHash, lli);
        return createLLIResponse;
    }

    public async Task<Response> EditLLIWithCalendar(string userHash, LLI lli)
    {
        var editLLIResponse = await lliService.UpdateLLI(userHash, lli);
        return editLLIResponse;
    }


}
