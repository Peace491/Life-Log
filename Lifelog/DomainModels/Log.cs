namespace DomainModels;

public class Log
{
    public DateTime Timestamp { get; set; }
    public string Level { get; set; }
    public string Category { get; set; }
    public string? Message { get; set; }
}
