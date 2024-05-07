namespace Peace.Lifelog.UserFormShould;

using Peace.Lifelog.UserManagement;
using Peace.Lifelog.DataAccess;
using Peace.Lifelog.UserForm;
using Peace.Lifelog.Logging;
using Peace.Lifelog.Security;
using Xunit.Sdk;
using Peace.Lifelog.Infrastructure;

public class UserFormServiceShould : IAsyncLifetime, IDisposable
{
    private const int NUM_OF_CATEGORIES = 10;

    private static ICreateDataOnlyDAO createDataOnlyDAO = new CreateDataOnlyDAO();
    private static IReadDataOnlyDAO readDataOnlyDAO = new ReadDataOnlyDAO();
    private static IUpdateDataOnlyDAO updateDataOnlyDAO = new UpdateDataOnlyDAO();
    private static LogTarget logTarget = new LogTarget(createDataOnlyDAO, readDataOnlyDAO);
    private static Logging logging = new Logging(logTarget);
    private static LifelogAuthService lifelogAuthService = new LifelogAuthService();
    private static IUserFormRepo userFormRepo = new UserFormRepo(createDataOnlyDAO, readDataOnlyDAO, updateDataOnlyDAO);
    private static IUserFormService userFormService = new UserFormService(userFormRepo, lifelogAuthService, logging);


    private const string USER_ID = "TestUserFormRepoAccount";
    private string USER_HASH = "";
    private const string ROLE = "Normal";

    private string DOB = DateTime.Today.ToString("yyyy-MM-dd");

    private const string ZIP_CODE = "90704";

    private const int MAX_TIME_IN_SECOND = 3;

    public async Task InitializeAsync()
    {

        var appUserManagementService = new AppUserManagementService();
        var lifelogUserManagementService = new LifelogUserManagementService();

        var testLifelogAccountRequest = new LifelogAccountRequest();
        testLifelogAccountRequest.UserId = ("UserId", USER_ID);
        testLifelogAccountRequest.Role = ("Role", ROLE);

        var deleteAccountResponse = await appUserManagementService.DeleteAccount(testLifelogAccountRequest); // Make sure no test account with the same name exist

        var testLifelogProfileRequest = new LifelogProfileRequest();
        testLifelogProfileRequest.DOB = ("DOB", DOB);
        testLifelogProfileRequest.ZipCode = ("ZipCode", ZIP_CODE);


        var createAccountResponse = await lifelogUserManagementService.CreateLifelogUser(testLifelogAccountRequest, testLifelogProfileRequest);

        if (createAccountResponse.Output is not null)
        {
            foreach (string output in createAccountResponse.Output)
            {
                USER_HASH = output;
            }
        }

    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        var appUserManagementService = new AppUserManagementService();
        var testLifelogAccountRequest = new LifelogAccountRequest();
        testLifelogAccountRequest.UserId = ("UserId", USER_ID);
        var deleteAccountResponse = appUserManagementService.DeleteAccount(testLifelogAccountRequest);
    }

    #region Create User Form
    [Theory]
    [InlineData(1, 2, 3, 4, 5, 6, 7, 8, 9, 10)]
    [InlineData(10, 9, 8, 7, 6, 5, 4, 3, 2, 1)]
    [InlineData(2, 4, 1, 3, 5, 6, 8, 7, 9, 10)]
    public async Task UserFormServiceCreateUserFormShould_CreateTheUserForm(
        int mentalHealthRating,
        int physicalHealthRating,
        int outdoorRating,
        int sportRating,
        int artRating,
        int hobbyRating,
        int thrillRating,
        int travelRating,
        int volunteeringRating,
        int foodRating
    )
    {
        // Arrange
        var principal = new AppPrincipal();
        principal.UserId = USER_HASH;
        principal.Claims = new Dictionary<string, string>() { { "Role", "Normal" } };

        var createUserFormRequest = new CreateUserFormRequest
        {
            Principal = principal,
            MentalHealthRating = mentalHealthRating,
            PhysicalHealthRating = physicalHealthRating,
            OutdoorRating = outdoorRating,
            SportRating = sportRating,
            ArtRating = artRating,
            HobbyRating = hobbyRating,
            ThrillRating = thrillRating,
            TravelRating = travelRating,
            VolunteeringRating = volunteeringRating,
            FoodRating = foodRating
        };

        var readUserFormSql = $"SELECT MentalHealthRanking, PhysicalHealthRanking, OutdoorRanking, SportRanking, ArtRanking, HobbyRanking, ThrillRanking, TravelRanking, VolunteeringRanking, FoodRanking FROM UserForm WHERE UserHash=\"{USER_HASH}\"";

        // Act
        var response = await userFormService.CreateUserForm(createUserFormRequest);

        // Assert
        Assert.True(response.HasError == false);

        HashSet<int> uniqueValues = new HashSet<int>();
        var readUserFormResponse = await readDataOnlyDAO.ReadData(readUserFormSql);

        Assert.True(readUserFormResponse.Output != null);

        foreach (List<Object> userFormData in readUserFormResponse.Output)
        {
            foreach (int ranking in userFormData)
            {
                uniqueValues.Add(ranking);
            }
        }

        Assert.True(uniqueValues.Count == NUM_OF_CATEGORIES);
    }

