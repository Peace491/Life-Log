namespace Peace.Lifelog.RE;

public class REDataMart
{
    public string UserHash { get; set; } = string.Empty;
    public string MostCommonUserCategory { get; set; } = string.Empty; // or have system's most common user category be most common public category
    public string MostCommonUserSubCategory { get; set; } = string.Empty;
    public string MostCommonPublicCategory { get; set; } = string.Empty;
}
