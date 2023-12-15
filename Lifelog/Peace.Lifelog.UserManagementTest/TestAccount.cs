namespace Peace.Lifelog.UserManagementTest;

using Peace.Lifelog.UserManagement;

public class TestAccount : BaseUserAccount
{
    public override string ModelName { get; } = "TestAccount";
    public (string Type, string Value) Password { get; set; }
    public (string Type, string Value) MfaId { get; set; }

}