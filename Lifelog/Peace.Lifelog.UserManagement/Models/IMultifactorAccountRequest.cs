namespace Peace.Lifelog.UserManagement;

public interface IMultifactorAccountRequest : IUserAccountRequest
{
    public (string Type, string Value) MfaId { get; set; }
}
