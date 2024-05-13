namespace Peace.Lifelog.UserManagementWebService;
using Peace.Lifelog.Security;
public class UpdateStatus
{
    public AppPrincipal Principal { get; set; } = new AppPrincipal();
    public string UserId { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;

}
