namespace Peace.Lifelog.LLI;

public class LLIRecurrence
{
    public LLIRecurrenceStatus LLIRecurrenceStatus { get; set; } = LLIRecurrenceStatus.Off;
    public LLIRecurrenceFrequency? LLIRecurrenceFrequency { get; set; } = null;
}

public enum LLIRecurrenceStatus
{
    On,
    Off
}

public enum LLIRecurrenceFrequency
{
    Weekly,
    Monthly,
    Yearly
}
