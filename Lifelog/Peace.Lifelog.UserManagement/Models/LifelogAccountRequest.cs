namespace Peace.Lifelog.UserManagementTest;

using Peace.Lifelog.UserManagement;

public class LifelogAccountRequest : IUserAccountRequest, IMultifactorAccountRequest, IStatusAccountRequest
{
    public string ModelName { get; } = "LifelogAccount";
    public (string Type, string Value) UserId { get; set; }
    public (string Type, string Value) UserHash { get; set; }
    public (string Type, string Value) CreationDate { get; set; }
    public (string Type, string Value) Salt { get; set; }
    public (string Type, string Value) Role { get; set; }
    public (string Type, string Value) MfaId { get; set; }
    public (string Type, string Value) AccountStatus { get; set; } = ("AccountStatus", "Enabled");
}