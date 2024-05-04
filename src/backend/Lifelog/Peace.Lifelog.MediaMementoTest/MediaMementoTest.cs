namespace Peace.Lifelog.MediaMementoTest;

using System; // Add missing import statement 
using Peace.Lifelog.Infrastructure;
using Peace.Lifelog.MediaMementoService;
using Peace.Lifelog.DataAccess;
using Peace.Lifelog.Logging;
using Peace.Lifelog.Security;
using DomainModels;

public class MediaMementoServiceShould
{
    private int lliId = 1;
    private string USER_HASH = "System";
    private const string ROLE = "Normal";
    private byte[] bytes = new byte[] { 0x01, 0x02, 0x03, 0x04 };

    [Fact]
    public async Task UploadMediaMementoShould_UploadMediaToDB()
    {
        // Arrange
        var principal = new AppPrincipal();
        principal.UserId = USER_HASH;
        principal.Claims = new Dictionary<string, string>() {{"Role", ROLE}};
        IUpdateDataOnlyDAO updateDataOnlyDAO = new UpdateDataOnlyDAO();
        IReadDataOnlyDAO readDataOnlyDAO = new ReadDataOnlyDAO();
        var mediaMementoRepo = new MediaMementoRepo(updateDataOnlyDAO, readDataOnlyDAO);
        ILifelogAuthService lifelogAuthService = new LifelogAuthService();


        CreateDataOnlyDAO createDataOnlyDAO = new CreateDataOnlyDAO();
        LogTarget logTarget = new LogTarget(createOnlyDAO: createDataOnlyDAO, readDataOnlyDAO: readDataOnlyDAO);
        Logging logger = new Logging(logTarget: logTarget);


        var mediaMementoService = new MediaMementoService(mediaMementoRepo, logger, lifelogAuthService);

        // Act
        var result = await mediaMementoService.UploadMediaMemento(USER_HASH, lliId, bytes, principal);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.HasError == false);
        Assert.True(result.ErrorMessage == null);
    }
    [Fact]
    public async Task DeleteMediaMementoShould_DeleteMediaFromDB()
    {
        // Arrange
        var principal = new AppPrincipal();
        principal.UserId = USER_HASH;
        principal.Claims = new Dictionary<string, string>() {{"Role", ROLE}};
        IUpdateDataOnlyDAO updateDataOnlyDAO = new UpdateDataOnlyDAO();
        IReadDataOnlyDAO readDataOnlyDAO = new ReadDataOnlyDAO();
        var mediaMementoRepo = new MediaMementoRepo(updateDataOnlyDAO, readDataOnlyDAO);
        ILifelogAuthService lifelogAuthService = new LifelogAuthService();


        CreateDataOnlyDAO createDataOnlyDAO = new CreateDataOnlyDAO();
        LogTarget logTarget = new LogTarget(createOnlyDAO: createDataOnlyDAO, readDataOnlyDAO: readDataOnlyDAO);
        Logging logger = new Logging(logTarget: logTarget);


        var mediaMementoService = new MediaMementoService(mediaMementoRepo, logger, lifelogAuthService);

        // Act
        var result = await mediaMementoService.DeleteMediaMemento(lliId, principal);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.HasError == false);
        Assert.True(result.ErrorMessage == null);
    }
    [Fact]
    public async Task GetAllUserLLIShould_GetAllUserLLI()
    {
        // Arrange
        var principal = new AppPrincipal();
        principal.UserId = USER_HASH;
        principal.Claims = new Dictionary<string, string>() {{"Role", ROLE}};
        IUpdateDataOnlyDAO updateDataOnlyDAO = new UpdateDataOnlyDAO();
        IReadDataOnlyDAO readDataOnlyDAO = new ReadDataOnlyDAO();
        var mediaMementoRepo = new MediaMementoRepo(updateDataOnlyDAO, readDataOnlyDAO);
        ILifelogAuthService lifelogAuthService = new LifelogAuthService();


        CreateDataOnlyDAO createDataOnlyDAO = new CreateDataOnlyDAO();
        LogTarget logTarget = new LogTarget(createOnlyDAO: createDataOnlyDAO, readDataOnlyDAO: readDataOnlyDAO);
        Logging logger = new Logging(logTarget: logTarget);


        var mediaMementoService = new MediaMementoService(mediaMementoRepo, logger, lifelogAuthService);

        // Act
        var result = await mediaMementoService.GetAllUserImages(USER_HASH, principal);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.HasError == false);
        Assert.True(result.ErrorMessage == null);
    }
    [Fact]
    public async Task UploadMediaMementoShould_ReturnErrorMessage_WhenMediaUploadFails()
    {
        // Arrange
        var principal = new AppPrincipal();
        principal.UserId = USER_HASH;
        principal.Claims = new Dictionary<string, string>() {{"Role", ROLE}};
        IUpdateDataOnlyDAO updateDataOnlyDAO = new UpdateDataOnlyDAO();
        IReadDataOnlyDAO readDataOnlyDAO = new ReadDataOnlyDAO();
        var mediaMementoRepo = new MediaMementoRepo(updateDataOnlyDAO, readDataOnlyDAO);
        ILifelogAuthService lifelogAuthService = new LifelogAuthService();


        CreateDataOnlyDAO createDataOnlyDAO = new CreateDataOnlyDAO();
        LogTarget logTarget = new LogTarget(createOnlyDAO: createDataOnlyDAO, readDataOnlyDAO: readDataOnlyDAO);
        Logging logger = new Logging(logTarget: logTarget);

        byte[] badByte = new byte[] {};


        var mediaMementoService = new MediaMementoService(mediaMementoRepo, logger, lifelogAuthService);

        // Act
        var result = await mediaMementoService.UploadMediaMemento(USER_HASH, lliId, badByte, principal);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.HasError == true);
        Assert.True(result.ErrorMessage == "File size is greater than 50 mb or empty.");
    }
    [Fact]
    public async Task UploadMediaMementoShould_ReturnErrorMessage_IfMediaMementoIsEmpty()
    {
        // Arrange
        var principal = new AppPrincipal();
        principal.UserId = USER_HASH;
        principal.Claims = new Dictionary<string, string>() {{"Role", ROLE}};
        IUpdateDataOnlyDAO updateDataOnlyDAO = new UpdateDataOnlyDAO();
        IReadDataOnlyDAO readDataOnlyDAO = new ReadDataOnlyDAO();
        var mediaMementoRepo = new MediaMementoRepo(updateDataOnlyDAO, readDataOnlyDAO);
        ILifelogAuthService lifelogAuthService = new LifelogAuthService();


        CreateDataOnlyDAO createDataOnlyDAO = new CreateDataOnlyDAO();
        LogTarget logTarget = new LogTarget(createOnlyDAO: createDataOnlyDAO, readDataOnlyDAO: readDataOnlyDAO);
        Logging logger = new Logging(logTarget: logTarget);


        var mediaMementoService = new MediaMementoService(mediaMementoRepo, logger, lifelogAuthService);

        byte[] badByte = new byte[] {};


        // Act
        var result = await mediaMementoService.UploadMediaMemento(USER_HASH, lliId, badByte, principal);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.HasError == true);
        Assert.True(result.ErrorMessage == "File size is greater than 50 mb or empty.");

    }
    [Fact]
    public async Task UploadMediaMementoShould_ReturnErrorMessage_IfMediaMementoBiggerThan50mbs()
    {
        // Arrange
        var principal = new AppPrincipal();
        principal.UserId = USER_HASH;
        principal.Claims = new Dictionary<string, string>() {{"Role", ROLE}};
        IUpdateDataOnlyDAO updateDataOnlyDAO = new UpdateDataOnlyDAO();
        IReadDataOnlyDAO readDataOnlyDAO = new ReadDataOnlyDAO();
        var mediaMementoRepo = new MediaMementoRepo(updateDataOnlyDAO, readDataOnlyDAO);
        ILifelogAuthService lifelogAuthService = new LifelogAuthService();


        CreateDataOnlyDAO createDataOnlyDAO = new CreateDataOnlyDAO();
        LogTarget logTarget = new LogTarget(createOnlyDAO: createDataOnlyDAO, readDataOnlyDAO: readDataOnlyDAO);
        Logging logger = new Logging(logTarget: logTarget);


        var mediaMementoService = new MediaMementoService(mediaMementoRepo, logger, lifelogAuthService);

        int sizeInBytes = 50 * 1048576 + 100; // 50 MB plus an additional 100 bytes
        byte[] largeByteArray = new byte[sizeInBytes];


        // Act
        var result = await mediaMementoService.UploadMediaMemento(USER_HASH, lliId, largeByteArray, principal);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.HasError == true);
        Assert.True(result.ErrorMessage == "File size is greater than 50 mb or empty.");

    }
    [Fact]
    public async Task DeleteMediaMementoShould_ReturnErrorMessage_WhenMediaDeleteFails()
    {
        // Arrange
        var principal = new AppPrincipal();
        principal.UserId = USER_HASH;
        principal.Claims = new Dictionary<string, string>() {{"Role", ROLE}};
        IUpdateDataOnlyDAO updateDataOnlyDAO = new UpdateDataOnlyDAO();
        IReadDataOnlyDAO readDataOnlyDAO = new ReadDataOnlyDAO();
        var mediaMementoRepo = new MediaMementoRepo(updateDataOnlyDAO, readDataOnlyDAO);
        ILifelogAuthService lifelogAuthService = new LifelogAuthService();


        CreateDataOnlyDAO createDataOnlyDAO = new CreateDataOnlyDAO();
        LogTarget logTarget = new LogTarget(createOnlyDAO: createDataOnlyDAO, readDataOnlyDAO: readDataOnlyDAO);
        Logging logger = new Logging(logTarget: logTarget);


        var mediaMementoService = new MediaMementoService(mediaMementoRepo, logger, lifelogAuthService);

        // Act
        var result = await mediaMementoService.DeleteMediaMemento(-1, principal);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.HasError == true);
        Assert.True(result.ErrorMessage == "No media memento found to delete from.");

    }
    [Fact]
    public async Task DeleteMediaMementoShould_ReturnErrorMessage_IfLLIIDDoesntExistOnDb()
    {
        // Arrange
        var principal = new AppPrincipal();
        principal.UserId = USER_HASH;
        principal.Claims = new Dictionary<string, string>() {{"Role", ROLE}};
        IUpdateDataOnlyDAO updateDataOnlyDAO = new UpdateDataOnlyDAO();
        IReadDataOnlyDAO readDataOnlyDAO = new ReadDataOnlyDAO();
        var mediaMementoRepo = new MediaMementoRepo(updateDataOnlyDAO, readDataOnlyDAO);
        ILifelogAuthService lifelogAuthService = new LifelogAuthService();


        CreateDataOnlyDAO createDataOnlyDAO = new CreateDataOnlyDAO();
        LogTarget logTarget = new LogTarget(createOnlyDAO: createDataOnlyDAO, readDataOnlyDAO: readDataOnlyDAO);
        Logging logger = new Logging(logTarget: logTarget);


        var mediaMementoService = new MediaMementoService(mediaMementoRepo, logger, lifelogAuthService);

        // Act
        var result = await mediaMementoService.DeleteMediaMemento(-1, principal);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.HasError == true);
        Assert.True(result.ErrorMessage == "No media memento found to delete from.");

    }
    [Fact]
    public async Task UploadMediaMementosShould_UploadMultipleMediaToDB()
    {
        // Arrange
        var principal = new AppPrincipal();
        principal.UserId = USER_HASH;
        principal.Claims = new Dictionary<string, string>() {{"Role", ROLE}};
        IUpdateDataOnlyDAO updateDataOnlyDAO = new UpdateDataOnlyDAO();
        IReadDataOnlyDAO readDataOnlyDAO = new ReadDataOnlyDAO();
        var mediaMementoRepo = new MediaMementoRepo(updateDataOnlyDAO, readDataOnlyDAO);
        ILifelogAuthService lifelogAuthService = new LifelogAuthService();


        CreateDataOnlyDAO createDataOnlyDAO = new CreateDataOnlyDAO();
        LogTarget logTarget = new LogTarget(createOnlyDAO: createDataOnlyDAO, readDataOnlyDAO: readDataOnlyDAO);
        Logging logger = new Logging(logTarget: logTarget);


        var mediaMementoService = new MediaMementoService(mediaMementoRepo, logger, lifelogAuthService);

        List<List<string>> csvContent = new List<List<string>> { new List<string> { "Guided Imagery Sessions", "SGVsbG8sIHdvcmxkIQ==" }, new List<string> { "Yoga Session", "SGVsbG8sIHdvcmxkIQ==" } };

        // Act
        var result = await mediaMementoService.UploadMediaMementosFromCSV(USER_HASH, csvContent, principal);

        // Arrange
        Assert.NotNull(result);
        Assert.True(result.HasError == false);
    }
    [Fact]
    public async Task UploadMediaMementosShould_ReturnErrorWithSQLInjection()
    {
        // Arrange
        var principal = new AppPrincipal();
        principal.UserId = USER_HASH;
        principal.Claims = new Dictionary<string, string>() {{"Role", ROLE}};
        IUpdateDataOnlyDAO updateDataOnlyDAO = new UpdateDataOnlyDAO();
        IReadDataOnlyDAO readDataOnlyDAO = new ReadDataOnlyDAO();
        var mediaMementoRepo = new MediaMementoRepo(updateDataOnlyDAO, readDataOnlyDAO);
        ILifelogAuthService lifelogAuthService = new LifelogAuthService();


        CreateDataOnlyDAO createDataOnlyDAO = new CreateDataOnlyDAO();
        LogTarget logTarget = new LogTarget(createOnlyDAO: createDataOnlyDAO, readDataOnlyDAO: readDataOnlyDAO);
        Logging logger = new Logging(logTarget: logTarget);


        var mediaMementoService = new MediaMementoService(mediaMementoRepo, logger, lifelogAuthService);

        List<List<string>> csvContent = new List<List<string>> { new List<string> { "Guided Imagery Sessions", "SGVsbG8sIHdvcmxkIQ==" }, new List<string> { "Yoga Session", "John'; DROP TABLE users; --" } };

        // Act
        var result = await mediaMementoService.UploadMediaMementosFromCSV(USER_HASH, csvContent, principal);

        // Arrange
        Assert.NotNull(result);
        Assert.True(result.HasError == true);
    }
}