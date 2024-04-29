namespace Peace.Lifelog.MediaMementoTest;

using System; // Add missing import statement 
using Peace.Lifelog.Infrastructure;
using Peace.Lifelog.MediaMementoService;
using Peace.Lifelog.DataAccess;
using Peace.Lifelog.Logging;
using DomainModels;

public class MediaMementoServiceShould
{
    private int lliId = 100;
    private string userHash = "System";
    private byte[] bytes = new byte[] { 0x01, 0x02, 0x03, 0x04 };

    private string csvContent = "Title,BinaryData\nJoin a Gym,iVBORw0KGgoAAAANSUhEUgAAADIAAAAyCAIAAACRXR/mAAAATUlEQVR4nO3OMQHAIBAAsQf/nlsDLJlguCjImvnmPft24KyWqCVqiVqilqglaolaopaoJWqJWqKWqCVqiVqilqglaolaopaoJWqJWuIHP6wBY/cJXlsAAAAASUVORK5CYII=\nWeekly Hiking,iVBORw0KGgoAAAANSUhEUgAAADIAAAAyCAIAAACRXR/mAAAATUlEQVR4nO3OMQHAIBAAsQf/nlsDLJlguCjImvnmPft24KyWqCVqiVqilqglaolaopaoJWqJWqKWqCVqiVqilqglaolaopaoJWqJWuIHP6wBY/cJXlsAAAAASUVORK5CYII=\nMarathon for Beginners,iVBORw0KGgoAAAANSUhEUgAAADIAAAAyCAIAAACRXR/mAAAATUlEQVR4nO3OMQHAIBAAsQf/nlsDLJlguCjImvnmPft24KyWqCVqiVqilqglaolaopaoJWqJWqKWqCVqiVqilqglaolaopaoJWqJWuIHP6wBY/cJXlsAAAAASUVORK5CYII=\n";

