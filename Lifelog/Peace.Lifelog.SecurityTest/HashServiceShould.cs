namespace Peace.Lifelog.SecurityTest;
using Peace.Lifelog.DataAccess;
using Peace.Lifelog.Security;
using System.Diagnostics;

public class HashServiceShould
{

    private const int MAX_EXECUTION_TIME_IN_SECONDS = 3001;
    [Fact]
    public void HasherShould_ReproduceOutputsWithSameInput()
    {
        // Hash function should produce the same result on the same inputs
        // Arrange
        string hasherInput = "jackpickleissoCOOL707";

        Stopwatch timer = new Stopwatch();
        // Act
        timer.Start();
        var hashOne = HashService.Hasher(hasherInput);
        timer.Stop();
        var hashTwo = HashService.Hasher(hasherInput);

        // Assert
        // will need to change if we are using different method sig for hasher (ideally a response object?)
        Assert.True(hashOne == hashTwo);
        Assert.True(timer.ElapsedMilliseconds < MAX_EXECUTION_TIME_IN_SECONDS);
    }
    [Fact]
    public void HasherShouldNot_HashANullString()
    {
        // Arrange
        string hasherInput = null;
        var errorhash = "1234";

        // Act/Assert
        // This just highlights to me that we should be using a response object here
        Assert.Fail(HashService.Hasher(hasherInput));

        // Assert


    }
}
