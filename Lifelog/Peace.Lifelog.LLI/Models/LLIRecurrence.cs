﻿namespace Peace.Lifelog.LLI;

public class LLIRecurrence
{
    public LLIRecurrenceStatus Status { get; set; } = LLIRecurrenceStatus.Off;
    public LLIRecurrenceFrequency? Frequency { get; set; } = LLIRecurrenceFrequency.None;
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
    Yearly,
    None
}