    [Fact]
    public async Task UserFormServiceCreateUserFormShould_ReturnAnErrorIfRequestIsNull()
    {
        // Arrange

        var readUserFormSql = $"SELECT MentalHealthRanking, PhysicalHealthRanking, OutdoorRanking, SportRanking, ArtRanking, HobbyRanking, ThrillRanking, TravelRanking, VolunteeringRanking, FoodRanking FROM UserForm WHERE UserHash=\"{USER_HASH}\"";

        // Act
        var response = await userFormService.CreateUserForm(null!);

        // Assert
        Assert.True(response.HasError == true);
        Assert.True(response.ErrorMessage == "User Form Request Must Not Be Null");

        var readUserFormResponse = await readDataOnlyDAO.ReadData(readUserFormSql);

        Assert.True(readUserFormResponse.Output == null);
    }

    [Fact]
    public async Task UserFormServiceCreateUserFormShould_ReturnAnErrorIfAppPrincipalIsNull()
    {
        // Arrange
        var createUserFormRequest = new CreateUserFormRequest();


        var readUserFormSql = $"SELECT MentalHealthRanking, PhysicalHealthRanking, OutdoorRanking, SportRanking, ArtRanking, HobbyRanking, ThrillRanking, TravelRanking, VolunteeringRanking, FoodRanking FROM UserForm WHERE UserHash=\"{USER_HASH}\"";

        // Act
        var response = await userFormService.CreateUserForm(createUserFormRequest);

        // Assert
        Assert.True(response.HasError == true);
        Assert.True(response.ErrorMessage == "App Principal must not be empty");

        var readUserFormResponse = await readDataOnlyDAO.ReadData(readUserFormSql);

        Assert.True(readUserFormResponse.Output == null);
    }

    [Fact]
    public async Task UserFormServiceCreateUserFormShould_ReturnAnErrorIfUserHashIsEmpty()
    {
        // Arrange
        var createUserFormRequest = new CreateUserFormRequest();

        var appPrincipal = new AppPrincipal();
        createUserFormRequest.Principal = appPrincipal;


        var readUserFormSql = $"SELECT MentalHealthRanking, PhysicalHealthRanking, OutdoorRanking, SportRanking, ArtRanking, HobbyRanking, ThrillRanking, TravelRanking, VolunteeringRanking, FoodRanking FROM UserForm WHERE UserHash=\"{USER_HASH}\"";

        // Act
        var response = await userFormService.CreateUserForm(createUserFormRequest);

        // Assert
        Assert.True(response.HasError == true);
        Assert.True(response.ErrorMessage == "User Hash must not be empty");

        var readUserFormResponse = await readDataOnlyDAO.ReadData(readUserFormSql);

        Assert.True(readUserFormResponse.Output == null);
    }

    [Fact]
    public async Task UserFormServiceCreateUserFormShould_ReturnAnErrorIfClaimsIsNull()
    {
        // Arrange
        var createUserFormRequest = new CreateUserFormRequest();

        var appPrincipal = new AppPrincipal();
        appPrincipal.UserId = USER_HASH;
        appPrincipal.Claims = null;
        createUserFormRequest.Principal = appPrincipal;


        var readUserFormSql = $"SELECT MentalHealthRanking, PhysicalHealthRanking, OutdoorRanking, SportRanking, ArtRanking, HobbyRanking, ThrillRanking, TravelRanking, VolunteeringRanking, FoodRanking FROM UserForm WHERE UserHash=\"{USER_HASH}\"";

        // Act
        var response = await userFormService.CreateUserForm(createUserFormRequest);

        // Assert
        Assert.True(response.HasError == true);
        Assert.True(response.ErrorMessage == "Claims must not be empty");

        var readUserFormResponse = await readDataOnlyDAO.ReadData(readUserFormSql);

        Assert.True(readUserFormResponse.Output == null);
    }

