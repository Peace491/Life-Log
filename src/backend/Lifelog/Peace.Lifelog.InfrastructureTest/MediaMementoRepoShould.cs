namespace Peace.Lifelog.InfrastructureTest;
using DomainModels;
using Peace.Lifelog.DataAccess;
using Peace.Lifelog.Infrastructure;

public class MediaMementoRepoShould
{
    [Fact]
    public async Task UploadMediaMemento()
    {
        // Arrange
        var lliId = 1;
        var binary = new byte[] { 0x48, 0x69 }; // 'Hi' in ASCII
        IUpdateDataOnlyDAO updateDataOnlyDAO = new UpdateDataOnlyDAO();
        IReadDataOnlyDAO readDataOnlyDAO = new ReadDataOnlyDAO();
        var mediaMementoRepo = new MediaMementoRepo(updateDataOnlyDAO, readDataOnlyDAO);

        // Act
        var response = await mediaMementoRepo.UploadMediaMemento(lliId, binary);

        // Assert
        Assert.True(response.HasError == false);
    }

    [Fact]
    public async Task DeleteMediaMemento()
    {
        // Arrange
        var lliId = 1;
        IUpdateDataOnlyDAO updateDataOnlyDAO = new UpdateDataOnlyDAO();
        IReadDataOnlyDAO readDataOnlyDAO = new ReadDataOnlyDAO();
        var mediaMementoRepo = new MediaMementoRepo(updateDataOnlyDAO, readDataOnlyDAO);

        // Act
        var response = await mediaMementoRepo.DeleteMediaMemento(lliId);

        // Assert
        Assert.True(response.HasError == false);
    }
    [Fact]
    public async Task UploadMediaMementosFromCSV()
    {
        // Arrange
        IUpdateDataOnlyDAO updateDataOnlyDAO = new UpdateDataOnlyDAO();
        IReadDataOnlyDAO readDataOnlyDAO = new ReadDataOnlyDAO();
        var mediaMementoRepo = new MediaMementoRepo(updateDataOnlyDAO, readDataOnlyDAO);

        List<List<string>> csvContent = new List<List<string>> { new List<string> { "Guided Imagery Sessions", "1" }, new List<string> { "Yoga Session", "SGVsbG8h" } };
        // Act
        var response = await mediaMementoRepo.UploadMediaMementosFromCSV(csvContent);

        // Assert
        Assert.True(response.HasError == false);
    }
}
