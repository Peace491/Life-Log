namespace Peace.Lifelog.UserManagement;
/// <summary>
/// For User Account that has a multifactor field
/// </summary>
public interface IMultifactorAccountRequest : IUserAccountRequest
{
    public (string Type, string Value) MfaId { get; set; }
}
