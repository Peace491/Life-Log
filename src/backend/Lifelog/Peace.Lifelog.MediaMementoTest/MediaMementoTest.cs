namespace Peace.Lifelog.MediaMementoTest;

using Peace.Lifelog.Infrastructure;
using Peace.Lifelog.MediaMementoService;
using Peace.Lifelog.DataAccess;
using Peace.Lifelog.Logging;

public class MediaMementoServiceShould
{
    private int lliId = 1;
    private string userHash = "System";
    private string hexString = "89 50 4E 47 0D 0A 1A 0A 00 00 00 0D 49 48 44 52 00 00 01 F8 00 00 03 80 08 02 00 00 00 59 A4 F6";
    [Fact]
    public void UploadMediaMementoShould_UploadMediaToDB()
    {
        // Arrange
        IUpdateDataOnlyDAO updateDataOnlyDAO = new UpdateDataOnlyDAO();
        var mediaMementoRepo = new MediaMementoRepo(updateDataOnlyDAO);

        CreateDataOnlyDAO createDataOnlyDAO = new CreateDataOnlyDAO();
        ReadDataOnlyDAO readDataOnlyDAO = new ReadDataOnlyDAO();
        LogTarget logTarget = new LogTarget(createOnlyDAO: createDataOnlyDAO);
        Logging logger = new Logging(logTarget: logTarget);


        var mediaMementoService = new MediaMementoService(mediaMementoRepo, logger);
        var mediaData = HexStringToByteArray(hexString);

        // Act
        var result = mediaMementoService.UploadMediaMemento(userHash, lliId, mediaData);

        // Assert
        Assert.NotNull(result);
    }
    [Fact]
    public void DeleteMediaMementoShould_DeleteMediaFromDB()
    {
        // Arrange
        IUpdateDataOnlyDAO updateDataOnlyDAO = new UpdateDataOnlyDAO();
        var mediaMementoRepo = new MediaMementoRepo(updateDataOnlyDAO);

        CreateDataOnlyDAO createDataOnlyDAO = new CreateDataOnlyDAO();
        ReadDataOnlyDAO readDataOnlyDAO = new ReadDataOnlyDAO();
        LogTarget logTarget = new LogTarget(createOnlyDAO: createDataOnlyDAO);
        Logging logger = new Logging(logTarget: logTarget);


        var mediaMementoService = new MediaMementoService(mediaMementoRepo, logger);

        // Act
        var result = mediaMementoService.DeleteMediaMemento(lliId);

        // Assert
        Assert.NotNull(result);
    }
    [Fact]
    public void UploadMediaMementoShould_ReturnErrorMessage_WhenMediaUploadFails()
    {

    }
    [Fact]
    public void DeleteMediaMementoShould_ReturnErrorMessage_WhenMediaDeleteFails()
    {

    }

    // Helper method to convert a hex string to a byte array for easier testing
    private static byte[] HexStringToByteArray(string hex)
    {
        // Removing any spaces or hyphens that might be present in the string
        hex = hex.Replace(" ", "").Replace("-", "");

        // Initialize a byte array with the length of half the hex string
        byte[] bytes = new byte[hex.Length / 2];

        // Loop through the hex string, converting two characters at a time
        for (int i = 0; i < bytes.Length; i++)
        {
            // Convert the number expressed in base-16 to an integer
            bytes[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
        }

        return bytes;
    }
}