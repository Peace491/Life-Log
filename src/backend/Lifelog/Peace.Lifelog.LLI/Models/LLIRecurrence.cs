namespace Peace.Lifelog.LLI;

public class LLIRecurrence
{
    public string Status { get; set; } = LLIRecurrenceStatus.Off;
    public string Frequency { get; set; } = LLIRecurrenceFrequency.None;
}

public class LLIRecurrenceStatus
{
    public static string On = "On";
    public static string Off = "Off";
}

public class LLIRecurrenceFrequency
{
    public static string Weekly = "Weekly";
    public static string Monthly = "Monthly";
    public static string Yearly = "Yearly";
    public static string None = "None";
}