    [Fact]
    public async Task UserFormServiceCreateUserFormShould_ReturnAnErrorIfClaimsDoesntContainsRole()
    {
        // Arrange
        var createUserFormRequest = new CreateUserFormRequest();

        var appPrincipal = new AppPrincipal();
        appPrincipal.UserId = USER_HASH;
        appPrincipal.Claims = new Dictionary<string, string>() { { "Roles", "Normal" } };
        createUserFormRequest.Principal = appPrincipal;


        var readUserFormSql = $"SELECT MentalHealthRanking, PhysicalHealthRanking, OutdoorRanking, SportRanking, ArtRanking, HobbyRanking, ThrillRanking, TravelRanking, VolunteeringRanking, FoodRanking FROM UserForm WHERE UserHash=\"{USER_HASH}\"";

        // Act
        var response = await userFormService.CreateUserForm(createUserFormRequest);

        // Assert
        Assert.True(response.HasError == true);
        Assert.True(response.ErrorMessage == "Claims must contain the user role");

        var readUserFormResponse = await readDataOnlyDAO.ReadData(readUserFormSql);

        Assert.True(readUserFormResponse.Output == null);
    }

    [Fact]
    public async Task UserFormServiceCreateUserFormShould_ReturnAnErrorIfUserIsUnAuthorized()
    {
        // Arrange
        var appPrincipal = new AppPrincipal();
        appPrincipal.UserId = USER_HASH;
        appPrincipal.Claims = new Dictionary<string, string>() { { "Role", "Fake" } };

        var createUserFormRequest = new CreateUserFormRequest
        {
            Principal = appPrincipal,
            MentalHealthRating = 9,
            PhysicalHealthRating = 7,
            OutdoorRating = 5,
            SportRating = 8,
            ArtRating = 3,
            HobbyRating = 6,
            ThrillRating = 2,
            TravelRating = 4,
            VolunteeringRating = 10,
            FoodRating = 1
        };


        var readUserFormSql = $"SELECT MentalHealthRanking, PhysicalHealthRanking, OutdoorRanking, SportRanking, ArtRanking, HobbyRanking, ThrillRanking, TravelRanking, VolunteeringRanking, FoodRanking FROM UserForm WHERE UserHash=\"{USER_HASH}\"";

        // Act
        var response = await userFormService.CreateUserForm(createUserFormRequest);

        // Assert
        Assert.True(response.HasError == true);
        Assert.True(response.ErrorMessage == "The User Is Not Authorized To Use The User Form");

        var readUserFormResponse = await readDataOnlyDAO.ReadData(readUserFormSql);

        Assert.True(readUserFormResponse.Output == null);
    }

    [Theory]
    [InlineData(1, 2, 3, 4, 5, 6, 7, 8, 9, 11)]
    [InlineData(10, 9, 8, 7, 6, 5, 4, 3, 2, -1)]
    [InlineData(100, 4, 1, 3, 5, 6, 8, 7, 2, 10)]
    public async Task UserFormServiceCreateUserFormShould_ReturnAnErrorIfTheUserFormRankingsAreNotInRange(
        int mentalHealthRating,
        int physicalHealthRating,
        int outdoorRating,
        int sportRating,
        int artRating,
        int hobbyRating,
        int thrillRating,
        int travelRating,
        int volunteeringRating,
        int foodRating
    )
    {
        // Arrange
        var principal = new AppPrincipal();
        principal.UserId = USER_HASH;
        principal.Claims = new Dictionary<string, string>() { { "Role", "Normal" } };

        var createUserFormRequest = new CreateUserFormRequest
        {
            Principal = principal,
            MentalHealthRating = mentalHealthRating,
            PhysicalHealthRating = physicalHealthRating,
            OutdoorRating = outdoorRating,
            SportRating = sportRating,
            ArtRating = artRating,
            HobbyRating = hobbyRating,
            ThrillRating = thrillRating,
            TravelRating = travelRating,
            VolunteeringRating = volunteeringRating,
            FoodRating = foodRating
        };

        var readUserFormSql = $"SELECT MentalHealthRanking, PhysicalHealthRanking, OutdoorRanking, SportRanking, ArtRanking, HobbyRanking, ThrillRanking, TravelRanking, VolunteeringRanking, FoodRanking FROM UserForm WHERE UserHash=\"{USER_HASH}\"";

        // Act
        var response = await userFormService.CreateUserForm(createUserFormRequest);

        // Assert
        Assert.True(response.HasError == true);
        Assert.True(response.ErrorMessage == "The LLI rankings are not in range");

        var readUserFormResponse = await readDataOnlyDAO.ReadData(readUserFormSql);

        Assert.True(readUserFormResponse.Output == null);
    }

