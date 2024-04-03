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
    private static LogTarget logTarget = new LogTarget(createDataOnlyDAO);
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

    [Fact]
    public async void UserFormServiceShould_CreateTheUserForm()
    {
        // Arrange
        var principal = new AppPrincipal();
        principal.UserId = USER_HASH;
        principal.Claims = new Dictionary<string, string>() {{"Role", "Normal"}};

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
            foreach(int ranking in userFormData)
            {
                uniqueValues.Add(ranking);
            }
        }

        Assert.True(uniqueValues.Count == NUM_OF_CATEGORIES);
    }

    [Fact]
    public async void UserFormServiceShould_UpdateTheUserForm()
    {
        // Arrange
        var principal = new AppPrincipal();
        principal.UserId = USER_HASH;
        principal.Claims = new Dictionary<string, string>() {{"Role", "Normal"}};

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

        var updateUserFormRequest = new UpdateUserFormRequest
        {
            Principal = principal,
            MentalHealthRating = 7,
            PhysicalHealthRating = 9
        };

        var readUserFormSql = $"SELECT MentalHealthRanking FROM UserForm WHERE UserHash=\"{USER_HASH}\"";

        // Act
        var updateResponse = await userFormService.UpdateUserForm(updateUserFormRequest);

        // Assert
        Assert.True(updateResponse.HasError == false);
        
        var readUserFormResponse = await readDataOnlyDAO.ReadData(readUserFormSql);

        Assert.True(readUserFormResponse.Output != null);

        foreach (List<Object> userFormData in readUserFormResponse.Output)
        {
            foreach(int ranking in userFormData)
            {
                Assert.True(ranking == 7);
            }
        }

    }

    [Fact]
    public async void UserFormServiceShould_CheckIfTheUserFormHasBeenCompleted()
    {
        // Arrange
        var principal = new AppPrincipal();
        principal.UserId = USER_HASH;
        principal.Claims = new Dictionary<string, string>() {{"Role", "Normal"}};

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
        var isUserFormCompleted = await userFormService.IsUserFormCompleted(USER_HASH);

        // Assert
        Assert.True(isUserFormCompleted);

    }
}