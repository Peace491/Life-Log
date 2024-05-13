namespace Peace.Lifelog.Infrastructure;

public class LLIDB
{
    public string LLIID { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? Category1 { get; set; } = null; // Maybe give default category
    public string? Category2 { get; set; } = null;
    public string? Category3 { get; set; } = null;
    public string Status { get; set; } = string.Empty;
    public string Visibility { get; set; } = string.Empty;
    public string Deadline { get; set; } = string.Empty;
    public int? Cost { get; set; } = null;
    public string RecurrenceStatus { get; set; } = string.Empty;
    public string RecurrenceFrequency { get; set; } = string.Empty;
    public string CompletionDate { get; set; } = string.Empty;

}
