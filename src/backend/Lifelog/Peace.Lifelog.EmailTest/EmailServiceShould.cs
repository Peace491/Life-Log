namespace Peace.Lifelog.EmailTest;
using Peace.Lifelog.DataAccess;
using Peace.Lifelog.Email;
using Peace.Lifelog.Security;


public class EmailServiceShould
{
    [Fact]
    public async void SendOTPEmailShould_SendEmail()
    {
        // Arrange
        ICreateDataOnlyDAO createDataOnlyDAO = new CreateDataOnlyDAO();
        IReadDataOnlyDAO readDataOnlyDAO = new ReadDataOnlyDAO();
        IUpdateDataOnlyDAO updateDataOnlyDAO = new UpdateDataOnlyDAO();
        IDeleteDataOnlyDAO deleteDataOnlyDAO = new DeleteDataOnlyDAO();
        ISaltService saltService = new SaltService();
        IHashService hashService = new HashService();
        IOTPService oTPService = new OTPService(updateDataOnlyDAO);
        IEmailService emailService = new EmailService(readDataOnlyDAO, oTPService, updateDataOnlyDAO);

        // Act
        var emailResponse = await emailService.SendOTPEmail("0Yg6cgh/M4+ImmL0GozWqhgcDCqTZEhzm9angvVAC30=");

        // Assert
        Assert.True(emailResponse.HasError == false);
    }
}