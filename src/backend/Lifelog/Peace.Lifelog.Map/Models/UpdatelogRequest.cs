namespace Peace.Lifelog.Map;

using Peace.Lifelog.Security;

public class UpdateLogRequest
{
    public AppPrincipal? Principal { get; set; }
}

public static class logRequsetType
{
    public static string Update = "Update";
}