namespace Peace.Lifelog.UserManagement;

public class LifelogUserHashRequest : IUserHashRequest
{
    public string ModelName { get; } = "LifelogUserHash";

    public (string Type, string Value) UserId { get ; set; }
    public (string Type, string Value) UserHash { get; set;}
}