    [Theory]
    [InlineData(1, 1, 3, 4, 5, 6, 7, 8, 9, 10)]
    [InlineData(10, 9, 9, 7, 6, 5, 4, 3, 2, 1)]
    [InlineData(2, 4, 1, 3, 5, 6, 8, 7, 7, 10)]
    public async Task UserFormServiceCreateUserFormShould_ReturnAnErrorIfTheUserFormRankingsAreNotUnique(
        int mentalHealthRating,
        int physicalHealthRating,
        int outdoorRating,
        int sportRating,
        int artRating,
        int hobbyRating,
        int thrillRating,
        int travelRating,
        int volunteeringRating,
        int foodRating
    )
    {
        // Arrange
        var principal = new AppPrincipal();
        principal.UserId = USER_HASH;
        principal.Claims = new Dictionary<string, string>() { { "Role", "Normal" } };

        var createUserFormRequest = new CreateUserFormRequest
        {
            Principal = principal,
            MentalHealthRating = mentalHealthRating,
            PhysicalHealthRating = physicalHealthRating,
            OutdoorRating = outdoorRating,
            SportRating = sportRating,
            ArtRating = artRating,
            HobbyRating = hobbyRating,
            ThrillRating = thrillRating,
            TravelRating = travelRating,
            VolunteeringRating = volunteeringRating,
            FoodRating = foodRating
        };

        var readUserFormSql = $"SELECT MentalHealthRanking, PhysicalHealthRanking, OutdoorRanking, SportRanking, ArtRanking, HobbyRanking, ThrillRanking, TravelRanking, VolunteeringRanking, FoodRanking FROM UserForm WHERE UserHash=\"{USER_HASH}\"";

        // Act
        var response = await userFormService.CreateUserForm(createUserFormRequest);

        // Assert
        Assert.True(response.HasError == true);
        Assert.True(response.ErrorMessage == "The LLI rankings are not unique");

        var readUserFormResponse = await readDataOnlyDAO.ReadData(readUserFormSql);

        Assert.True(readUserFormResponse.Output == null);
    }

    [Fact]
    public async Task UserFormServiceCreateUserFormShould_ReturnAnErrorIfUserDoesntExist()
    {
        // Arrange
        var appPrincipal = new AppPrincipal();
        appPrincipal.UserId = "Fake User";
        appPrincipal.Claims = new Dictionary<string, string>() { { "Role", "Normal" } };

        var createUserFormRequest = new CreateUserFormRequest
        {
            Principal = appPrincipal,
            MentalHealthRating = 9,
            PhysicalHealthRating = 7,
            OutdoorRating = 5,
            SportRating = 8,
            ArtRating = 3,
            HobbyRating = 6,
            ThrillRating = 2,
            TravelRating = 4,
            VolunteeringRating = 10,
            FoodRating = 1
        };

        // Act
        var response = await userFormService.CreateUserForm(createUserFormRequest);

        // Assert
        Assert.True(response.HasError == true);
    }    


    #endregion

