namespace Peace.Lifelog.SecurityTest;

using Peace.Lifelog.DataAccess;
using Peace.Lifelog.Security;
using System.Diagnostics;

public class OTPServiceShould
{
    // LifelogConfig lifelogConfig = LifelogConfig.LoadConfiguration();
    
    [Fact]
    public async void GenerateOneTimePasswordShould_ProduceOTPLengthEight()
    {
        // Arrange
        IUpdateDataOnlyDAO updateDataOnlyDAO = new UpdateDataOnlyDAO();
        OTPService oTPService = new OTPService(updateDataOnlyDAO);
        Stopwatch timer = new Stopwatch();

        // Act
        timer.Start();
        var otpResponse = await oTPService.generateOneTimePassword("System");
        timer.Stop();

        // Assert
        if (otpResponse.Output != null)
        {
            foreach (string otpOutput in otpResponse.Output.Cast<string>())
            {
                Assert.True(otpOutput.Length == 8);
            }
        }
        Assert.True(timer.ElapsedMilliseconds < 3001);
    }
}
