namespace Peace.Lifelog.RETest;
using Peace.Lifelog.RE;
public class ReServiceShould
{
    [Fact]
    public void Test_Test()
    {
        var reService = new ReService();
        var response = reService.getNumRecs("0Yg6cgh/M4+ImmL0GozWqhgcDCqTZEhzm9angvVAC30=", 1);
        Assert.NotNull(response);
    }
}