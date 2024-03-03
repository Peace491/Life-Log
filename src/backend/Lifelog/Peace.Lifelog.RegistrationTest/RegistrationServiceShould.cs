
using Peace.Lifelog.DataAccess;
using Peace.Lifelog.UserManagement;
using Peace.Lifelog.RegistrationService;

namespace Peace.Lifelog.RegistrationTest;
public class RegistrationServiceShould
{

    private const string EMAIL = "zarif.shams@gmail.com";
    private string DOB = DateTime.Today.ToString("yyyy-MM-dd");
    private const string ZIPCODE = "90716"; 


#region Input Tests

[Fact]
public void RegistrationServiceShould_HaveValidInputs()
{
    
    // Arrange
    var registrationService = new RegistrationService();

    // Act
    var validEmailFormatResponse = registrationService.CheckInputValidation(EMAIL, DOB, ZIPCODE);

    // Assert
    Assert.True(validEmailFormatResponse.HasError == false);

}

[Fact]
public void RegistrationServiceShould_HaveValidEmailFormat()
{
    // Arrange
    string invalidEmail = "invalid@com.";
    var registrationService = new RegistrationService();

    // Act
    var validEmailFormatResponse = registrationService.CheckInputValidation(invalidEmail, DOB, ZIPCODE);

    // Assert
    Assert.True(validEmailFormatResponse.HasError == false);

}

[Fact]
public void RegistrationServiceShould_EmailBeAtLeast3Chars()
{
    // Arrange
    string invalidEmail = "z@gmail.com";
    var registrationService = new RegistrationService();

    // Act
    var threeCharsResponse = registrationService.CheckInputValidation(invalidEmail, DOB, ZIPCODE);

    // Assert
    Assert.True(threeCharsResponse.HasError == false);
}

[Fact]
public void RegistrationServiceShould_EmailOnlyBeAlphanumeric()
{
    // Arrange
    string invalidEmail = "zarif&123!@gmail.com";
    var registrationService = new RegistrationService();

    // Act
    var alphanumericResponse = registrationService.CheckInputValidation(invalidEmail, DOB, ZIPCODE);

    // Assert
    Assert.True(alphanumericResponse.HasError == false);
}

[Fact]
public void RegistrationServiceShould_HaveValidDOB()
{
    // Arrange
    string invalidDOB = "1969-01-01";
    var registrationService = new RegistrationService();

    // Act
    var validDOBResponse = registrationService.CheckInputValidation(EMAIL, invalidDOB, ZIPCODE);

    // Assert
    Assert.True(validDOBResponse.HasError == false);
}

[Fact]
public void RegistrationServiceShould_HaveValidZipCode()
{
    // Arrange
    string invalidZipCode = "90623";
    var registrationService = new RegistrationService();

    // Act
    var validZipCodeResponse = registrationService.CheckInputValidation(EMAIL, DOB, invalidZipCode);

    // Assert
    Assert.True(validZipCodeResponse.HasError == false);
}

[Fact]
public void RegistrationServiceShould_InputsBeNotNull()
{
    // Arrange
    string emptyEmail= ""; 
    string emptyDOB = ""; // <-should this be empty or null?
    string emptyZipCode = ""; 
    var registrationService = new RegistrationService();

    // Act
    var inputNotNullResponse = registrationService.CheckInputValidation(emptyEmail, emptyDOB, emptyZipCode);

    // Assert
    Assert.True(inputNotNullResponse.HasError == false);
}


#endregion

}

