namespace Peace.Lifelog.SecurityTest;
using Peace.Lifelog.Security;
using Peace.Lifelog.DataAccess;

using System.Diagnostics;

public class AppAuthServiceShould
{
    private const int MAX_EXECUTION_TIME_IN_SECONDS = 3;
    private const string TABLE = "app_auth_service_test";



    // Setup for all test
    public AppAuthServiceShould()
    {
        var DDLTransactionDAO = new DDLTransactionDAO();
        var createMockTableSql = $"CREATE TABLE {TABLE} ("
        + "(user_id serial  NOT NULL," +
        "proof text  NOT NULL," +
        "CONSTRAINT app_auth_service_test_pk " +
        "PRIMARY KEY (user_id)" +
        ");";

        DDLTransactionDAO.ExecuteDDLCommand(createMockTableSql);
    }

    // Cleanup for all tests
    public async void Dispose()
    {
        var DDLTransactionDAO = new DDLTransactionDAO();

        var deleteMockTableSql = $"DROP TABLE {TABLE}";

        await DDLTransactionDAO.ExecuteDDLCommand(deleteMockTableSql);
    }

    // Authentication Tests
    // TODO

    // Authorization Tests
    [Fact]
    public async void AppAuthServiceShould_ReturnTrue_IfUserIsAuthorize()
    {
        // Arrange
        var timer = new Stopwatch();

        var appAuthService = new AppAuthService();
  
        var currentPrincipal = new AppPrincipal();
        currentPrincipal.UserId = "1";
        currentPrincipal.Claims = new Dictionary<string, string>() {
            {"RoleName", "Admin"}
        };

        var requiredClaims = new Dictionary<string, string>() {
            {"RoleName", "Admin"}
        };


        // Act
        timer.Start();
        bool IsAuthorizeResponse = appAuthService.IsAuthorize(currentPrincipal, requiredClaims); // Need to test for all behavior of string
        timer.Stop();


        // Assert
        Assert.True(timer.Elapsed.TotalSeconds <= MAX_EXECUTION_TIME_IN_SECONDS);
        Assert.True(IsAuthorizeResponse == true);
    }

    [Fact]
    public async void AppAuthServiceShould_ReturnThrowArgumentNullException_IfCurrentPrincpalIsNull()
    {
        // Arrange
        var timer = new Stopwatch();

        var appAuthService = new AppAuthService();
  
        AppPrincipal? currentPrincipal = null;

        var requiredClaims = new Dictionary<string, string>() {
            {"RoleName", "Admin"}
        };

        var nullExceptionIsReturn = false;


        // Act
        try {
            bool IsAuthorizeResponse = appAuthService.IsAuthorize(currentPrincipal, requiredClaims); // Need to test for all behavior of string
        } 
        catch (ArgumentNullException)
        {
            nullExceptionIsReturn = true;   
        }

        // Assert
        Assert.True(nullExceptionIsReturn);
    }

    [Fact]
    public async void AppAuthServiceShould_ReturnThrowArgumentNullException_IfRequiredClaimsIsNull()
    {
        // Arrange
        var timer = new Stopwatch();

        var appAuthService = new AppAuthService();
  
        var currentPrincipal = new AppPrincipal();
        currentPrincipal.UserId = "1";
        currentPrincipal.Claims = new Dictionary<string, string>() {
            {"RoleName", "Admin"}
        };

        IDictionary<string, string>? requiredClaims = null;
        var nullExceptionIsReturn = false;


        // Act
        try {
            bool IsAuthorizeResponse = appAuthService.IsAuthorize(currentPrincipal, requiredClaims); // Need to test for all behavior of string
        } 
        catch (ArgumentNullException)
        {
            nullExceptionIsReturn = true;   
        }

        // Assert
        Assert.True(nullExceptionIsReturn);
    }

    [Fact]
    public async void AppAuthServiceShould_ReturnFalse_IfUserIsNotAuthorize()
    {
        // Arrange
        var timer = new Stopwatch();

        var appAuthService = new AppAuthService();
  
        var currentPrincipal = new AppPrincipal();
        currentPrincipal.UserId = "1";
        currentPrincipal.Claims = new Dictionary<string, string>() {
            {"RoleName", "Normal User"}
        };

        var requiredClaims = new Dictionary<string, string>() {
            {"RoleName", "Admin"}
        };


        // Act
        timer.Start();
        bool IsAuthorizeResponse = appAuthService.IsAuthorize(currentPrincipal, requiredClaims); // Need to test for all behavior of string
        timer.Stop();


        // Assert
        Assert.True(timer.Elapsed.TotalSeconds <= MAX_EXECUTION_TIME_IN_SECONDS);
        Assert.True(IsAuthorizeResponse == false);
    }



}
