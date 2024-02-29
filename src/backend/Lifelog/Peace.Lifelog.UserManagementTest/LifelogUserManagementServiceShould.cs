namespace Peace.Lifelog.UserManagementTest;

using Peace.Lifelog.DataAccess;
using Peace.Lifelog.UserManagement;
using System.Diagnostics;

public class LifelogUserManagementServiceShould
{

    #region Create Account Tests
    [Fact]
    public async void LifelogUserManagementServiceCreateAccountShould_CreateAnAccountInTheDatabase()
    {
        //Arrange
        var timer = new Stopwatch();

        var LifelogUserManagementService = new LifelogUserManagementService();

        var readDataOnlyDAO = new ReadDataOnlyDAO();
        var deleteDataOnlyDAO = new DeleteDataOnlyDAO();

        var mockUserId = "TestUserCreation";
        var mockRole = "Normal";
        var mockMfaId = "2";

        // Creating User Profile based off User Account
        var mockDob = DateTime.Now.ToString("yyyy-MM-dd");
        var mockZipCode = "90704";


        var testLifelogAccountRequest = new LifelogAccountRequest();
        testLifelogAccountRequest.UserId = ("UserId", mockUserId);
        testLifelogAccountRequest.Role = ("Role", mockRole);

        var testLifelogProfileRequest = new LifelogProfileRequest();
        testLifelogProfileRequest.DOB = ("DOB", mockDob);
        testLifelogProfileRequest.ZipCode = ("ZipCode", mockZipCode);

        var readAccountSql = $"SELECT * FROM LifelogAccount WHERE UserId = \"{mockUserId}\"";

        // Act
        timer.Start();
        var createAccountResponse = await LifelogUserManagementService.CreateLifelogUser(testLifelogAccountRequest, testLifelogProfileRequest);
        timer.Stop();

        var readResponse = await readDataOnlyDAO.ReadData(readAccountSql);


        // Assert
        Assert.True(createAccountResponse.HasError == false);
        Assert.True(timer.Elapsed.TotalSeconds <= TestVariables.MAX_EXECUTION_TIME_IN_SECONDS);
        Assert.True(readResponse.Output.Count == 1);

        //Cleanup
        var deleteAccountResponse = await LifelogUserManagementService.DeleteLifelogUser(testLifelogAccountRequest, testLifelogProfileRequest);

    }
    /*
        [Fact]
        public async void LifelogUserManagementServiceCreateAccountShould_ThrowArgumentNullErrorIfUserIdIsNull()
        {
            //Arrange
            var LifelogUserManagementService = new LifelogUserManagementService();

            var mockMfaId = "2";
            var mockPassword = "password";

            var testAccountRequest = new TestAccountRequest();

            testAccountRequest.UserId = (TestVariables.USER_ID_TYPE, string.Empty);

            var errorIsThrown = false;

            // Act
            try
            {
                var createAccountResponse = await LifelogUserManagementService.CreateAccount(testAccountRequest);
            }
            catch (ArgumentNullException)
            {
                errorIsThrown = true;
            }


            // Assert
            Assert.True(errorIsThrown);
        }

        [Fact]
        public async void LifelogUserManagementServiceCreateAccountShould_ThrowArgumentNullErrorIfAccountDetailsIsNull()
        {
            //Arrange
            var LifelogUserManagementService = new LifelogUserManagementService();

            var mockUserId = "1";

            var testAccountRequest = new TestAccountRequest();

            testAccountRequest.UserId = (TestVariables.USER_ID_TYPE, mockUserId);

            var errorIsThrown = false;

            // Act
            try
            {
                var createAccountResponse = await LifelogUserManagementService.CreateAccount(testAccountRequest);
            }
            catch (ArgumentNullException)
            {
                errorIsThrown = true;
            }


            // Assert
            Assert.True(errorIsThrown);
        }

        [Fact]
        public async void LifelogUserManagementServiceCreateAccountShould_ReturnAnErrorResponseIfTheAccountAlreadyExist()
        {
            //Arrange
            var LifelogUserManagementService = new LifelogUserManagementService();


            var mockUserId = "1";
            var mockMfaId = "2";
            var mockPassword = "password";
            var mockUserHash = ")@#$*%!)#%*!dgjwodwnjvon";

            var testAccountRequest = new TestAccountRequest();

            testAccountRequest.UserId = (TestVariables.USER_ID_TYPE, mockUserId);
            testAccountRequest.MfaId = (TestVariables.MFA_ID_TYPE, mockMfaId);
            testAccountRequest.UserHash = (TestVariables.USER_HASH_TYPE, mockUserHash);
            testAccountRequest.CreationDate = (TestVariables.CREATION_DATE_TYPE, DateTime.Now.ToString("yyyy-MM-dd"));
            testAccountRequest.Password = (TestVariables.PASSWORD_TYPE, mockPassword);

            await LifelogUserManagementService.CreateAccount(testAccountRequest);

            // Act
            var createAccountResponse = await LifelogUserManagementService.CreateAccount(testAccountRequest);

            // Assert
            Assert.True(createAccountResponse.HasError == true);
        }

        [Fact]
        public async void LifelogUserManagementServiceCreateAccountShould_ReturnAnErrorResponseIfTheRequestResultInInvalidSQL()
        {
            //Arrange
            var LifelogUserManagementService = new LifelogUserManagementService();

            var wrongUserIdType = "Phone";
            var mockUserId = "1";
            var mockMfaId = "2";
            var mockPassword = "password";
            var mockUserHash = ")@#$*%!)#%*!dgjwodwnjvon";

            var testAccountRequest = new TestAccountRequest();

            testAccountRequest.UserId = (wrongUserIdType, mockUserId);
            testAccountRequest.MfaId = (TestVariables.MFA_ID_TYPE, mockMfaId);
            testAccountRequest.UserHash = (TestVariables.USER_HASH_TYPE, mockUserHash);
            testAccountRequest.CreationDate = (TestVariables.CREATION_DATE_TYPE, DateTime.Now.ToString("yyyy-MM-dd"));
            testAccountRequest.Password = (TestVariables.PASSWORD_TYPE, mockPassword);

            await LifelogUserManagementService.CreateAccount(testAccountRequest);

            // Act
            var createAccountResponse = await LifelogUserManagementService.CreateAccount(testAccountRequest);

            // Assert
            Assert.True(createAccountResponse.HasError == true);
        }*/
    #endregion

