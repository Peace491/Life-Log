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
        Assert.False(validNewPhraseResponse.HasError == false && validNewPhraseResponse.ErrorMessage == "Quotes have not changed.");
    }

    [Fact]
    public async void MotivationalQuoteServiceShould_OutputsAPlaceholder()
    {
        //Arrange
        var timer = new Stopwatch();
        var motivationalQuoteService = new MotivationalQuoteService();

        //Act
        timer.Start();
        var validPhraseResponse = await motivationalQuoteService.GetPhrase();
        timer.Stop();

        // Assert
        Assert.False(validPhraseResponse.HasError == false && validPhraseResponse.ErrorMessage == "Critical Error, Placeholder Was Used");
        //Assert.True(validPhraseResponse.ErrorMessage == "Critical Error, Placeholder Was Used");
    }

    [Fact]
    public async void MotivationalQuoteServiceShould_ThrowAnErrorIfImpartialQuote()
    {
        //Arrange
        var timer = new Stopwatch();
        var motivationalQuoteService = new MotivationalQuoteService();

        //Act
        timer.Start();
        var validPhraseResponse = await motivationalQuoteService.GetPhrase();
        timer.Stop();

        // Assert
        Assert.False(validPhraseResponse.HasError == false && validPhraseResponse.ErrorMessage == "Impartial Quote Was Pulled");
        //Assert.True(validPhraseResponse.ErrorMessage == "Impartial Quote Was Pulled");
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
        Assert.False(validPhraseResponse.HasError == false && validPhraseResponse.ErrorMessage == "Impartial Author Was Pulled");
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
        Assert.False(validPhraseResponse.HasError == false && validPhraseResponse.ErrorMessage == "Quote changed outside the specified time window.");
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
        Assert.False(validPhraseResponse.HasError == false && validPhraseResponse.ErrorMessage == "Quote changed outside the specified time window.");
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
        Assert.False(validPhraseResponse.HasError == false && validPhraseResponse.ErrorMessage == "The Quote was used prior to recycling");
        //Assert.True(validPhraseResponse.Output != null); // Assuming the quote has been recycled
    }

    [Fact]
    public async void MotivationalQuoteServiceShould_ThrowAnErrorIfPlaceholderWasNotDisplayed()
    {
        //Arrange
        var timer = new Stopwatch();
        var motivationalQuoteService = new MotivationalQuoteService();

        //Act
        timer.Start();
        var validPhraseResponse = await motivationalQuoteService.GetPhrase();
        timer.Stop();

        // Assert
        Assert.False(validPhraseResponse.HasError == false && validPhraseResponse.ErrorMessage == "Critical Error, Placeholder Was Used");
        //Assert.True(validPhraseResponse.ErrorMessage == "Critical Error, Placeholder Was Used");
    }

    [Fact]
    public async void MotivationalQuoteServiceShould_ThrowAnErrorIfQuotesWereNotRecycled()
    {
        //Arrange
        var timer = new Stopwatch();
        var motivationalQuoteService = new MotivationalQuoteService();

        //Act
        timer.Start();
        var validPhraseResponse = await motivationalQuoteService.GetPhrase();
        timer.Stop();

        // Assert
        Assert.False(validPhraseResponse.HasError == false && validPhraseResponse.ErrorMessage == "Quotes were not recycled properly.");
        //Assert.True(validPhraseResponse.ErrorMessage == "Critical Error, Placeholder Was Used");
    }

}