namespace Peace.Lifelog.UserManagement;

public abstract class BaseUserAccount
{
    public abstract string ModelName { get; }
    public (string Type, string Value) UserId { get; set; } = (string.Empty, string.Empty);
}
