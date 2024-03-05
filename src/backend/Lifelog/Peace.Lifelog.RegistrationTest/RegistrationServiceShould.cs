namespace Peace.Lifelog.RegistrationTest;

using Peace.Lifelog.DataAccess;
using Peace.Lifelog.UserManagement;
using Peace.Lifelog.RegistrationService;


public class RegistrationServiceShould
{

    private const string EMAIL = "zarif.shams@gmail.com";
    private string DOB = DateTime.Today.ToString("yyyy-MM-dd");
    private const string ZIPCODE = "90716";


    #region Input validation Tests

    [Fact]
    public async void RegistrationServiceShould_HaveValidInputs()
    {

        // Arrange
        var registrationService = new RegistrationService();

        // Act
        var validEmailFormatResponse = await registrationService.CheckInputValidation(EMAIL, DOB, ZIPCODE);

        // Assert
        Assert.True(validEmailFormatResponse.HasError == false);

    }

    [Fact]
    public async void RegistrationServiceShould_HaveValidEmailFormat()
    {
        // Arrange
        string invalidEmail = "invalid@com.";
        var registrationService = new RegistrationService();

        // Act
        var validEmailFormatResponse = await registrationService.CheckInputValidation(invalidEmail, DOB, ZIPCODE);

        // Assert
        Assert.True(validEmailFormatResponse.HasError == false);

    }

    [Fact]
    public async void RegistrationServiceShould_EmailBeAtLeast3Chars()
    {
        // Arrange
        string invalidEmail = "z@gmail.com";
        var registrationService = new RegistrationService();

        // Act
        var threeCharsResponse = await registrationService.CheckInputValidation(invalidEmail, DOB, ZIPCODE);

        // Assert
        Assert.True(threeCharsResponse.HasError == false);
    }

    [Fact]
    public async void RegistrationServiceShould_EmailOnlyBeAlphanumeric()
    {
        // Arrange
        string invalidEmail = "zarif&123!@gmail.com";
        var registrationService = new RegistrationService();

        // Act
        var alphanumericResponse = await registrationService.CheckInputValidation(invalidEmail, DOB, ZIPCODE);

        // Assert
        Assert.True(alphanumericResponse.HasError == false);
    }

    [Fact]
    public async void RegistrationServiceShould_HaveValidDOB()
    {
        // Arrange
        string invalidDOB = "1969-01-01";
        var registrationService = new RegistrationService();

        // Act
        var validDOBResponse = await registrationService.CheckInputValidation(EMAIL, invalidDOB, ZIPCODE);

        // Assert
        Assert.True(validDOBResponse.HasError == false);
    }

    [Fact]
    public async void RegistrationServiceShould_HaveValidZipCode()
    {
        // Arrange
        string invalidZipCode = "90623";
        var registrationService = new RegistrationService();

        // Act
        var validZipCodeResponse = await registrationService.CheckInputValidation(EMAIL, DOB, invalidZipCode);

        // Assert
        Assert.True(validZipCodeResponse.HasError == false);
    }

    [Fact]
    public async void RegistrationServiceShould_InputsBeNotNull()
    {
        // Arrange
        string emptyEmail = "";
        string emptyDOB = ""; // <-should this be empty or null?
        string emptyZipCode = "";
        var registrationService = new RegistrationService();

        // Act
        var inputNotNullResponse = await registrationService.CheckInputValidation(emptyEmail, emptyDOB, emptyZipCode);

        // Assert
        Assert.True(inputNotNullResponse.HasError == false);
    }


    #endregion



    #region Create user tests

    [Fact]
    public async void RegistrationServiceShould_RegisterNormalUser()
    {
        // Arrange
        // init view and delete DAO
        var readDataOnlyDAO = new ReadDataOnlyDAO();
        var deleteDataOnlyDAO = new DeleteDataOnlyDAO();
        string readSQL = $"";
        string deleteSQL = $"";
        // create reg object
        var registrationService = new RegistrationService();
        
        // Act
        var sucessfulRegistrationResponse = await  registrationService.RegisterNormalUser(EMAIL, DOB, ZIPCODE);

        // Assert
        Assert.True(sucessfulRegistrationResponse.HasError == false);

    }

    [Fact]
    public async void RegistrationServiceShould_RegisterAdminUser()
    {
        // Arrange
        var registrationService = new RegistrationService();

        // Act
        var sucessfulRegistrationResponse = await registrationService.RegisterAdminUser(EMAIL, DOB, ZIPCODE);

        // Assert
        Assert.True(sucessfulRegistrationResponse.HasError == false);

    }



    #endregion

}

