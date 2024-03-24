namespace Peace.Lifelog.CalendarServiceTest;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Peace.Lifelog.CalendarService;
using Peace.Lifelog.CalendarService.Models;
using Peace.Lifelog.LLI;


public class CalendarServiceShould()
    {

        private const int MAX_EXECUTION_TIME_IN_SECONDS = 3;

        #region Get Month Data Tests

        [Fact]
        public void CalendarServiceShould_HaveCurrentMonthData()
        {

        // Arrange
        var calendarService = new CalendarService();
        DateTime currentDate = DateTime.Now;
        DateTime firstDayOfMonth = new DateTime(currentDate.Year, currentDate.Month, 1);

        // Act
        var correctMonthDataResponse = calendarService.GetMonthData();

        // Assert
        Assert.True(correctMonthDataResponse.HasError);

        var monthData = new MonthData();
        if (correctMonthDataResponse.Output is not null)
        {
            foreach (MonthData output in correctMonthDataResponse.Output)
            {
                monthData = output;
            }
        }

        Assert.True(monthData.Month == currentDate.Month);
        Assert.True(monthData.CurrDay == currentDate.Day);
        Assert.True(monthData.NumOfDayInMonth == DateTime.DaysInMonth(currentDate.Year, currentDate.Month));
        Assert.True(monthData.DayOfTheWeekFor1stDay == firstDayOfMonth.DayOfWeek.ToString());

    }

        [Fact]
        public void CalendarServiceShould_HaveNextMonthData()
        {


        // Arrange
        var calendarService = new CalendarService();
        
        // Act
        var nextMonthDataResponse = calendarService.NextMonth();

        // Assert
        Assert.True(nextMonthDataResponse.HasError);


        DateTime thisMonthDate = calendarService.MonthState;
        DateTime firstDayOfMonth = new DateTime(thisMonthDate.Year, thisMonthDate.Month, 1);

        var monthData = new MonthData();
        if (nextMonthDataResponse.Output is not null)
        {
            foreach (MonthData output in nextMonthDataResponse.Output)
            {
                monthData = output;
            }
        }

        // change to next month
        Assert.True(monthData.Month == thisMonthDate.Month);
        Assert.True(monthData.CurrDay == thisMonthDate.Day);
        Assert.True(monthData.NumOfDayInMonth == DateTime.DaysInMonth(thisMonthDate.Year, thisMonthDate.Month));
        Assert.True(monthData.DayOfTheWeekFor1stDay == firstDayOfMonth.DayOfWeek.ToString());

    }

        [Fact]
        public void CalendarServiceShould_HavePrevMonthData()
        {

        // Arrange
        var calendarService = new CalendarService();

        // Act
        var prevMonthDataResponse = calendarService.PrevMonth();

        // Assert
        Assert.True(prevMonthDataResponse.HasError);


        DateTime thisMonthDate = calendarService.MonthState;
        DateTime firstDayOfMonth = new DateTime(thisMonthDate.Year, thisMonthDate.Month, 1);

        var monthData = new MonthData();
        if (prevMonthDataResponse.Output is not null)
        {
            foreach (MonthData output in prevMonthDataResponse.Output)
            {
                monthData = output;
            }
        }


        // change to next month
        Assert.True(monthData.Month == thisMonthDate.Month);
        Assert.True(monthData.CurrDay == thisMonthDate.Day);
        Assert.True(monthData.NumOfDayInMonth == DateTime.DaysInMonth(thisMonthDate.Year, thisMonthDate.Month));
        Assert.True(monthData.DayOfTheWeekFor1stDay == firstDayOfMonth.DayOfWeek.ToString());

    }

        [Fact]
        public void CalendarServiceShould_HaveCurrentMonthLLI()
        {
        // Arrange
        var calendarService = new CalendarService();

        // Act
        var monthLLIResponse = calendarService.GetMonthLLI();

        // Assert

        var currMonthDateTime = calendarService.MonthState;

        var monthData = new MonthData();
        if (monthLLIResponse.Output is not null)
        {
            foreach (MonthData output in monthLLIResponse.Output)
            {
                monthData = output;
            }
        }

        var LLI = new LLI();
        if (monthData.LLIEvent is not null)
        {
            foreach (LLI outputLLI in monthData.LLIEvent)
            {
                DateTime LLIdateTime;
                DateTime.TryParseExact(outputLLI.Deadline, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out LLIdateTime);

                Assert.True(currMonthDateTime.Year == LLIdateTime.Year && currMonthDateTime.Month == LLIdateTime.Month);

            }
        }


    }

        

        [Fact]
        public void CalendarServiceShould_HaveCurrentMonthPN()
        {
            // Arrange

            // Act

            // Assert
            throw new NotImplementedException();

        }


        #endregion

        [Fact]
        public void CalendarServiceShould_GetMonthDataInLessThan3Seconds()
        {

        // Arrange
        var timer = new Stopwatch();
        var calendarService = new CalendarService();

        // Act
        timer.Start();
        var calendarResponse = calendarService.GetMonthData();
        timer.Stop();

        // Assert
        Assert.True(calendarResponse.HasError == false);
        Assert.True(timer.Elapsed.TotalSeconds <= MAX_EXECUTION_TIME_IN_SECONDS);

    }


}

