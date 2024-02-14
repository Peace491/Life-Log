namespace Peace.Lifelog.UserManagement;
/// <summary>
/// For User Account that has a status (Ex: Enabled)
/// </summary>
public interface IStatusAccountRequest : IUserAccountRequest
{
    public (string Type, string Value) AccountStatus { get; set; }

}
