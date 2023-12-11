namespace Peace.Lifelog.Security;

public class AuthenticationSQLDetails
{
    public string TableName { get; set; } = string.Empty;
    public string UserIdColumnName { get; set; } = string.Empty;
    public string ProofColumnName { get; set; } = string.Empty;
    public string ClaimColumnName { get; set; } = string.Empty;
}
