namespace Peace.Lifelog.PersonalNoteWebService;
public class PostPersonalNoteRequest
{
    public string NoteDate { get; set; } = string.Empty;
    public string NoteContent { get; set; } = string.Empty;
}

public class PutPersonalNoteRequest
{
    public string NoteId { get; set; } = string.Empty;
    public string NoteDate { get; set; } = string.Empty;
    public string NoteContent { get; set; } = string.Empty;
}