    #region Read User Form Ranking
    [Theory]
    [InlineData(1, 2, 3, 4, 5, 6, 7, 8, 9, 10)]
    [InlineData(10, 9, 8, 7, 6, 5, 4, 3, 2, 1)]
    [InlineData(2, 4, 1, 3, 5, 6, 8, 7, 9, 10)]
    public async Task UserFormServiceReadUserFormRankingShould_ReadUserFormRanking(
        int mentalHealthRating,
        int physicalHealthRating,
        int outdoorRating,
        int sportRating,
        int artRating,
        int hobbyRating,
        int thrillRating,
        int travelRating,
        int volunteeringRating,
        int foodRating
    )
    {
        // Arrange
        var principal = new AppPrincipal();
        principal.UserId = USER_HASH;
        principal.Claims = new Dictionary<string, string>() { { "Role", "Normal" } };

        var createUserFormRequest = new CreateUserFormRequest
        {
            Principal = principal,
            MentalHealthRating = mentalHealthRating,
            PhysicalHealthRating = physicalHealthRating,
            OutdoorRating = outdoorRating,
            SportRating = sportRating,
            ArtRating = artRating,
            HobbyRating = hobbyRating,
            ThrillRating = thrillRating,
            TravelRating = travelRating,
            VolunteeringRating = volunteeringRating,
            FoodRating = foodRating
        };

        var createResponse = await userFormService.CreateUserForm(createUserFormRequest);

        // Act
        var getRankingResponse = await userFormService.GetUserFormRanking(principal);

        // Assert
        Assert.True(getRankingResponse.HasError == false);
        Assert.True(getRankingResponse.Output != null);

        foreach (UserFormRanking userFormRanking in getRankingResponse.Output)
        {
            Assert.True(userFormRanking.MentalHealthRating == createUserFormRequest.MentalHealthRating);
            Assert.True(userFormRanking.PhysicalHealthRating == createUserFormRequest.PhysicalHealthRating);
            Assert.True(userFormRanking.OutdoorRating == createUserFormRequest.OutdoorRating);
            Assert.True(userFormRanking.SportRating == createUserFormRequest.SportRating);
            Assert.True(userFormRanking.ArtRating == createUserFormRequest.ArtRating);
            Assert.True(userFormRanking.HobbyRating == createUserFormRequest.HobbyRating);
            Assert.True(userFormRanking.ThrillRating == createUserFormRequest.ThrillRating);
            Assert.True(userFormRanking.TravelRating == createUserFormRequest.TravelRating);
            Assert.True(userFormRanking.VolunteeringRating == createUserFormRequest.VolunteeringRating);
            Assert.True(userFormRanking.FoodRating == createUserFormRequest.FoodRating);
        }
    }

    [Fact]
    public async Task UserFormServiceReadUserFormRankingShould_ReturnErrorIfAppPrincipalIsNull()
    {
        // Arrange
        AppPrincipal? principal = null;

        // Act
        var getRankingResponse = await userFormService.GetUserFormRanking(principal!);

        // Assert
        Assert.True(getRankingResponse.HasError == true);
        Assert.True(getRankingResponse.ErrorMessage == "App Principal must not be empty");
    }

    [Fact]
    public async Task UserFormServiceReadUserFormRankingShould_ReturnErrorIfUserHashIsEmpty()
    {
        // Arrange
        AppPrincipal principal = new AppPrincipal();

        // Act
        var getRankingResponse = await userFormService.GetUserFormRanking(principal!);

        // Assert
        Assert.True(getRankingResponse.HasError == true);
        Assert.True(getRankingResponse.ErrorMessage == "User Hash must not be empty");
    }

    [Fact]
    public async Task UserFormServiceReadUserFormRankingShould_ReturnErrorIfClaimsIsEmpty()
    {
        // Arrange
        AppPrincipal principal = new AppPrincipal() { UserId = USER_HASH, Claims = null };

        // Act
        var getRankingResponse = await userFormService.GetUserFormRanking(principal!);

        // Assert
        Assert.True(getRankingResponse.HasError == true);
        Assert.True(getRankingResponse.ErrorMessage == "Claims must not be empty");
    }

    [Fact]
    public async Task UserFormServiceReadUserFormRankingShould_ReturnErrorIfClaimsIsInvalid()
    {
        // Arrange
        AppPrincipal principal = new AppPrincipal() 
        { UserId = USER_HASH, 
        Claims = new Dictionary<string, string>() {{"Roles", "Normal"}} };

        // Act
        var getRankingResponse = await userFormService.GetUserFormRanking(principal!);

        // Assert
        Assert.True(getRankingResponse.HasError == true);
        Assert.True(getRankingResponse.ErrorMessage == "Claims must contain the user role");
    }

