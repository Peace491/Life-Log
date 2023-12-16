namespace Peace.Lifelog.UserManagement;

public interface IUserProfileRequest
{
    public string ModelName { get; }
    public (string Type, string Value) UserId { get; set; }

}
