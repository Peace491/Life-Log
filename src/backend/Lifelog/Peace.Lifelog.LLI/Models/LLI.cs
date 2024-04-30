namespace Peace.Lifelog.LLI;
public class LLI
{
    public string LLIID { get; set; } = string.Empty;
    public string UserHash { get; set; } = string.Empty; // Need to reimplement using a global UserHash model later on
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = LLIStatus.Active;
    public string Visibility { get; set; } = LLIVisibility.Public;
    public string Deadline { get; set; } = string.Empty;
    public int? Cost { get; set; } = null;
    public LLIRecurrence Recurrence { get; set; } = new LLIRecurrence(); // LLIRecurrence has the default values
    public string CreationDate { get; set; } = string.Empty;
    public string CompletionDate { get; set; } = string.Empty;
    public string? Category1 { get; set; } = null; // Maybe give default category
    public string? Category2 { get; set; } = null;
    public string? Category3 { get; set; } = null;
    public byte[]? MediaMemento { get; set; } = null;
}

