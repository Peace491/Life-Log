using Peace.Lifelog.Security;

namespace Peace.Lifelog.SecurityTest;

using DomainModels;
using Peace.Lifelog.DataAccess;
using Peace.Lifelog.Security;
using System.Diagnostics;
using System.Net;
using System.Security.Policy;

public class SaltServiceShould
{
    private const int MAX_EXECUTION_TIME_IN_SECONDS = 3001;

    [Fact]
    public void getSaltShould_ProduceSalt()
    {
        // Arrange
        SaltService saltService = new SaltService();
        Stopwatch timer = new Stopwatch();

        // Act
        timer.Start();
        var saltResponse = saltService.getSalt();
        timer.Stop();

        // Assert
        Assert.False(saltResponse.HasError);
        Assert.True(timer.ElapsedMilliseconds < MAX_EXECUTION_TIME_IN_SECONDS);
    }

    [Fact]
    public void getSaltShould_ProduceUniqueSalts()
    {
        // Arrange
        SaltService saltService = new SaltService();
        Stopwatch timer = new Stopwatch();

        // Act
        timer.Start();
        var saltResponse = saltService.getSalt();
        var saltResponse2 = saltService.getSalt();
        timer.Stop();

        // Assert
        Assert.False(saltResponse.Output == saltResponse2.Output);
        Assert.True(timer.ElapsedMilliseconds < MAX_EXECUTION_TIME_IN_SECONDS);
    }
}
