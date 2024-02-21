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
    private const string TIME = DateTime.Now.ToString("hh:mm:ss tt");

    public async void MotivationalQuoteServiceShould_ThrowAnErrorIfImpartialQuote()
    {
        //Arrange
        var wrongQuote = "TestMotivationalQuote";

        var invalidQuote = new Phrase();
        var invalidAuthor = new Phrase();
        
        invalidQuote.Quote = wrongQuote;
        invalidQuote.Author = AUTHOR;
        invalidAuthor.Quote = QUOTE; 
        invalidAuthor.Author = wrongAuthor; 

        //Act
        var validQuoteResponse = MotivationQuoteService.ICheckPhrase(invalidQuote);
        var validAuthorResponse = MotivationQuoteService.ICheckPhrase(invalidAuthor);

        // Assert
        Assert.True(validQuoteResponse.HasError == true);
        Assert.True(validAuthorResponse.HasError == true);
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
        correctPhrase.CurrentTime = priorTime; 
                

        //Act
        var validTimeResponse = MotivationQuoteService.ICheckPhrase(correctPhrase);

        // Assert
        Assert.True(validTimeResponse.HasError == true);
    }

    [Fact]
    public async void MotivationalQuoteServiceShould_ThrowAnErrorIfQuoteChangeAfter()
    {
        //Arrange
        string afterTime = "00:00:05 AM";

        var correctPhrase = new Phrase();
        
        correctPhrase.Quote = QUOTE;
        correctPhrase.Author = AUTHOR; 
        correctPhrase.CurrentTime = afterTime; 
                

        //Act
        var validTimeResponse = MotivationQuoteService.ICheckPhrase(correctPhrase);

        // Assert
        Assert.True(validTimeResponse.HasError == true);
    }

    
}