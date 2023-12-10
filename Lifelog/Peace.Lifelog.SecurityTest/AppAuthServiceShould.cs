namespace Peace.Lifelog.SecurityTest;
using Peace.Lifelog.DataAccess;
using Peace.Lifelog.Security;
using System.Diagnostics;

public class AppAuthServiceShould : IDisposable
{
    private const int MAX_EXECUTION_TIME_IN_SECONDS = 3;
    private const string TABLE = "app_auth_service_test";
    private const string USER_ID_COLUMN_NAME = "user_id";
    private const string PROOF_COLUMN_NAME = "proof";
    private const string CLAIM_COLUMN_NAME = "claim";




    // Setup for all test
    public AppAuthServiceShould()
    {
        var DDLTransactionDAO = new DDLTransactionDAO();
        var createMockTableSql = $"CREATE TABLE {TABLE} ("
        + $"{USER_ID_COLUMN_NAME} serial  NOT NULL," +
        $"{PROOF_COLUMN_NAME} text NOT NULL," +
        $"{CLAIM_COLUMN_NAME} text NOT NULL," +
        $"PRIMARY KEY ({USER_ID_COLUMN_NAME})" +
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

    public async void AppAuthServiceShould_ReturnPrincipal_ForTheUser()
    {
        //Arrange
        var timer = new Stopwatch();

        var appAuthService = new AppAuthService();

        var createDataOnlyDAO = new CreateDataOnlyDAO();

        var readDataOnlyDAO = new ReadDataOnlyDAO();

        var mockUserId = "1";
        var mockProof = "Proof";
        var mockClaim = "Claim";

        var insertSql = $"INSERT INTO {TABLE} (user_id, proof, claim) VALUES ('{mockUserId}', '{mockProof}', '{mockClaim}')";
        await createDataOnlyDAO.CreateData(insertSql);

        var authRequest = new AuthenticationRequest();

        authRequest.UserId = mockUserId;
        authRequest.Proof = mockProof;


        //Act
        timer.Start();
        var response = await appAuthService.AuthenticateUser(authRequest, TABLE, USER_ID_COLUMN_NAME, PROOF_COLUMN_NAME, CLAIM_COLUMN_NAME);
        timer.Stop();

        //Assert
        Assert.True(response.Claims[CLAIM_COLUMN_NAME] == mockClaim);
        Assert.True(timer.Elapsed.TotalSeconds <= MAX_EXECUTION_TIME_IN_SECONDS);

    }

    [Fact]
    public async void AppAuthServiceShould_ReturnThrowArgumentNullException_IfUserIdIsNull()
    {
        //Arrange
        var timer = new Stopwatch();

        var appAuthService = new AppAuthService();

        var createDataOnlyDAO = new CreateDataOnlyDAO();

        var readDataOnlyDAO = new ReadDataOnlyDAO();

        // mockUserId is null
        var mockProof = "Proof";
        var mockClaim = "Claim";
        var mockClaimDict = new Dictionary<string, string>() {
            {"claim", mockClaim}
        };

        var authRequest = new AuthenticationRequest();

        authRequest.UserId = null;
        authRequest.Proof = mockProof;

        var nullExceptionIsReturn = false;


        //Act
        try
        {
            var response = await appAuthService.AuthenticateUser(authRequest, TABLE, USER_ID_COLUMN_NAME, PROOF_COLUMN_NAME, CLAIM_COLUMN_NAME);
        }
        catch (ArgumentNullException)
        {
            nullExceptionIsReturn = true;
        }

        // Assert
        Assert.True(nullExceptionIsReturn);

    }

    [Fact]
    public async void AppAuthServiceShould_ReturnThrowArgumentNullException_IfProofIsNull()
    {
        //Arrange
        var timer = new Stopwatch();

        var appAuthService = new AppAuthService();

        var createDataOnlyDAO = new CreateDataOnlyDAO();

        var readDataOnlyDAO = new ReadDataOnlyDAO();

        var mockUserId = "1";
        // var mockProof = ""; is Null 
        var mockClaim = "Claim";
        var mockClaimDict = new Dictionary<string, string>() {
            {"claim", mockClaim}
        };

        var authRequest = new AuthenticationRequest();

        authRequest.UserId = mockUserId;
        authRequest.Proof = null;

        var nullExceptionIsReturn = false;


        //Act
        try
        {
            var response = await appAuthService.AuthenticateUser(authRequest, TABLE, USER_ID_COLUMN_NAME, PROOF_COLUMN_NAME, CLAIM_COLUMN_NAME);
        }
        catch (ArgumentNullException)
        {
            nullExceptionIsReturn = true;
        }

        // Assert
        Assert.True(nullExceptionIsReturn);

    }

    [Fact]
    public async void AppAuthServiceShould_ReturnNull_IfUserIdIsNotFound()
    {
        //Arrange
        var timer = new Stopwatch();

        var appAuthService = new AppAuthService();

        var createDataOnlyDAO = new CreateDataOnlyDAO();

        var readDataOnlyDAO = new ReadDataOnlyDAO();

        var mockUserId = "4"; // this user id does not exist
        var mockProof = "Proof";
        var mockClaim = "Claim";
        var mockClaimDict = new Dictionary<string, string>() {
            {"claim", mockClaim}
        };

        var authRequest = new AuthenticationRequest();

        authRequest.UserId = mockUserId;
        authRequest.Proof = mockProof;


        //Act

        var response = await appAuthService.AuthenticateUser(authRequest, TABLE, USER_ID_COLUMN_NAME, PROOF_COLUMN_NAME, CLAIM_COLUMN_NAME);

        // Assert
        Assert.Null(response);

    }

    [Fact]
    public async void AppAuthServiceShould_ReturnNull_IfUserIdProofCombinationIsInvalid()
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

        var insertSql = $"INSERT INTO {TABLE} (user_id, proof, claim) VALUES ('{mockUserId}', '{mockProof}', '{mockClaim}')";
        await createDataOnlyDAO.CreateData(insertSql);

        var authRequest = new AuthenticationRequest();

        authRequest.UserId = mockUserId;
        authRequest.Proof = mockWrongProof;


        //Act

        var response = await appAuthService.AuthenticateUser(authRequest, TABLE, USER_ID_COLUMN_NAME, PROOF_COLUMN_NAME, CLAIM_COLUMN_NAME);

        // Assert
        Assert.Null(response);

    }

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