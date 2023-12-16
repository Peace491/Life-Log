namespace Peace.Lifelog.SecurityTest;

using DomainModels;
using Peace.Lifelog.DataAccess;
using Peace.Lifelog.Security;
using System.Diagnostics;
using System.Security.Policy;

public class HashServiceShould
{

    private const int MAX_EXECUTION_TIME_IN_SECONDS = 3001;
    private const string HASH_NULL_PASSWORD_MESSAGE = "Password is null";
    
    [Fact]
    public void HasherShould_ReproduceOutputsWithSameInput()
    {
        // Arrange
        HashService hashService = new HashService();
        Stopwatch timer = new Stopwatch();

        string hasherInput = "jackpickleissoCOOL707";
        string expectedHash = "TxT3KzlpTG0ExziT6GhXfJDStrAssjrEZjbe14UBfvU=";

        // Act
        timer.Start();
        var hashResponse = hashService.Hasher(hasherInput);
        timer.Stop();

        // Assert
        Assert.False(hashResponse.HasError);
        foreach (String hashOutput in hashResponse.Output)
        {
            Assert.True(hashOutput == expectedHash);
        }
        Assert.True(timer.ElapsedMilliseconds < MAX_EXECUTION_TIME_IN_SECONDS);
    }
    [Fact]
    public void HasherShouldNot_HashANullString()
    {
        // Arrange
        HashService hashService = new HashService();
        Stopwatch timer = new Stopwatch();

        string hasherInput = null;

        // Act
        timer.Start();
        var hashResponse = hashService.Hasher(hasherInput);
        timer.Stop();

        // Assert
        Assert.True(hashResponse.HasError);
        Assert.True(hashResponse.ErrorMessage == HASH_NULL_PASSWORD_MESSAGE);

    }
}
