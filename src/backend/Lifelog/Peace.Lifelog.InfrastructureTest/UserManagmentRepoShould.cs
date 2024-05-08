namespace Peace.Lifelog.InfrastructureTest;
using DomainModels;
using Peace.Lifelog.DataAccess;
using Peace.Lifelog.Infrastructure;
using Peace.Lifelog.Logging;
using Xunit;
using ZstdSharp.Unsafe;

public class UserManagmentRepoShould
{
    [Fact]
    public async Task DeletePersonalIdentifiableInformation()
    {
        // Arrange
        var userHash = "userHash";
        ICreateDataOnlyDAO createDataOnlyDAO = new CreateDataOnlyDAO();
        IReadDataOnlyDAO readDataOnlyDAO = new ReadDataOnlyDAO();
        IDeleteDataOnlyDAO deleteDataOnlyDAO = new DeleteDataOnlyDAO();
        ILogTarget logTarget = new LogTarget(createDataOnlyDAO, readDataOnlyDAO);
        ILogging logger = new Logging(logTarget);
        var userManagmentRepo = new UserManagmentRepo(createDataOnlyDAO, readDataOnlyDAO, deleteDataOnlyDAO, logger);

        _ = await logger.CreateLog("logs", userHash, "Info", "Error", "message");

        // Act
        var response = await userManagmentRepo.DeletePersonalIdentifiableInformation(userHash);

        // Assert
        Assert.True(response.HasError == false);
    }

    [Fact]
    public async Task ViewPersonalIdentifiableInformation()
    {
        // Arrange
        // TODO: Replace with a test user
        var userHash = "System";
        ICreateDataOnlyDAO createDataOnlyDAO = new CreateDataOnlyDAO();
        IReadDataOnlyDAO readDataOnlyDAO = new ReadDataOnlyDAO();
        IDeleteDataOnlyDAO deleteDataOnlyDAO = new DeleteDataOnlyDAO();
        ILogTarget logTarget = new LogTarget(createDataOnlyDAO, readDataOnlyDAO);
        ILogging logger = new Logging(logTarget);
        var userManagmentRepo = new UserManagmentRepo(createDataOnlyDAO,readDataOnlyDAO, deleteDataOnlyDAO, logger);

        _ = await logger.CreateLog("Logs", userHash, "Info", "View", "view PII test");
        _ = await logger.CreateLog("Logs", userHash, "Info", "View", "view PII test");

        // Act
        var response = await userManagmentRepo.ViewPersonalIdentifiableInformation(userHash);

        // Assert
        Assert.True(response.HasError == false);
    }
}