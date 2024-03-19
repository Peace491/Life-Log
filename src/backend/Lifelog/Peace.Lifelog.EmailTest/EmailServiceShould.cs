namespace Peace.Lifelog.EmailTest;

using Peace.Lifelog.Email;


public class EmailServiceShould
{
    [Fact]
    public async void SendOTPEmailShould_SendEmail()
    {
        // Arrange

        var emailService = new EmailService();

        // Act
        var emailResponse = await emailService.SendOTPEmail("0Yg6cgh/M4+ImmL0GozWqhgcDCqTZEhzm9angvVAC30=");

        // Assert
        Assert.True(emailResponse.HasError == false);
    }
}