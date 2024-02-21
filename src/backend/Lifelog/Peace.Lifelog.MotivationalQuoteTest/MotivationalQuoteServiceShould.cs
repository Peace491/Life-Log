namespace Peace.Lifelog.MotivationalQuoteTest;

using System.Threading.Tasks;
using DomainModels;
using Peace.Lifelog.DataAccess;
using Peace.Lifelog.MotivationalQuote;
using System.Diagnostics;

[Fact]
public class MotivationalQuoteServiceShould: IAsyncLifetime, IDisposable
{
    private const string QUOTE = "TestMotivationalQuoteServiceQuote";
    private const string AUTHOR = "TestMotivationalQuoteServiceAuthor";
    private const string TIME = "00:00:00 AM";
    
    [Fact]
    public async void MotivationalQuoteServiceShould_OutputsAQuote()
    {
        //Arrange
        var correctPhrase = new Phrase();
        
        correctPhrase.Quote = QUOTE;
        correctPhrase.Author = AUTHOR;
        correctPhrase.Time = TIME;
        

        //Act
        var validQuoteResponse = MotivationQuoteService.IGetPhrase(correctPhrase);

        // Assert
        Assert.True(validPhraseResponse.HasError == false);
        Assert.True(validPhraseResponse.ErrorMessage == "The phrase is correct.");
    }

    [Fact]
    public async void MotivationalQuoteServiceShould_OutputsAPlaceholder()
    {
        //Arrange
        var correctPhrase = new Phrase();
        
        correctPhrase.Quote = "ERROR";
        correctPhrase.Author = "ERROR";
        correctPhrase.Time = "ERROR";
        

        //Act
        var validQuoteResponse = MotivationQuoteService.ISendPhrase(correctPhrase);

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
        var validQuoteResponse = MotivationQuoteService.ICheckPhrase(invalidQuote);

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
        var validAuthorResponse = MotivationQuoteService.RegisterUser(invalidAuthor);

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
    }*/

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
        var validTimeResponse = MotivationQuoteService.ICheckPhrase(correctPhrase);

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
        var validTimeResponse = MotivationQuoteService.ICheckPhrase(correctPhrase);

        // Assert
        Assert.True(validTimeResponse.HasError == true);
        Assert.True(validTimeResponse.ErrorMessage == "The Quote changed after 12:00 AM.");
    }

    [Fact]
    public async void MotivationalQuoteServiceShould_ThrowAnErrorIfQuoteHasNotChanged()
    {
        //Arrange
        string priorTime = "11:59:55 PM";
        string afterTime = "00:00:05 AM";
        string newQuote = "new";
        string newAuthor = "new";

        var correctPhrase = new Phrase();
        var newPhrase = new Phrase();
        
        correctPhrase.Quote = QUOTE;
        correctPhrase.Author = AUTHOR; 
        correctPhrase.Time = priorTime; 
        newPhrase.Quote = newQuote;
        newPhrase.Author = newAuthor; 
        newPhrase.Time = afterTime;
                

        //Act
        var validPhraseResponse = MotivationQuoteService.ICheckPhrase(correctPhrase);
        var validNewPhraseResponse = MotivationQuoteService.ICheckPhrase(newPhrase);

        // Assert
        Assert.True(validNewPhraseResponse.HasError == true);
        Assert.True(validNewPhraseResponse.ErrorMessage == "Quotes have not changed.");
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
    }
}