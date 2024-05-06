using Peace.Lifelog.UserManagement;

namespace Peace.Lifelog.UserManagement;

public class LifelogProfileRequest : IUserProfileRequest
{
    public string ModelName { get; } = "LifelogProfile";
    public (string Type, string Value) UserId { get; set; }
    public (string Type, string Value) DOB { get; set; }
    public (string Type, string Value) ZipCode { get; set; }
    public (string Type, int Value) UserFormCompletionStatus { get; set; } = ("IsUserFormCompleted", 0);

}