    [Fact]
    public async Task UserFormServiceReadUserFormRankingShould_ReturnErrorIfUserIsUnAuthorized()
    {
        // Arrange
        AppPrincipal principal = new AppPrincipal() 
        { UserId = USER_HASH, 
        Claims = new Dictionary<string, string>() {{"Role", "Fake"}} };

        // Act
        var getRankingResponse = await userFormService.GetUserFormRanking(principal!);

        // Assert
        Assert.True(getRankingResponse.HasError == true);
        Assert.True(getRankingResponse.ErrorMessage == "The User Is Not Authorized To Use The User Form");
    }

    [Fact]
    public async Task UserFormServiceReadUserFormRankingShould_ReturnErrorIfUserDoesntExist()
    {
        // Arrange
        AppPrincipal principal = new AppPrincipal() 
        { UserId = "Fake User", 
        Claims = new Dictionary<string, string>() {{"Role", "Normal"}} };

        // Act
        var getRankingResponse = await userFormService.GetUserFormRanking(principal!);

        // Assert
        Assert.True(getRankingResponse.HasError == true);
    }

    #endregion
    #region Update User Form
    [Theory]
    [InlineData(1, 2, 3, 4, 5, 6, 7, 8, 9, 10)]
    public async Task UserFormServiceUpdateUserFormShould_UpdateTheUserForm(
        int mentalHealthRating,
        int physicalHealthRating,
        int outdoorRating,
        int sportRating,
        int artRating,
        int hobbyRating,
        int thrillRating,
        int travelRating,
        int volunteeringRating,
        int foodRating
    )
    {
        // Arrange
        var principal = new AppPrincipal();
        principal.UserId = USER_HASH;
        principal.Claims = new Dictionary<string, string>() { { "Role", "Normal" } };

        var createUserFormRequest = new CreateUserFormRequest
        {
            Principal = principal,
            MentalHealthRating = mentalHealthRating,
            PhysicalHealthRating = physicalHealthRating,
            OutdoorRating = outdoorRating,
            SportRating = sportRating,
            ArtRating = artRating,
            HobbyRating = hobbyRating,
            ThrillRating = thrillRating,
            TravelRating = travelRating,
            VolunteeringRating = volunteeringRating,
            FoodRating = foodRating
        };

        var response = await userFormService.CreateUserForm(createUserFormRequest);

        var updateUserFormRequest = new UpdateUserFormRequest
        {
            Principal = principal,
            MentalHealthRating = 2,
            PhysicalHealthRating = 1
        };

        // Act
        var updateResponse = await userFormService.UpdateUserForm(updateUserFormRequest);

        // Assert
        Assert.True(updateResponse.HasError == false);

        var getRankingResponse = await userFormService.GetUserFormRanking(principal);

        Assert.True(getRankingResponse.Output != null);
        foreach (UserFormRanking userFormRanking in getRankingResponse.Output) {
            Assert.True(userFormRanking.MentalHealthRating == updateUserFormRequest.MentalHealthRating);
            Assert.True(userFormRanking.MentalHealthRating != createUserFormRequest.MentalHealthRating);

            Assert.True(userFormRanking.PhysicalHealthRating == updateUserFormRequest.PhysicalHealthRating);
            Assert.True(userFormRanking.PhysicalHealthRating != createUserFormRequest.PhysicalHealthRating);
        }

    }

    [Fact]
    public async Task UserFormServiceUpdateUserFormShould_ReturnErrorIfRequestIsNull() {
        // Arrange

        // Act
        var updateResponse = await userFormService.UpdateUserForm(null!);

        // Assert
        Assert.True(updateResponse.HasError == true);
        Assert.True(updateResponse.ErrorMessage == "User Form Request Must Not Be Null");
    }

    [Fact]
    public async Task UserFormServiceUpdateUserFormShould_ReturnErrorIfPrincipalIsEmpty() {
        // Arrange
        var updateUserFormRequest = new UpdateUserFormRequest();

        // Act
        var updateResponse = await userFormService.UpdateUserForm(updateUserFormRequest);

        // Assert
        Assert.True(updateResponse.HasError == true);
        Assert.True(updateResponse.ErrorMessage == "App Principal must not be empty");
    }

