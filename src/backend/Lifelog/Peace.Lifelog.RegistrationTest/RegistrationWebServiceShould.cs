namespace Peace.Lifelog.RegistrationTest;

using Peace.Lifelog.DataAccess;
using Peace.Lifelog.UserManagement;
using Peace.Lifelog.RegistrationWebService;

public class RegistrationWebServiceShould
{

    private const string EMAIL = "zarifshams@gmail.com";
    private string DOB = DateTime.Today.ToString("yyyy-MM-dd");
    private const string ZIPCODE = "90716";
    private const string USERROLE = "Normal";

#region Invalid Input Tests
[Fact]
public void RegistrationWebServiceShould_HaveValidEmailFormat()
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
public void RegistrationWebServiceShould_EmailBeAtLeast3Chars()
{
    // Arrange
    string invalidEmail = "z@gmail.com";
    var testRegUser = new RegistrationWebService();

    // Act
    var validEmailFormatResponse = testRegUser.RegisterUser(invalidEmail, DOB, ZIPCODE, USERROLE);

    // Assert
    Assert.True(validEmailFormatResponse == false);
}

[Fact]
public void RegistrationWebServiceShould_EmailOnlyBeAlphanumeric()
{
    // Arrange
    string invalidEmail = "zarif-123@gmail.com";
    var testRegUser = new RegistrationWebService();

    // Act
    var validEmailFormatResponse = testRegUser.RegisterUser(invalidEmail, DOB, ZIPCODE, USERROLE);

    // Assert
    Assert.True(validEmailFormatResponse == false);
}

[Fact]
public void RegistrationWebServiceShould_HaveValidDOB()
{
    // Arrange
    string invalidDOB = "1969-01-01";
    var testRegUser = new RegistrationWebService();

    // Act
    var validEmailFormatResponse = testRegUser.RegisterUser(EMAIL, invalidDOB, ZIPCODE, USERROLE);

    // Assert
    Assert.True(validEmailFormatResponse == false);
}

[Fact]
public void RegistrationWebServiceShould_HaveValidZipCode()
{
    // Arrange
    string invalidZipCode = "90623";
    var testRegUser = new RegistrationWebService();

    // Act
    var validEmailFormatResponse = testRegUser.RegisterUser(EMAIL, DOB, ZIPCODE, USERROLE);

    // Assert
    Assert.True(validEmailFormatResponse == false);
}

[Fact]
public void RegistrationWebServiceShould_InputsBeNotNull()
{
    // Arrange
    string emptyEmail= "";
    string emptyDOB = "";
    string emptyZipCode = "";
    var testRegUser = new RegistrationWebService();

    // Act
    var validEmailFormatResponse = testRegUser.RegisterUser(emptyEmail, emptyDOB, emptyZipCode, USERROLE);

    // Assert
    Assert.True(validEmailFormatResponse == false);
}


#endregion

}

