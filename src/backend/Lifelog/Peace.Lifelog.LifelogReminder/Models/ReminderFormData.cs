namespace Peace.Lifelog.LifelogReminder;

using Peace.Lifelog.Security;

public class ReminderFormData
{
    public AppPrincipal? Principal { get; set; }
    public string UserHash { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string Frequency { get; set; } = string.Empty;
}