namespace Peace.Lifelog.SecurityTest;

using DomainModels;
using Peace.Lifelog.DataAccess;
using Peace.Lifelog.Security;
using System.Diagnostics;

public class AppAuthServiceShould : IDisposable
{
    // Setup for all test
    public AppAuthServiceShould()
    {
        var DDLTransactionDAO = new DDLTransactionDAO();
        var createMockTableSql = $"CREATE TABLE {TestVariables.TABLE} ("
        + $"{TestVariables.USER_ID_TYPE} serial  NOT NULL," +
        $"{TestVariables.PROOF_TYPE} text NOT NULL," +
        $"{TestVariables.CLAIM_TYPE} text NOT NULL," +
        $"PRIMARY KEY ({TestVariables.USER_ID_TYPE})" +
        ");";

        var _ = DDLTransactionDAO.ExecuteDDLCommand(createMockTableSql);
        
    }

    // Cleanup for all tests
    public async void Dispose()
    {
        var DDLTransactionDAO = new DDLTransactionDAO();

        var deleteMockTableSql = $"DROP TABLE {TestVariables.TABLE}";

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

        var insertSql = $"INSERT INTO {TestVariables.TABLE} ({TestVariables.USER_ID_TYPE}, {TestVariables.PROOF_TYPE}, {TestVariables.CLAIM_TYPE}) "
        + $"VALUES ('{mockUserId}', '{mockProof}', '{mockClaim}')";
        await createDataOnlyDAO.CreateData(insertSql);

        var authRequest = new TestAuthenticationRequest();

        authRequest.UserId = (TestVariables.USER_ID_TYPE, mockUserId);
        authRequest.Proof = (TestVariables.PROOF_TYPE ,mockProof);
        authRequest.Claims = (TestVariables.CLAIM_TYPE, mockClaim);

        //Act
        timer.Start();
        var response = await appAuthService.AuthenticateUser(authRequest)!;
        timer.Stop();

        //Assert
        Assert.True(response.Claims != null);
        Assert.True(response.Claims[TestVariables.CLAIM_TYPE] == mockClaim);
        Assert.True(timer.Elapsed.TotalSeconds <= TestVariables.MAX_EXECUTION_TIME_IN_SECONDS);

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

        authRequest.UserId = (TestVariables.USER_ID_TYPE, "");
        authRequest.Proof = (TestVariables.PROOF_TYPE ,mockProof);
        authRequest.Claims = (TestVariables.CLAIM_TYPE, mockClaim);

        var nullExceptionIsReturn = false;

        //Act
        try
        {
            var response = await appAuthService.AuthenticateUser(authRequest)!;
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

        authRequest.UserId = (TestVariables.USER_ID_TYPE, mockUserId);
        authRequest.Proof = (TestVariables.PROOF_TYPE ,"");
        authRequest.Claims = (TestVariables.CLAIM_TYPE, mockClaim);

        var nullExceptionIsReturn = false;

        //Act
        try
        {
            var response = await appAuthService.AuthenticateUser(authRequest)!;
        }
        catch (ArgumentNullException)
        {
            nullExceptionIsReturn = true;
        }

        // Assert
        Assert.True(nullExceptionIsReturn);

    }

    [Fact]
    public async void AppAuthServiceAuthNShould_ReturnThrowArgumentNullException_IfClaimIsNull()
    {
        //Arrange
        var appAuthService = new AppAuthService();

        var mockUserId = "1";
        var mockProof = "Proof";

        var authRequest = new TestAuthenticationRequest();

        authRequest.UserId = (TestVariables.USER_ID_TYPE, mockUserId);
        authRequest.Proof = (TestVariables.PROOF_TYPE ,mockProof);
        authRequest.Claims = ("", "");

        var nullExceptionIsReturn = false;

        //Act
        try
        {
            var response = await appAuthService.AuthenticateUser(authRequest)!;
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

        authRequest.UserId = (TestVariables.USER_ID_TYPE, mockUserId);
        authRequest.Proof = (TestVariables.PROOF_TYPE ,mockProof);
        authRequest.Claims = (TestVariables.CLAIM_TYPE, mockClaim);

        //Act

        var response = await appAuthService.AuthenticateUser(authRequest)!;

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

        var insertSql = $"INSERT INTO {TestVariables.TABLE} ({TestVariables.USER_ID_TYPE}, {TestVariables.PROOF_TYPE}, {TestVariables.CLAIM_TYPE}) VALUES ('{mockUserId}', '{mockProof}', '{mockClaim}')";
        await createDataOnlyDAO.CreateData(insertSql);

        var authRequest = new TestAuthenticationRequest();

        authRequest.UserId = (TestVariables.USER_ID_TYPE, mockUserId);
        authRequest.Proof = (TestVariables.PROOF_TYPE ,mockWrongProof);
        authRequest.Claims = (TestVariables.CLAIM_TYPE, mockClaim);

        //Act
        var response = await appAuthService.AuthenticateUser(authRequest)!;

        // Assert
        Assert.Null(response);

    }

    [Fact]
    public async void AppAuthServiceAuthNShould_ReturnNull_IfRequestResultInInvalidSQL()
    {
        //Arrange
        var appAuthService = new AppAuthService();

        var createDataOnlyDAO = new CreateDataOnlyDAO();

        var readDataOnlyDAO = new ReadDataOnlyDAO();

        var mockUserId = "4"; // this user id does not exist
        var mockProof = "Proof";
        var mockClaim = "Claim";
        var wrongUserIdType = "Phone";

        var authRequest = new TestAuthenticationRequest();

        authRequest.UserId = (wrongUserIdType, mockUserId);
        authRequest.Proof = (TestVariables.PROOF_TYPE ,mockProof);
        authRequest.Claims = (TestVariables.CLAIM_TYPE, mockClaim);

        //Act
        var response = await appAuthService.AuthenticateUser(authRequest)!;

        // Assert
        Assert.Null(response);

    }

    // Authorization Tests
    [Fact]
    public void AppAuthServiceAuthZShould_ReturnTrue_IfUserIsAuthorize()
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
        Assert.True(timer.Elapsed.TotalSeconds <= TestVariables.MAX_EXECUTION_TIME_IN_SECONDS);
        Assert.True(IsAuthorizeResponse == true);
    }

    [Fact]
    public void AppAuthServiceAuthZShould_ReturnThrowArgumentNullException_IfCurrentPrincpalIsNull()
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
            bool IsAuthorizeResponse = appAuthService.IsAuthorize(currentPrincipal!, requiredClaims); // Need to test for all behavior of string
        }
        catch (ArgumentNullException)
        {
            nullExceptionIsReturn = true;
        }

        // Assert
        Assert.True(nullExceptionIsReturn);
    }

    [Fact]
    public void AppAuthServiceAuthZShould_ReturnThrowArgumentNullException_IfRequiredClaimsIsNull()
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
            bool IsAuthorizeResponse = appAuthService.IsAuthorize(currentPrincipal, requiredClaims!); // Need to test for all behavior of string
        }
        catch (ArgumentNullException)
        {
            nullExceptionIsReturn = true;
        }

        // Assert
        Assert.True(nullExceptionIsReturn);
    }

    [Fact]
    public void AppAuthServiceAuthZShould_ReturnFalse_IfUserIsNotAuthorize()
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
        Assert.True(timer.Elapsed.TotalSeconds <= TestVariables.MAX_EXECUTION_TIME_IN_SECONDS);
        Assert.True(IsAuthorizeResponse == false);
    }



}
