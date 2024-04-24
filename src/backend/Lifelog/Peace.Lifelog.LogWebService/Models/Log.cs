namespace Peace.Lifelog.LogWebService;

public class Log
{
    public string Table = "Logs" ;
    public string UserHash { get; set; } = string.Empty ;
    public string Level { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty ;
    public string? Message { get; set; }

}
