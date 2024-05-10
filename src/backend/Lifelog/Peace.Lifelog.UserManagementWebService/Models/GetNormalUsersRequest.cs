namespace Peace.Lifelog.UserManagementWebService;
using Peace.Lifelog.Security;

public class GetNormalUsersRequest
{
    public AppPrincipal Principal { get; set; } = new AppPrincipal();
}
