namespace Peace.Lifelog.UserManagement;

public interface IUserHashRequest : IUserManagementRequest
{
    public new string ModelName { get; }
    public (string Type, string Value) UserId { get; set; }
    public (string Type, string Value) UserHash { get; set; }

}
