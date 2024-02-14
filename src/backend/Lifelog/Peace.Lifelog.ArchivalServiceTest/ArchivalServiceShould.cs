namespace Peace.Lifelog.ArchivalServiceTest;

public class ArchivalServiceShould
{
    // On Success
    [Fact]
    public void ArchivalServiceShould_SelectLogsOlderThan30DaysOld()
    {

    }
    [Fact]
    public void ArchivalServiceShould_DeleteArchivedLogsFromLogTable()
    {

    }
    [Fact]
    public void ArchivalServiceShould_UploadCompressedFiletoS3Bucket()
    {

    }

    // On Failure
    [Fact]
    public void ArchiveShouldNot_DeleteLogsIfEncountersError()
    {

    }
    [Fact]
    public void ArchiveShouldNot_ContinueOnInvalidDatetime()
    {

    }
    [Fact]
    public void ArchiveShouldNot_ContinueOnDbInaccessable()
    {

    }
    [Fact]
    public void ArchiveShouldNot_ContinueOnS3InstanceInaccessable()
    {

    }
    [Fact]
    public void ArchiveShouldNot_ContinueOnUnableToWriteToFile()
    {

    }
}