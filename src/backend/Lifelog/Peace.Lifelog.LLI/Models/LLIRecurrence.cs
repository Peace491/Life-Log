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
    private static HashSet<string> RecurrenceStatusSet = new HashSet<string>
        {
            On,
            Off
        };

    public static bool IsValidRecurrenceStatus(string recurrenceStatus)
    {
        return RecurrenceStatusSet.Contains(recurrenceStatus);
    }
}

public class LLIRecurrenceFrequency
{
    public static string Weekly = "Weekly";
    public static string Monthly = "Monthly";
    public static string Yearly = "Yearly";
    public static string None = "None";
    private static HashSet<string> RecurrenceFrequencySet = new HashSet<string>
        {
            Weekly,
            Monthly,
            Yearly,
            None
        };
    
    public static bool IsValidRecurrenceFrequency(string recurrenceFrequency)
    {
        return RecurrenceFrequencySet.Contains(recurrenceFrequency);
    }
}
