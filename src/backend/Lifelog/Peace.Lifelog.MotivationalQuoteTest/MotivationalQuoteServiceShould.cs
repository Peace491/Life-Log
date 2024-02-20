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

    public async void MotivationalQuoteServiceShould_ImpartialQuote()
    {
        //Arrange
        string invalidQuote = "WrongQuote";
        string invalidAuthor = "WrongAuthor";

        //Act
        var validQuoteAuthorResponse = testRegUser.RegisterUser(invalidQuote, AUTHOR);

        // Assert
        Assert.True(validQuoteAuthorResponse == false);
    }

    public async void MotivationalQuoteServiceShould_ImpartialQuote()
    {
        //Arrange
        string invalidQuote = "WrongQuote";
        string invalidAuthor = "WrongAuthor";

        //Act
        var validQuoteAuthorResponse = testRegUser.RegisterUser(invalidQuote, AUTHOR);

        // Assert
        Assert.True(validQuoteAuthorResponse == false);
    }

    public async void MotivationalQuoteServiceShould_ImpartialQuote()
    {
        //Arrange
        string invalidQuote = "WrongQuote";
        string invalidAuthor = "WrongAuthor";

        //Act
        var validQuoteAuthorResponse = testRegUser.RegisterUser(invalidQuote, AUTHOR);

        // Assert
        Assert.True(validQuoteAuthorResponse == false);
    }

    public async void MotivationalQuoteServiceShould_ImpartialQuote()
    {
        //Arrange
        string invalidQuote = "WrongQuote";
        string invalidAuthor = "WrongAuthor";

        //Act
        var validQuoteAuthorResponse = testRegUser.RegisterUser(invalidQuote, AUTHOR);

        // Assert
        Assert.True(validQuoteAuthorResponse == false);
    }

    public async void MotivationalQuoteServiceShould_ImpartialQuote()
    {
        //Arrange
        string invalidQuote = "WrongQuote";
        string invalidAuthor = "WrongAuthor";

        //Act
        var validQuoteAuthorResponse = testRegUser.RegisterUser(invalidQuote, AUTHOR);

        // Assert
        Assert.True(validQuoteAuthorResponse == false);
    }

    public async void MotivationalQuoteServiceShould_ImpartialQuote()
    {
        //Arrange
        string invalidQuote = "WrongQuote";
        string invalidAuthor = "WrongAuthor";

        //Act
        var validQuoteAuthorResponse = testRegUser.RegisterUser(invalidQuote, AUTHOR);

        // Assert
        Assert.True(validQuoteAuthorResponse == false);
    }

    public async void MotivationalQuoteServiceShould_ImpartialQuote()
    {
        //Arrange
        string invalidQuote = "WrongQuote";
        string invalidAuthor = "WrongAuthor";

        //Act
        var validQuoteAuthorResponse = testRegUser.RegisterUser(invalidQuote, AUTHOR);

        // Assert
        Assert.True(validQuoteAuthorResponse == false);
    }
}