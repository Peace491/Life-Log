namespace Peace.Lifelog.LLI;

public class LLIVisibility
{
    public static string Public = "Public";
    public static string Private = "Private";
    private static HashSet<string> VisibilitySet = new HashSet<string>
        {
            Public,
            Private
        };

    public static bool IsValidVisibility(string visibility) {
        return VisibilitySet.Contains(visibility);
    }

}
