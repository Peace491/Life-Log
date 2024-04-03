namespace Peace.Lifelog.CalendarWebService.Models;


using Peace.Lifelog.LLI;

public class CurrMonthRequest
{
    public int Month { get; set; } = 0;
    public int Year { get; set; } = 0;
}

public class PostPersonalNoteRequest
{
    public string NoteDate { get; set; } = string.Empty;
    public string NoteContent { get; set; } = string.Empty;
}

public class PutPersonalNoteRequest
{
    public string NoteId { get; set; } = string.Empty;
    public string NoteDate { get; set; } = string.Empty;
    public string NoteContent { get; set; } = string.Empty;
}

public class PostLLIRequest
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? Category1 { get; set; } = null; // Maybe give default category
    public string? Category2 { get; set; } = null;
    public string? Category3 { get; set; } = null;
    public string Status { get; set; } = LLIStatus.Active;
    public string Visibility { get; set; } = LLIVisibility.Public;
    public string Deadline { get; set; } = string.Empty;
    public int? Cost { get; set; } = null;
    public string RecurrenceStatus { get; set; } = "On";
    public string RecurrenceFrequency { get; set; } = "None";
}

public class PutLLIRequest
{
    public string LLIID { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? Category1 { get; set; } = null; // Maybe give default category
    public string? Category2 { get; set; } = null;
    public string? Category3 { get; set; } = null;
    public string Status { get; set; } = LLIStatus.Active;
    public string Visibility { get; set; } = LLIVisibility.Public;
    public string Deadline { get; set; } = string.Empty;
    public int? Cost { get; set; } = null;
    public string RecurrenceStatus { get; set; } = "On";
    public string RecurrenceFrequency { get; set; } = "None";
}