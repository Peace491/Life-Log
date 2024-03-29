
namespace Peace.Lifelog.CalendarService.Models;
using Peace.Lifelog.LLI;
// using Peace.Lifelog.PN;

public class MonthData
{
    // change into DateTime obj so next/prev will contain the same state
    public int Month { get; set; } = 0;

    public int Year { get; set; } = 0;

    public int CurrDay { get; set; } = -1;

    public int NumOfDayInMonth { get; set;} = 0;

    public string DayOfTheWeekFor1stDay { get; set; } = string.Empty; // Friday, Thursday etc.

    public ICollection<LLI>? LLIEvent { get; set; }

    // add PN prop


}
