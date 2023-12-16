namespace Peace.Lifelog.UserManagement;

public interface IStatusAccountRequest : IUserAccountRequest
{
    public (string Type, string Value) AccountStatus { get; set; }

}
