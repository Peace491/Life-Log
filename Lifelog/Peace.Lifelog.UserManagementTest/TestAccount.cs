namespace Peace.Lifelog.UserManagementTest;

using Peace.Lifelog.UserManagement;

public class TestAccount : UserAccount
{
    public string? Password { get; set; }
    public string? MfaId { get; set; }

}