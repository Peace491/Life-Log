﻿namespace Peace.Lifelog.SecurityTest;

using DomainModels;
using Peace.Lifelog.DataAccess;
using Peace.Lifelog.Security;
using Peace.Lifelog.UserManagement;
using Peace.Lifelog.UserManagementTest;
using System.Diagnostics;

public class LifelogAuthServiceShould : IAsyncLifetime, IDisposable
{
    private const string USER_ID = "TestAuthServiceAccount";
    private string USER_HASH = "";
    private const string ROLE = "Normal";

    private string DOB = DateTime.Today.ToString("yyyy-MM-dd");
    private string DEADLINE = DateTime.Today.ToString("yyyy-MM-dd");
    private const string ZIP_CODE = "90704";

    public async Task InitializeAsync()
    {   
        var lifelogUserManagementService = new LifelogUserManagementService();

        var testLifelogAccountRequest = new LifelogAccountRequest();
        testLifelogAccountRequest.UserId = ("UserId", USER_ID);
        testLifelogAccountRequest.Role = ("Role", ROLE);

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
    public async void LifelogAuthServiceAuthNShould_ReturnTheRole_ForTheUser()
    {
        //Arrange
        var timer = new Stopwatch();
        var lifelogAuthService = new LifelogAuthService();

        // Generate OTP
        OTPService oTPService = new OTPService();
        var otpResponse = await oTPService.generateOneTimePassword(USER_HASH);

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
        var response = await lifelogAuthService.AuthenticateLifelogUser(USER_HASH, OTP)!;
        timer.Stop();

        //Assert
        Assert.True(response.Claims != null);
        Assert.True(response.Claims["Role"] == "Normal");
        Assert.True(timer.Elapsed.TotalSeconds <= TestVariables.MAX_EXECUTION_TIME_IN_SECONDS);
    }


}