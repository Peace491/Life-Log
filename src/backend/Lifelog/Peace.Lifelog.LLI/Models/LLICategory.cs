namespace Peace.Lifelog.LLI;

public class LLICategory
{
    public static string MentalHealth = "Mental Health";
    public static string PhysicalHealth = "Physical Health";
    public static string Outdoor = "Outdoor";
    public static string Sport = "Sport";
    public static string Art = "Art";
    public static string Hobby = "Hobby";
    public static string Thrill = "Thrill";
    public static string Travel = "Travel";
    public static string Volunteering = "Volunteering";
    public static string Food = "Food";
    private static HashSet<string> categorySet = new HashSet<string>()
        {
            MentalHealth,
            PhysicalHealth,
            Outdoor,
            Sport,
            Art,
            Hobby,
            Thrill,
            Travel,
            Volunteering,
            Food
        };

    public static bool IsValidCategory(string category)
    {
        return categorySet.Contains(category);
    }

}
