namespace Peace.Lifelog.UserManagement;

public interface IUserAccountRequest
{
    public string ModelName { get; }
    public (string Type, string Value) UserId { get; set; }
}