    [Fact]
    public async Task UploadMediaMementoShould_UploadMediaToDB()
    {
        // Arrange
        IUpdateDataOnlyDAO updateDataOnlyDAO = new UpdateDataOnlyDAO();
        IReadDataOnlyDAO readDataOnlyDAO = new ReadDataOnlyDAO();
        var mediaMementoRepo = new MediaMementoRepo(updateDataOnlyDAO, readDataOnlyDAO);

        CreateDataOnlyDAO createDataOnlyDAO = new CreateDataOnlyDAO();
        LogTarget logTarget = new LogTarget(createOnlyDAO: createDataOnlyDAO, readDataOnlyDAO: readDataOnlyDAO);
        Logging logger = new Logging(logTarget: logTarget);


        var mediaMementoService = new MediaMementoService(mediaMementoRepo, logger);

        // Act
        var result = mediaMementoService.UploadMediaMemento(userHash, lliId, bytes);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Result.HasError == false);
        Assert.True(result.Result.ErrorMessage == null);
    }
    [Fact]
    public async Task DeleteMediaMementoShould_DeleteMediaFromDB()
    {
        // Arrange
        IUpdateDataOnlyDAO updateDataOnlyDAO = new UpdateDataOnlyDAO();
        IReadDataOnlyDAO readDataOnlyDAO = new ReadDataOnlyDAO();
        var mediaMementoRepo = new MediaMementoRepo(updateDataOnlyDAO, readDataOnlyDAO);

        CreateDataOnlyDAO createDataOnlyDAO = new CreateDataOnlyDAO();
        LogTarget logTarget = new LogTarget(createOnlyDAO: createDataOnlyDAO, readDataOnlyDAO: readDataOnlyDAO);
        Logging logger = new Logging(logTarget: logTarget);


        var mediaMementoService = new MediaMementoService(mediaMementoRepo, logger);

        // Act
        var result = mediaMementoService.DeleteMediaMemento(lliId);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Result.HasError == false);
        Assert.True(result.Result.ErrorMessage == null);
    }
    [Fact]
    public async Task GetAllUserLLIShould_GetAllUserLLI()
    {
        // Arrange
        IUpdateDataOnlyDAO updateDataOnlyDAO = new UpdateDataOnlyDAO();
        IReadDataOnlyDAO readDataOnlyDAO = new ReadDataOnlyDAO();
        var mediaMementoRepo = new MediaMementoRepo(updateDataOnlyDAO, readDataOnlyDAO);

        CreateDataOnlyDAO createDataOnlyDAO = new CreateDataOnlyDAO();
        LogTarget logTarget = new LogTarget(createOnlyDAO: createDataOnlyDAO, readDataOnlyDAO: readDataOnlyDAO);
        Logging logger = new Logging(logTarget: logTarget);

        var mediaMementoService = new MediaMementoService(mediaMementoRepo, logger);

        // Act
        var result = mediaMementoService.GetAllUserImages(userHash);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Result.HasError == false);
        Assert.True(result.Result.ErrorMessage == null);
    }
    [Fact]
    public async Task UploadMediaMementoShould_ReturnErrorMessage_WhenMediaUploadFails()
    {
        // Arrange
        IUpdateDataOnlyDAO updateDataOnlyDAO = new UpdateDataOnlyDAO();
        IReadDataOnlyDAO readDataOnlyDAO = new ReadDataOnlyDAO();
        var mediaMementoRepo = new MediaMementoRepo(updateDataOnlyDAO, readDataOnlyDAO);

        CreateDataOnlyDAO createDataOnlyDAO = new CreateDataOnlyDAO();
        LogTarget logTarget = new LogTarget(createOnlyDAO: createDataOnlyDAO, readDataOnlyDAO: readDataOnlyDAO);
        Logging logger = new Logging(logTarget: logTarget);

        byte[] badByte = new byte[] {};

        var mediaMementoService = new MediaMementoService(mediaMementoRepo, logger);

        // Act
        var result = mediaMementoService.UploadMediaMemento(userHash, lliId, badByte);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Result.HasError == true);
        Assert.True(result.Result.ErrorMessage == "File size is greater than 50 mb or empty.");
    }
    [Fact]
    public async Task UploadMediaMementoShould_ReturnErrorMessage_IfMediaMementoIsEmpty()
    {
         // Arrange
        IUpdateDataOnlyDAO updateDataOnlyDAO = new UpdateDataOnlyDAO();
        IReadDataOnlyDAO readDataOnlyDAO = new ReadDataOnlyDAO();
        var mediaMementoRepo = new MediaMementoRepo(updateDataOnlyDAO, readDataOnlyDAO);

        CreateDataOnlyDAO createDataOnlyDAO = new CreateDataOnlyDAO();
        LogTarget logTarget = new LogTarget(createOnlyDAO: createDataOnlyDAO, readDataOnlyDAO: readDataOnlyDAO);
        Logging logger = new Logging(logTarget: logTarget);

        byte[] badByte = new byte[] {};

        var mediaMementoService = new MediaMementoService(mediaMementoRepo, logger);

        // Act
        var result = mediaMementoService.UploadMediaMemento(userHash, lliId, badByte);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Result.HasError == true);
        Assert.True(result.Result.ErrorMessage == "File size is greater than 50 mb or empty.");

    }
    [Fact]
    public async Task UploadMediaMementoShould_ReturnErrorMessage_IfMediaMementoBiggerThan50mbs()
    {
         // Arrange
        IUpdateDataOnlyDAO updateDataOnlyDAO = new UpdateDataOnlyDAO();
        IReadDataOnlyDAO readDataOnlyDAO = new ReadDataOnlyDAO();
        var mediaMementoRepo = new MediaMementoRepo(updateDataOnlyDAO, readDataOnlyDAO);

        CreateDataOnlyDAO createDataOnlyDAO = new CreateDataOnlyDAO();
        LogTarget logTarget = new LogTarget(createOnlyDAO: createDataOnlyDAO, readDataOnlyDAO: readDataOnlyDAO);
        Logging logger = new Logging(logTarget: logTarget);

        int sizeInBytes = 50 * 1048576 + 100; // 50 MB plus an additional 100 bytes
        byte[] largeByteArray = new byte[sizeInBytes];


        var mediaMementoService = new MediaMementoService(mediaMementoRepo, logger);

        // Act
        var result = mediaMementoService.UploadMediaMemento(userHash, lliId, largeByteArray);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Result.HasError == true);
        Assert.True(result.Result.ErrorMessage == "File size is greater than 50 mb or empty.");

    }
    [Fact]
    public async Task UploadMediaMementoShould_ReturnErrorMessage_IfMediaMementoIsNotImage()
    {
        Assert.False(true);
    }
    [Fact]
    public async Task UploadMediaMementoShould_ReturnErrorMessage_IfUserHasTooMuchMediaUploaded()
    {
        // need to upload over 1GB of media to test this
        Assert.False(true);

    }
    [Fact]
    public async Task DeleteMediaMementoShould_ReturnErrorMessage_WhenMediaDeleteFails()
    {
        // Arrange
        IUpdateDataOnlyDAO updateDataOnlyDAO = new UpdateDataOnlyDAO();
        IReadDataOnlyDAO readDataOnlyDAO = new ReadDataOnlyDAO();
        var mediaMementoRepo = new MediaMementoRepo(updateDataOnlyDAO, readDataOnlyDAO);

        CreateDataOnlyDAO createDataOnlyDAO = new CreateDataOnlyDAO();
        LogTarget logTarget = new LogTarget(createOnlyDAO: createDataOnlyDAO, readDataOnlyDAO: readDataOnlyDAO);
        Logging logger = new Logging(logTarget: logTarget);


        var mediaMementoService = new MediaMementoService(mediaMementoRepo, logger);

        // Act
        var result = mediaMementoService.DeleteMediaMemento(-1);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Result.HasError == true);
        Assert.True(result.Result.ErrorMessage == "No media memento found to delete from.");

    }
    [Fact]
    public async Task DeleteMediaMementoShould_ReturnErrorMessage_IfLLIIDDoesntExistOnDb()
    {
        // Arrange
        IUpdateDataOnlyDAO updateDataOnlyDAO = new UpdateDataOnlyDAO();
        IReadDataOnlyDAO readDataOnlyDAO = new ReadDataOnlyDAO();
        var mediaMementoRepo = new MediaMementoRepo(updateDataOnlyDAO, readDataOnlyDAO);

        CreateDataOnlyDAO createDataOnlyDAO = new CreateDataOnlyDAO();
        LogTarget logTarget = new LogTarget(createOnlyDAO: createDataOnlyDAO, readDataOnlyDAO: readDataOnlyDAO);
        Logging logger = new Logging(logTarget: logTarget);


        var mediaMementoService = new MediaMementoService(mediaMementoRepo, logger);

        // Act
        var result = mediaMementoService.DeleteMediaMemento(-1);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Result.HasError == true);
        Assert.True(result.Result.ErrorMessage == "No media memento found to delete from.");

    }
    [Fact]
    public async Task UploadMediaMementosShould_UploadMultipleMediaToDB()
    {
        // Arrange
        IUpdateDataOnlyDAO updateDataOnlyDAO = new UpdateDataOnlyDAO();
        IReadDataOnlyDAO readDataOnlyDAO = new ReadDataOnlyDAO();
        var mediaMementoRepo = new MediaMementoRepo(updateDataOnlyDAO, readDataOnlyDAO);

        CreateDataOnlyDAO createDataOnlyDAO = new CreateDataOnlyDAO();
        LogTarget logTarget = new LogTarget(createOnlyDAO: createDataOnlyDAO, readDataOnlyDAO: readDataOnlyDAO);
        Logging logger = new Logging(logTarget: logTarget);


        var mediaMementoService = new MediaMementoService(mediaMementoRepo, logger);

        List<List<string>> csvContent = new List<List<string>> { new List<string> { "Guided Imagery Sessions", "1" }, new List<string> { "Yoga Session", "2" } };

        // Act
        var result = mediaMementoService.UploadMediaMementosFromCSV(userHash, csvContent);

        // Arrange
        Assert.NotNull(result);
    }
}