    [Fact]
    public async Task UserFormServiceUpdateUserFormShould_ReturnErrorIfUserHashIsEmpty() {
        // Arrange
        var updateUserFormRequest = new UpdateUserFormRequest();
        updateUserFormRequest.Principal = new AppPrincipal();

        // Act
        var updateResponse = await userFormService.UpdateUserForm(updateUserFormRequest);

        // Assert
        Assert.True(updateResponse.HasError == true);
        Assert.True(updateResponse.ErrorMessage == "User Hash must not be empty");
    }

    [Fact]
    public async Task UserFormServiceUpdateUserFormShould_ReturnErrorIfClaimsIsEmpty() {
        // Arrange
        var updateUserFormRequest = new UpdateUserFormRequest();
        updateUserFormRequest.Principal = new AppPrincipal(){UserId = USER_HASH, Claims = null};

        // Act
        var updateResponse = await userFormService.UpdateUserForm(updateUserFormRequest);

        // Assert
        Assert.True(updateResponse.HasError == true);
        Assert.True(updateResponse.ErrorMessage == "Claims must not be empty");
    }

    [Fact]
    public async Task UserFormServiceUpdateUserFormShould_ReturnErrorIfClaimsIsInvalid() {
        // Arrange
        var updateUserFormRequest = new UpdateUserFormRequest();
        updateUserFormRequest.Principal = new AppPrincipal(){UserId = USER_HASH, Claims = new Dictionary<string, string>(){{"Roles", "Normal"}}};

        // Act
        var updateResponse = await userFormService.UpdateUserForm(updateUserFormRequest);

        // Assert
        Assert.True(updateResponse.HasError == true);
        Assert.True(updateResponse.ErrorMessage == "Claims must contain the user role");
    }

    [Fact]
    public async Task UserFormServiceUpdateUserFormShould_ReturnErrorIfUserIsUnAuthorizedForUserForm() {
        // Arrange
        var updateUserFormRequest = new UpdateUserFormRequest();
        updateUserFormRequest.Principal = new AppPrincipal(){UserId = USER_HASH, Claims = new Dictionary<string, string>(){{"Role", "Fake"}}};

        // Act
        var updateResponse = await userFormService.UpdateUserForm(updateUserFormRequest);

        // Assert
        Assert.True(updateResponse.HasError == true);
        Assert.True(updateResponse.ErrorMessage == "The User Is Not Authorized To Use The User Form");
    }

    [Fact]
    public async Task UserFormServiceUpdateUserFormShould_ReturnErrorIfUserFormRankingIsOutOfRange() {
        // Arrange
        var updateUserFormRequest = new UpdateUserFormRequest();
        updateUserFormRequest.Principal = new AppPrincipal(){UserId = USER_HASH, Claims = new Dictionary<string, string>(){{"Role", "Normal"}}};
        updateUserFormRequest.MentalHealthRating = 11;

        // Act
        var updateResponse = await userFormService.UpdateUserForm(updateUserFormRequest);

        // Assert
        Assert.True(updateResponse.HasError == true);
        Assert.True(updateResponse.ErrorMessage == "The LLI rankings are not in range");
    }

    [Fact]
    public async Task UserFormServiceUpdateUserFormShould_ReturnErrorIfUserFormRankingIsNotUnique() {
        // Arrange
        var updateUserFormRequest = new UpdateUserFormRequest();
        updateUserFormRequest.Principal = new AppPrincipal(){UserId = USER_HASH, Claims = new Dictionary<string, string>(){{"Role", "Normal"}}};
        updateUserFormRequest.MentalHealthRating = 9;
        updateUserFormRequest.ArtRating = 9;

        // Act
        var updateResponse = await userFormService.UpdateUserForm(updateUserFormRequest);

        // Assert
        Assert.True(updateResponse.HasError == true);
        Assert.True(updateResponse.ErrorMessage == "The LLI rankings are not unique");
    }

    [Fact]
    public async Task UserFormServiceUpdateUserFormShould_ReturnErrorIfUserDoesntExist() {
        // Arrange
        var updateUserFormRequest = new UpdateUserFormRequest();
        updateUserFormRequest.Principal = new AppPrincipal(){UserId = "Fake User", Claims = new Dictionary<string, string>(){{"Role", "Normal"}}};
        updateUserFormRequest.MentalHealthRating = 9;
        updateUserFormRequest.ArtRating = 9;

        // Act
        var updateResponse = await userFormService.UpdateUserForm(updateUserFormRequest);

        // Assert
        Assert.True(updateResponse.HasError == true);
    }

