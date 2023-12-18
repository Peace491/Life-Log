namespace Peace.Lifelog.SecurityTest;
using Peace.Lifelog.DataAccess;
using Peace.Lifelog.Security;
using System.Diagnostics;

public class AppAuthServiceShould : IDisposable
{
    private const int MAX_EXECUTION_TIME_IN_SECONDS = 3;
    private const string TABLE = "TestAuthentication";
    private const string USER_ID_TYPE = "UserId";
    private const string PROOF_TYPE = "Password";
    private const string CLAIM_TYPE = "UserRole";




    // Setup for all test
    public AppAuthServiceShould()
    {
        var DDLTransactionDAO = new DDLTransactionDAO();
        var createMockTableSql = $"CREATE TABLE {TABLE} ("
        + $"{USER_ID_TYPE} serial  NOT NULL," +
        $"{PROOF_TYPE} text NOT NULL," +
        $"{CLAIM_TYPE} text NOT NULL," +
        $"PRIMARY KEY ({USER_ID_TYPE})" +
        ");";

        DDLTransactionDAO.ExecuteDDLCommand(createMockTableSql);
        
    }

    // Cleanup for all tests
    public async void Dispose()
    {
        var DDLTransactionDAO = new DDLTransactionDAO();

        var deleteMockTableSql = $"DROP TABLE {TABLE}";

        var test = await DDLTransactionDAO.ExecuteDDLCommand(deleteMockTableSql);
    }

    // Authentication Tests
    [Fact]

    public async void AppAuthServiceAuthNShould_ReturnPrincipal_ForTheUser()
    {
        //Arrange
        var timer = new Stopwatch();

        var appAuthService = new AppAuthService();

        var createDataOnlyDAO = new CreateDataOnlyDAO();

        var readDataOnlyDAO = new ReadDataOnlyDAO();

        var mockUserId = "1";
        var mockProof = "Proof";
        var mockClaim = "Claim";

        var insertSql = $"INSERT INTO {TABLE} ({USER_ID_TYPE}, {PROOF_TYPE}, {CLAIM_TYPE}) VALUES ('{mockUserId}', '{mockProof}', '{mockClaim}')";
        await createDataOnlyDAO.CreateData(insertSql);

        var authRequest = new TestAuthenticationRequest();

        authRequest.UserId = (USER_ID_TYPE, mockUserId);
        authRequest.Proof = (PROOF_TYPE ,mockProof);
        authRequest.Claims = (CLAIM_TYPE, mockClaim);

        //Act
        timer.Start();
        var response = await appAuthService.AuthenticateUser(authRequest);
        timer.Stop();

        //Assert
        Assert.True(response.Claims[CLAIM_TYPE] == mockClaim);
        Assert.True(timer.Elapsed.TotalSeconds <= MAX_EXECUTION_TIME_IN_SECONDS);

    }

    [Fact]
    public async void AppAuthServiceAuthNShould_ReturnThrowArgumentNullException_IfUserIdIsNull()
    {
        //Arrange
        var timer = new Stopwatch();

        var appAuthService = new AppAuthService();

        // mockUserId is null
        var mockProof = "Proof";
        var mockClaim = "Claim";

        var authRequest = new TestAuthenticationRequest();

        authRequest.UserId = (USER_ID_TYPE, "");
        authRequest.Proof = (PROOF_TYPE ,mockProof);
        authRequest.Claims = (CLAIM_TYPE, mockClaim);

        var nullExceptionIsReturn = false;

        //Act
        try
        {
            var response = await appAuthService.AuthenticateUser(authRequest);
        }
        catch (ArgumentNullException)
        {
            nullExceptionIsReturn = true;
        }

        // Assert
        Assert.True(nullExceptionIsReturn);

    }

