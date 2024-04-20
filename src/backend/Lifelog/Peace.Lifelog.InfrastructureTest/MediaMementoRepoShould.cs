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
        var binary = "binary";
        IUpdateDataOnlyDAO updateDataOnlyDAO = new UpdateDataOnlyDAO();
        var mediaMementoRepo = new MediaMementoRepo(updateDataOnlyDAO);

        // Act
        var response = await mediaMementoRepo.UploadMediaMemento(lliId, binary);

        // Assert
        Assert.True(response.HasError == false);
    }
    // [Fact]
    // public async Task DeleteMediaMemento()
    // {
    //     // Arrange
    //     var lliId = 1;
    //     var response = new Response { Status = "Success" };
    //     var mediaMementoRepo = new Mock<IMediaMementoRepo>();
    //     mediaMementoRepo.Setup(x => x.DeleteMediaMemento(lliId)).ReturnsAsync(response);
    //     var mediaMementoService = new MediaMementoService.MediaMementoService(mediaMementoRepo.Object);

    //     // Act
    //     var result = await mediaMementoService.DeleteMediaMemento("userhash", lliId);

    //     // Assert
    //     Assert.Equal(response, result);
    // }
}
