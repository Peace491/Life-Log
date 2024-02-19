namespace Peace.Lifelog.MotivationalQuoteTest;

using System.Threading.Tasks;
using DomainModels;
using Peace.Lifelog.DataAccess;
using Peace.Lifelog.MotivationalQuote;
using System.Diagnostics;

public class MotivationalQuoteServiceShould: IAsyncLifetime, IDisposable
{
    private const string QUOTE = "TestMotivationalQuoteServiceQuote";
    private const string AUTHOR = "TestMotivationalQuoteServiceAuthor";

    public async void MotivationalQuoteServiceShould_OutputANewPhrase()
    {
        //Arrange
        
    }
}