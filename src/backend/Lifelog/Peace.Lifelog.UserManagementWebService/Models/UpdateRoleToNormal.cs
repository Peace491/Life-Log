namespace Peace.Lifelog.UserManagementWebService;
using Peace.Lifelog.Security;
public class UpdateRoleToNormal
{
    public AppPrincipal Principal { get; set; } = new AppPrincipal();
    public string UserId { get; set; } = string.Empty;
}