    #endregion

    #region User Form Completion
    [Fact]
    public async void UserFormServiceCheckUserFormCompletionShould_CheckIfTheUserFormHasBeenCompleted()
    {
        // Arrange
        var principal = new AppPrincipal();
        principal.UserId = USER_HASH;
        principal.Claims = new Dictionary<string, string>() { { "Role", "Normal" } };

        var createUserFormRequest = new CreateUserFormRequest
        {
            Principal = principal,
            MentalHealthRating = 9,
            PhysicalHealthRating = 7,
            OutdoorRating = 5,
            SportRating = 8,
            ArtRating = 3,
            HobbyRating = 6,
            ThrillRating = 2,
            TravelRating = 4,
            VolunteeringRating = 10,
            FoodRating = 1
        };


        var response = await userFormService.CreateUserForm(createUserFormRequest);

        // Act
        var isUserFormCompleted = await userFormService.IsUserFormCompleted(principal);

        // Assert
        Assert.True(isUserFormCompleted);
    }

    [Fact]
    public async void UserFormServiceCheckUserFormCompletionShould_ReturnFalseIfUserFormIsNotCompleted()
    {
        // Arrange
        var principal = new AppPrincipal();
        principal.UserId = USER_HASH;
        principal.Claims = new Dictionary<string, string>() { { "Role", "Normal" } };

        // Act
        var isUserFormCompleted = await userFormService.IsUserFormCompleted(principal);

        // Assert
        Assert.False(isUserFormCompleted);
    }

    [Fact]
    public async void UserFormServiceCheckUserFormCompletionShould_ReturnFalseIfAppPrincipalIsNull()
    {
        // Arrange

        // Act
        var isUserFormCompleted = await userFormService.IsUserFormCompleted(null!);

        // Assert
        Assert.False(isUserFormCompleted);
    }

    [Fact]
    public async void UserFormServiceCheckUserFormCompletionShould_ReturnFalseIfUserHashINull()
    {
        // Arrange
        var principal = new AppPrincipal();
        principal.UserId = null!;
        principal.Claims = new Dictionary<string, string>() { { "Role", "Normal" } };

        // Act
        var isUserFormCompleted = await userFormService.IsUserFormCompleted(principal);

        // Assert
        Assert.False(isUserFormCompleted);
    }

    [Fact]
    public async void UserFormServiceCheckUserFormCompletionShould_ReturnFalseIfClaimsIsNull()
    {
        // Arrange
        var principal = new AppPrincipal();
        principal.UserId = USER_HASH;
        principal.Claims = null;

        // Act
        var isUserFormCompleted = await userFormService.IsUserFormCompleted(principal);

        // Assert
        Assert.False(isUserFormCompleted);
    }

    [Fact]
    public async void UserFormServiceCheckUserFormCompletionShould_ReturnFalseIfClaimsDoesntContainRole()
    {
        // Arrange
        var principal = new AppPrincipal();
        principal.UserId = USER_HASH;
        principal.Claims = new Dictionary<string, string>() { { "Roles", "Normal" } };

        // Act
        var isUserFormCompleted = await userFormService.IsUserFormCompleted(principal);

        // Assert
        Assert.False(isUserFormCompleted);
    }

    [Fact]
    public async void UserFormServiceCheckUserFormCompletionShould_ReturnFalseIfUserIsUnAuthorized()
    {
        // Arrange
        var principal = new AppPrincipal();
        principal.UserId = USER_HASH;
        principal.Claims = new Dictionary<string, string>() { { "Role", "Fake" } };

        // Act
        var isUserFormCompleted = await userFormService.IsUserFormCompleted(principal);

        // Assert
        Assert.False(isUserFormCompleted);
    }

    [Fact]
    public async void UserFormServiceCheckUserFormCompletionShould_ReturnFalseIfUserDoesntExist()
    {
        // Arrange
        var principal = new AppPrincipal();
        principal.UserId = "Fake User";
        principal.Claims = new Dictionary<string, string>() { { "Role", "Normal" } };

        // Act
        var isUserFormCompleted = await userFormService.IsUserFormCompleted(principal);

        // Assert
        Assert.False(isUserFormCompleted);
    }

    #endregion
}