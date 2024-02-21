namespace Peace.Lifelog.MotivationalQuote;

using DomainModels;
using Peace.Lifelog.DataAccess;
using Peace.Lifelog.Logging;

public class MotivationalQuoteServiceShould : IReadPhrase, ICheckPhrase, ISendPhrase
{
    private var previous = new Phrase();

    public async Task<Response> IGetQuote(Phrase phrase)
    {
        var response = new Response();
        
        if(ICheckTime(phrase) == false)
        {
            response = ICheckTime(phrase);
            return response;
        }

        if(ICheckQuote(phrase) == false)
        {
            response = ICheckQuote(phrase);
            return response;
        }

        if(ICheckAuthor(phrase) == false)
        {
            response = ICheckAuthor(phrase);
            return response;
        }

        if(ISendPhrase(phrase) == false)
        {
            response = ISendPhrase(phrase);
            return response;
        }
        else
        {
            response = ISendPhrase(phrase);
            previous = phrase;
            return response;
        }            
    }
    
    public async Task<Response> IReadPhrase(Phrase phrase)
    {
        var response = new Response();

        if (string.IsNullOrEmpty(phrase.Quote))
        {
            response.HasError = true;
            response.ErrorMessage = "The quote cannot be empty.";
            return response;
        }
    }

    public async Task<Response> ICheckQuote(Phrase phrase)
    {
        var response = new Response();

                
    }

    public async Task<Response> ICheckAuthor(Phrase phrase)
    {
        var response = new Response();

               
    }

    public async Task<Response> ICheckTime(Phrase phrase)
    {
        var response = new Response();

                
    }

    public async Task<Response> ISendPhrase(Phrase phrase)
    {
        var response = new Response();

        
    }
}