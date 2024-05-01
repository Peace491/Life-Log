using Peace.Lifelog.Security;

namespace Peace.Lifelog.UserManagementWebService;

public class RecoverAccountRequest
{
    public AppPrincipal Principal { get; set; } = new AppPrincipal();
    public string UserId { get; set; } = string.Empty;

}
