namespace Peace.Lifelog.LLI;

public class LLIStatus
{
    public static string Active = "Active";
    public static string Postponed = "Postponed";
    public static string Completed = "Completed";
    private static HashSet<string> StatusSet = new HashSet<string>
        {
            Active,
            Postponed,
            Completed
        };
        
    public static bool IsValidStatus(string status) {
        return StatusSet.Contains(status);
    }
}
