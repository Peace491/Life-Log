namespace Peace.Lifelog.SecurityTest;

using DomainModels;
using Peace.Lifelog.DataAccess;
using Peace.Lifelog.Security;
using Peace.Lifelog.UserManagement;
using Peace.Lifelog.UserManagementTest;
using System.Diagnostics;

public class LifelogAuthServiceShould
{
    [Fact]

    public async void LifelogAuthServiceAuthNShould_ReturnTheRole_ForTheUser()
    {
        //Arrange
        var timer = new Stopwatch();

        var lifelogAuthService = new LifelogAuthService();

        // Create Lifelog User
        var LifelogUserManagementService = new LifelogUserManagementService();

        var readDataOnlyDAO = new ReadDataOnlyDAO();

        var mockUserId = "TestAuthUser";
        var mockRole = "Normal";

        var mockDob = DateTime.Now.ToString("yyyy-MM-dd");
        var mockZipCode = "90704";

        var testLifelogAccountRequest = new LifelogAccountRequest();
        testLifelogAccountRequest.UserId = ("UserId", mockUserId);
        testLifelogAccountRequest.Role = ("Role", mockRole);

        var testLifelogProfileRequest = new LifelogProfileRequest();
        testLifelogProfileRequest.DOB = ("DOB", mockDob);
        testLifelogProfileRequest.ZipCode = ("ZipCode", mockZipCode);

        var createAccountResponse = await LifelogUserManagementService.CreateLifelogUser(testLifelogAccountRequest, testLifelogProfileRequest);

        string userHash = "";

        if (createAccountResponse.Output is not null)
        {
            foreach (string output in createAccountResponse.Output)
            {
                userHash = output;
            }
        }

        // Generate OTP
        OTPService oTPService = new OTPService();
        var otpResponse = await oTPService.generateOneTimePassword(userHash);

        string OTP = "";
        if (otpResponse.Output != null)
        {
            foreach (string otpOutput in otpResponse.Output.Cast<string>())
            {
                OTP = otpOutput;
            }
        }

        //Act
        timer.Start();
        var response = await lifelogAuthService.AuthenticateLifelogUser(userHash, OTP)!;
        timer.Stop();

        //Assert
        Assert.True(response.Claims != null);
        Assert.True(response.Claims["Role"] == "Normal");
        Assert.True(timer.Elapsed.TotalSeconds <= TestVariables.MAX_EXECUTION_TIME_IN_SECONDS);

        //Cleanup
        var deleteAccountResponse = await LifelogUserManagementService.DeleteLifelogUser(testLifelogAccountRequest, testLifelogProfileRequest);

    }


}
