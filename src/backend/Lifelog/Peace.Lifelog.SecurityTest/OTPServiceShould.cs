namespace Peace.Lifelog.SecurityTest;

using Org.BouncyCastle.Crypto;
using Peace.Lifelog.Security;
using System.Diagnostics;

public class OTPServiceShould
{
    LifelogConfig lifelogConfig = LifelogConfig.LoadConfiguration();
    
    [Fact]
    public async void generateOneTimePasswordShould_ProduceOTP_LengthEight()
    {
        // Arrange
        OTPService oTPService = new OTPService();
        Stopwatch timer = new Stopwatch();

        // Act
        timer.Start();
        var otpResponse = await oTPService.generateOneTimePassword("System");
        timer.Stop();

        // Assert
        if (otpResponse.Output != null)
        {
            foreach (List<Object> otpOutput in otpResponse.Output)
            {
                if (otpOutput[0] != null)
                {
                    Assert.True(otpOutput[0]?.ToString()?.Length == 8);
                }
            }
        }
        Assert.True(timer.ElapsedMilliseconds < 3001);
    }
}
