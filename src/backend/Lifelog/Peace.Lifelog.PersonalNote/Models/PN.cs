namespace Peace.Lifelog.PersonalNote;

public class PN
{
    public string NoteId { get; set; } = string.Empty;
    public string UserHash { get; set; } = string.Empty; // Need to reimplement using a global UserHash model later on
    public string NoteDate { get; set; } = string.Empty;
    public string NoteContent { get; set; } = string.Empty;
}
