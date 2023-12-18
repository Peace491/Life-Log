using System.Dynamic;
using Peace.Lifelog.UserManagement;

namespace Peace.Lifelog.UserManagementTest;

public class TestProfileRequest : IUserProfileRequest
{
    public string ModelName { get; } = TestVariables.PROFILE_TABLE;
    public (string Type, string Value) UserId { get; set; }
    public (string Type, string Value) DOB { get; set; }
    public (string Type, string Value) ZipCode { get; set; }

}