    #region Delete Lifelog User Tests
    [Fact]
    public async void LifelogUserManagementServiceDeleteAccountShould_DeleteAnAccountInTheDatabase()
    {
        //Arrange
        var timer = new Stopwatch();

        var LifelogUserManagementService = new LifelogUserManagementService();

        var readDataOnlyDAO = new ReadDataOnlyDAO();

        var mockUserId = "TestUserCreation";
        var mockRole = "Normal";
        var mockMfaId = "2";

        // Creating User Profile based off User Account
        var mockDob = DateTime.Now.ToString("yyyy-MM-dd");
        var mockZipCode = "92612";


        var testLifelogAccountRequest = new LifelogAccountRequest();
        testLifelogAccountRequest.UserId = ("UserId", mockUserId);
        testLifelogAccountRequest.Role = ("Role", mockRole);

        var testLifelogProfileRequest = new LifelogProfileRequest();
        testLifelogProfileRequest.DOB = ("DOB", mockDob);
        testLifelogProfileRequest.ZipCode = ("ZipCode", mockZipCode);

        var readAccountSql = $"SELECT * FROM LifelogAccount WHERE UserId = \"{mockUserId}\"";
        var createAccountResponse = await LifelogUserManagementService.CreateLifelogUser(testLifelogAccountRequest, testLifelogProfileRequest);

        // Act
        timer.Start();
        var deleteAccountResponse = await LifelogUserManagementService.DeleteLifelogUser(testLifelogAccountRequest, testLifelogProfileRequest);
        timer.Stop();

        var readResponse = await readDataOnlyDAO.ReadData(readAccountSql);


        // Assert
        Assert.True(deleteAccountResponse.HasError == false);
        Assert.True(timer.Elapsed.TotalSeconds <= TestVariables.MAX_EXECUTION_TIME_IN_SECONDS);
        Assert.True(readResponse.Output is null);

    }

    #endregion
}