    [Fact]
    public async void AppAuthServiceAuthNShould_ReturnThrowArgumentNullException_IfProofIsNull()
    {
        //Arrange
        var appAuthService = new AppAuthService();

        var mockUserId = "1";
        var mockClaim = "Claim";

        var authRequest = new TestAuthenticationRequest();

        authRequest.UserId = (USER_ID_TYPE, mockUserId);
        authRequest.Proof = (PROOF_TYPE ,"");
        authRequest.Claims = (CLAIM_TYPE, mockClaim);

        var nullExceptionIsReturn = false;

        //Act
        try
        {
            var response = await appAuthService.AuthenticateUser(authRequest);
        }
        catch (ArgumentNullException)
        {
            nullExceptionIsReturn = true;
        }

        // Assert
        Assert.True(nullExceptionIsReturn);

    }

    [Fact]
    public async void AppAuthServiceAuthNShould_ReturnThrowArgumentNullException_IfSQLDetailsIsNull()
    {
        //Arrange
        var appAuthService = new AppAuthService();

        var mockUserId = "1";
        var mockProof = "Proof";

        var authRequest = new TestAuthenticationRequest();

        authRequest.UserId = (USER_ID_TYPE, mockUserId);
        authRequest.Proof = (PROOF_TYPE ,mockProof);
        authRequest.Claims = (CLAIM_TYPE, "");

        var nullExceptionIsReturn = false;

        //Act
        try
        {
            var response = await appAuthService.AuthenticateUser(authRequest);
        }
        catch (ArgumentNullException)
        {
            nullExceptionIsReturn = true;
        }

        // Assert
        Assert.True(nullExceptionIsReturn);

    }

    [Fact]
    public async void AppAuthServiceAuthNShould_ReturnNull_IfUserIdIsNotFound()
    {
        //Arrange
        var appAuthService = new AppAuthService();

        var createDataOnlyDAO = new CreateDataOnlyDAO();

        var readDataOnlyDAO = new ReadDataOnlyDAO();

        var mockUserId = "4"; // this user id does not exist
        var mockProof = "Proof";
        var mockClaim = "Claim";

        var authRequest = new TestAuthenticationRequest();

        authRequest.UserId = (USER_ID_TYPE, mockUserId);
        authRequest.Proof = (PROOF_TYPE ,mockProof);
        authRequest.Claims = (CLAIM_TYPE, mockClaim);

        //Act

        var response = await appAuthService.AuthenticateUser(authRequest);

        // Assert
        Assert.Null(response);

    }

    [Fact]
    public async void AppAuthServiceAuthNShould_ReturnNull_IfUserIdProofCombinationIsInvalid()
    {
        //Arrange
        var timer = new Stopwatch();

        var appAuthService = new AppAuthService();

        var createDataOnlyDAO = new CreateDataOnlyDAO();

        var readDataOnlyDAO = new ReadDataOnlyDAO();

        var mockUserId = "2"; // this user id does not exist
        var mockProof = "Proof";
        var mockWrongProof = "Wrong Proof";
        var mockClaim = "Claim";
        var mockClaimDict = new Dictionary<string, string>() {
            {"claim", mockClaim}
        };

        var insertSql = $"INSERT INTO {TABLE} ({USER_ID_TYPE}, {PROOF_TYPE}, {CLAIM_TYPE}) VALUES ('{mockUserId}', '{mockProof}', '{mockClaim}')";
        await createDataOnlyDAO.CreateData(insertSql);

        var authRequest = new TestAuthenticationRequest();

        authRequest.UserId = (USER_ID_TYPE, mockUserId);
        authRequest.Proof = (PROOF_TYPE ,mockWrongProof);
        authRequest.Claims = (CLAIM_TYPE, mockClaim);

        //Act
        var response = await appAuthService.AuthenticateUser(authRequest);

        // Assert
        Assert.Null(response);

    }

    // Authorization Tests
    [Fact]
    public async void AppAuthServiceAuthZShould_ReturnTrue_IfUserIsAuthorize()
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
    public async void AppAuthServiceAuthZShould_ReturnThrowArgumentNullException_IfCurrentPrincpalIsNull()
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
        try
        {
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
    public async void AppAuthServiceAuthZShould_ReturnThrowArgumentNullException_IfRequiredClaimsIsNull()
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
        try
        {
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
    public async void AppAuthServiceAuthZShould_ReturnFalse_IfUserIsNotAuthorize()
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
