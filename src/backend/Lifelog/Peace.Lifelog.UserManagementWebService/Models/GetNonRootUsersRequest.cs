namespace Peace.Lifelog.UserManagementWebService;
using Peace.Lifelog.Security;

public class GetNonRootUsersRequest
{
    public AppPrincipal Principal { get; set; } = new AppPrincipal();
}
