namespace Peace.Lifelog.UserManagement;

using Peace.Lifelog.Security;
using Peace.Lifelog.UserManagement;

public class LifelogAccountRequest : IUserAccountRequest, IStatusAccountRequest
{
    public string ModelName { get; } = "LifelogAccount";
    public AppPrincipal principal { get; set; } = new AppPrincipal();
    public (string Type, string Value) UserId { get; set; }
    public (string Type, string Value)? UserHash { get; set; } = null;
    public (string Type, string Value) CreationDate { get; set; }
    public (string Type, string Value) Salt { get; set; }
    public (string Type, string Value) Role { get; set; }
    public (string Type, string Value) AccountStatus { get; set; } = ("AccountStatus", "Enabled");
}