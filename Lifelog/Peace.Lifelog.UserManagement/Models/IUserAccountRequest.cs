namespace Peace.Lifelog.UserManagement;
/// <summary>
/// For all User Account
/// </summary>
public interface IUserAccountRequest
{
    public string ModelName { get; }
    public (string Type, string Value) UserId { get; set; }
}
