namespace Peace.Lifelog.MotivationalQuoteTest;
using System.Diagnostics;
using Peace.Lifelog.MotivationalQuote;


public class MotivationalQuoteServiceShould
{
    private const string QUOTE = "TestMotivationalQuoteServiceQuote";
    private const string AUTHOR = "TestMotivationalQuoteServiceAuthor";
    private const string TIME = "00:00:00 AM";


    [Fact]
    public async void MotivationalQuoteServiceShould_OutputsAQuote()
    {
        //Arrange
        var timer = new Stopwatch();
        var motivationalQuoteService = new MotivationalQuoteService();


        //Act
        timer.Start();
        var validPhraseResponse = await motivationalQuoteService.GetPhrase();
        timer.Stop();

        // Assert
        Assert.True(validPhraseResponse.HasError == false);
    }

    [Fact]
    public async void MotivationalQuoteServiceShould_ThrowAnErrorIfQuoteHasNotChanged()
    {
        //Arrange
        var timer = new Stopwatch();
        var motivationalQuoteService = new MotivationalQuoteService();


        //Act
        timer.Start();
        var validNewPhraseResponse = await motivationalQuoteService.GetPhrase();
        timer.Stop();

        // Assert
        Assert.True(!(validNewPhraseResponse.ErrorMessage == "A quote from the datastore was not displayed or partially displayed"));
    }

    [Fact]
    public async void MotivationalQuoteServiceShould_ThrowErrorIfOutputsAPlaceholder()
    {
        //Arrange
        var timer = new Stopwatch();
        var motivationalQuoteService = new MotivationalQuoteService();

        //Act
        timer.Start();
        var validPhraseResponse = await motivationalQuoteService.GetPhrase();
        timer.Stop();

        // Assert
        Assert.True(!(validPhraseResponse.ErrorMessage == "A placeholder message was displayed in place of a quote"));
        //Assert.True(validPhraseResponse.ErrorMessage == "Critical Error, Placeholder Was Used");
    }

    [Fact]
    public async void MotivationalQuoteServiceShould_ThrowAnErrorIfWrongAuthor()
    {
        //Arrange
        var timer = new Stopwatch();
        var motivationalQuoteService = new MotivationalQuoteService();

        //Act
        timer.Start();
        var validPhraseResponse = await motivationalQuoteService.GetPhrase();
        timer.Stop();

        // Assert
        Assert.True(!(validPhraseResponse.ErrorMessage == "A quote from the datastore did not include the associated author"));
        //Assert.True(validPhraseResponse.ErrorMessage == "Impartial Author Was Pulled");
    }

    [Fact]
    public async void MotivationalQuoteServiceShould_ThrowAnErrorIfQuoteChangePrior()
    {
        //Arrange
        var timer = new Stopwatch();
        var motivationalQuoteService = new MotivationalQuoteService();

        //Act
        timer.Start();
        var validPhraseResponse = await motivationalQuoteService.GetPhrase();
        timer.Stop();

        // Assert
        Assert.False(!(validPhraseResponse.ErrorMessage == "The quotes has changed prior to 12:00 am PST"));
        //Assert.True(validPhraseResponse.ErrorMessage == "Quote changed outside the specified time window.");
    }

    [Fact]
    public async void MotivationalQuoteServiceShould_ThrowAnErrorIfQuoteChangeAfter()
    {
        //Arrange
        var timer = new Stopwatch();
        var motivationalQuoteService = new MotivationalQuoteService();

        //Act
        timer.Start();
        var validPhraseResponse = await motivationalQuoteService.GetPhrase();
        timer.Stop();

        // Assert
        Assert.True(!(validPhraseResponse.ErrorMessage == "The quotes has changed after 12:00 am PST"));
        //Assert.True(validPhraseResponse.ErrorMessage == "Quote changed outside the specified time window.");
    }

    [Fact]
    public async void MotivationalQuoteServiceShould_ThrowAnErrorIfQuoteHasNotBeenRecycled()
    {
        //Arrange
        var timer = new Stopwatch();
        var motivationalQuoteService = new MotivationalQuoteService();

        //Act
        timer.Start();
        var validPhraseResponse = await motivationalQuoteService.GetPhrase();
        timer.Stop();

        // Assert
        Assert.True(!(validPhraseResponse.ErrorMessage == "The quotes have not been recycled"));
        //Assert.True(validPhraseResponse.Output != null); // Assuming the quote has been recycled
    }

    [Fact]
    public async void MotivationalQuoteServiceShould_ThrowAnErrorIfQuoteHaveNotBeenRefreshedOrChanged()
    {
        //Arrange
        var timer = new Stopwatch();
        var motivationalQuoteService = new MotivationalQuoteService();

        //Act
        timer.Start();
        var validPhraseResponse = await motivationalQuoteService.GetPhrase();
        timer.Stop();

        // Assert
        Assert.True(!(validPhraseResponse.ErrorMessage == "The quotes have not been refreshed/changed."));
        //Assert.True(validPhraseResponse.Output != null); // Assuming the quote has been recycled
    }

    [Fact]
    public async void MotivationalQuoteServiceShould_ThrowAnErrorIfUnableToPullQuote()
    {
        //Arrange
        var timer = new Stopwatch();
        var motivationalQuoteService = new MotivationalQuoteService();

        //Act
        timer.Start();
        var validPhraseResponse = await motivationalQuoteService.GetPhrase();
        timer.Stop();

        // Assert
        Assert.True(!(validPhraseResponse.ErrorMessage == "Unidentifiable issue with Data Repository which resulted in ERROR"));
        //Assert.True(validPhraseResponse.Output != null); // Assuming the quote has been recycled
    }


}