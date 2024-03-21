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
        var validNewPhraseResponse = await motivationalQuoteService.GetPhrase();

        // Assert
        Assert.True(validNewPhraseResponse.HasError != true && validNewPhraseResponse.ErrorMessage != "Quotes have not changed.");
        //Assert.True(validNewPhraseResponse.ErrorMessage == "Quotes have not changed.");
    }

    /*[Fact]
    public async void MotivationalQuoteServiceShould_OutputsAPlaceholder()
    {
        //Arrange
        var correctPhrase = new Phrase();
        
        correctPhrase.Quote = "ERROR";
        correctPhrase.Author = "ERROR";
        correctPhrase.Time = "ERROR";
        

        //Act
        var validQuoteResponse = MotivationQuoteService.IGetQuote(correctPhrase);

        // Assert
        Assert.True(validPhraseResponse.HasError == true);
        Assert.True(validPhraseResponse.ErrorMessage == "The phrase outputed a placeholder");
    }
    
    [Fact]
    public async void MotivationalQuoteServiceShould_ThrowAnErrorIfImpartialQuote()
    {
        //Arrange
        var wrongQuote = "TestMotivationalQuote";

        var invalidQuote = new Phrase();
        
        invalidQuote.Quote = wrongQuote;
        invalidQuote.Author = AUTHOR;

        //Act
        var validQuoteResponse = MotivationQuoteService.ICheckQuote(invalidQuote);

        // Assert
        Assert.True(validQuoteResponse.HasError == true);
        Assert.True(validQuoteResponse.ErrorMessage == "Impartial Quote was pulled.");
    }

    [Fact]
    public async void MotivationalQuoteServiceShould_ThrowAnErrorIfWrongAuthor()
    {
        //Arrange
        var wrongAuthor = "WrongAuthor";

        var invalidAuthor = new Phrase();
        
        invalidAuthor.Quote = QUOTE;
        invalidAuthor.Author = wrongAuthor; 

        //Act
        var validAuthorResponse = MotivationQuoteService.ICheckAuthor(invalidAuthor);

        // Assert
        Assert.True(validAuthorResponse.HasError == true);
        Assert.True(validAuthorResponse.ErrorMessage == "Associated Author was not pulled.");
    }

    /*[Fact]
    public async void MotivationalQuoteServiceShould_ThrowAnErrorIfUnableToRetrievePhrase()
    {
        //Arrange
        var wrongQuote = "WrongQuote";
        var wrongAuthor = "WrongAuthor";

        var invalidQuote = new Phrase();
        var invalidAuthor = new Phrase();
        
        invalidQuote.Quote = (wrongQuote, AUTHOR);
        invalidAuthor.Author = (Quote, wrongAuthor); 

        //Act
        var validQuoteResponse = MotivationQuoteService.RegisterUser(invalidQuote);
        var validAuthorResponse = MotivationQuoteService.RegisterUser(invalidAuthor);

        // Assert
        Assert.True(validQuoteResponse.HasError == true);
        Assert.True(validAuthorResponse.HasError == true);
    }

    [Fact]
    public async void MotivationalQuoteServiceShould_ThrowAnErrorIfQuoteChangePrior()
    {
        //Arrange
        string priorTime = "11:59:55 PM";

        var correctPhrase = new Phrase();
        
        correctPhrase.Quote = QUOTE;
        correctPhrase.Author = AUTHOR; 
        correctPhrase.Time = priorTime; 
                

        //Act
        var validTimeResponse = MotivationQuoteService.ICheckTime(correctPhrase);

        // Assert
        Assert.True(validTimeResponse.HasError == true);
        Assert.True(validTimeResponse.ErrorMessage == "The Quote changed prior to 12:00 AM.");
    }

    [Fact]
    public async void MotivationalQuoteServiceShould_ThrowAnErrorIfQuoteChangeAfter()
    {
        //Arrange
        string afterTime = "00:00:05 AM";

        var correctPhrase = new Phrase();
        
        correctPhrase.Quote = QUOTE;
        correctPhrase.Author = AUTHOR; 
        correctPhrase.Time = afterTime; 
                

        //Act
        var validTimeResponse = MotivationQuoteService.ICheckTime(correctPhrase);

        // Assert
        Assert.True(validTimeResponse.HasError == true);
        Assert.True(validTimeResponse.ErrorMessage == "The Quote changed after 12:00 AM.");
    }

    

    [Fact]
    public async void MotivationalQuoteServiceShould_ThrowAnErrorIfQuoteHasNotBeenRecycled()
    {
        //Arrange
        var correctPhrase = new Phrase();
        var newPhrase = new Phrase();
        
        correctPhrase.Quote = QUOTE;
        correctPhrase.Author = AUTHOR; 
        correctPhrase.Time = TIME; 
        newPhrase.Quote = newQuote;
        newPhrase.Author = newAuthor; 
        newPhrase.Time = TIME;
                

        //Act
        var validPhraseResponse = MotivationQuoteService.ICheckPhrase(correctPhrase);
        var validNewPhraseResponse = MotivationQuoteService.ICheckPhrase(newPhrase);

        // Assert
        Assert.True(validNewPhraseResponse.HasError == true);
        Assert.True(validNewPhraseResponse.ErrorMessage == "Quotes have not been recycled.");
    }

    [Fact]
    public async void MotivationalQuoteServiceShould_ThrowAnErrorIfPlaceholderWasNotDisplayed()
    {
        //Arrange
        var correctPhrase = new Phrase();
        
        correctPhrase.Quote = "ERROR";
        correctPhrase.Author = "ERROR"; 
        correctPhrase.Time = "ERROR"; 
                

        //Act
        var validPhraseResponse = MotivationQuoteService.ISendPhrase(correctPhrase);

        // Assert
        Assert.True(validPhraseResponse.HasError == true);
        Assert.True(validPhraseResponse.ErrorMessage == "Placeholder message was not displayed");
    }*/
}