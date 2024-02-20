namespace Peace.Lifelog.LLI;

public class LLI
{
    public string LLIID { get; set; } = string.Empty;
    public string UserHash { get; set; } = string.Empty; // Need to reimplement using a global UserHash model later on
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; } = null;
    public string? Category { get; set; }
    public string Status { get; set; } = LLIStatus.Active;
    public string Visibility { get; set; } = LLIVisibility.Public;
    public string Deadline { get; set; } = string.Empty;
    public int? Cost { get; set; } = null;
    public LLIRecurrence Recurrence { get; set; } = new LLIRecurrence(); // LLIRecurrence has the default values
}
