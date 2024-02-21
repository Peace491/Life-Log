namespace Peace.Lifelog.RegistrationTest;

using Peace.Lifelog.DataAccess;
using Peace.Lifelog.UserManagement;
using Peace.Lifelog.RegistrationService;

public class RegistrationServiceShould
{

    private const string EMAIL = "zarif.shams@gmail.com";
    private string DOB = DateTime.Today.ToString("yyyy-MM-dd");
    private const string ZIPCODE = "90716"; 
    private const string USERROLE = "Normal";

#region Invalid Input Tests
[Fact]
public void RegistrationServiceShould_HaveValidEmailFormat()
{
    // Arrange
    string invalidEmail = "invalid@com.";
    var testRegUser = new RegistrationWebService();

    // Act
    var validEmailFormatResponse = testRegUser.RegisterUser(invalidEmail, DOB, ZIPCODE, USERROLE);

    // Assert
    Assert.True(validEmailFormatResponse == false);

}

[Fact]
public void RegistrationServiceShould_EmailBeAtLeast3Chars()
{
    // Arrange
    string invalidEmail = "z@gmail.com";
    var testRegUser = new RegistrationWebService();

    // Act
    var threeCharsResponse = testRegUser.RegisterUser(invalidEmail, DOB, ZIPCODE, USERROLE);

    // Assert
    Assert.True(threeCharsResponse == false);
}

[Fact]
public void RegistrationServiceShould_EmailOnlyBeAlphanumeric()
{
    // Arrange
    string invalidEmail = "zarif&123!@gmail.com";
    var testRegUser = new RegistrationWebService();

    // Act
    var alphanumericResponse = testRegUser.RegisterUser(invalidEmail, DOB, ZIPCODE, USERROLE);

    // Assert
    Assert.True(alphanumericResponse == false);
}

[Fact]
public void RegistrationServiceShould_HaveValidDOB()
{
    // Arrange
    string invalidDOB = "1969-01-01";
    var testRegUser = new RegistrationWebService();

    // Act
    var validDOBResponse = testRegUser.RegisterUser(EMAIL, invalidDOB, ZIPCODE, USERROLE);

    // Assert
    Assert.True(validDOBResponse == false);
}

[Fact]
public void RegistrationServiceShould_HaveValidZipCode()
{
    // Arrange
    string invalidZipCode = "90623";
    var testRegUser = new RegistrationWebService();

    // Act
    var validZipCodeResponse = testRegUser.RegisterUser(EMAIL, DOB, invalidZipCode, USERROLE);

    // Assert
    Assert.True(validEmailFormatResponse == false);
}

[Fact]
public void RegistrationServiceShould_InputsBeNotNull()
{
    // Arrange
    string emptyEmail= ""; 
    string emptyDOB = ""; // <-should this be empty or null?
    string emptyZipCode = ""; 
    var testRegUser = new RegistrationWebService();

    // Act
    var inputNotNullResponse = testRegUser.RegisterUser(emptyEmail, emptyDOB, emptyZipCode, USERROLE);

    // Assert
    Assert.True(inputNotNullResponse == false);
}


#endregion

}

