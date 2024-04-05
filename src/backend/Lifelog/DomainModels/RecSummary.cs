namespace DomainModels;

public class RecSummary
{
    public string UserHash { get; set; } = string.Empty;
    public List<string> Categories { get; set; } = new List<string>();
}
