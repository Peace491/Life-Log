using System.Diagnostics.Tracing;

namespace Peace.Lifelog.LLI;
public class LLI2
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
    public string Category1 { get; set; } = "None"; // Maybe give default category
    public string Category2 { get; set; } = "None";
    public string Category3 { get; set; } = "None";
}

