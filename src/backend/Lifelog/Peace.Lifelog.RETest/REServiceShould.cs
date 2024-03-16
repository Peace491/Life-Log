namespace Peace.Lifelog.RETest;
using Peace.Lifelog.RE;
public class ReServiceShould
{
    [Fact]
    public void Test_Test()
    {
        var reService = new ReService();
        var response = reService.getNumRecs("System", 1);
        Assert.NotNull(response);
    }
}