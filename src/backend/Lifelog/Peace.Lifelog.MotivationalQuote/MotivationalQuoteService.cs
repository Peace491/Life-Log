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
    
    public async Task<Response> ICheckTime(Phrase phrase)
    {
        var response = new Response();

        if(DateTime.Parse(phrase.Time) < DateTime.Parse("11:59:59 PM"))
        {
            response.HasError = true;
            response.ErrorMessage = "Quote changed prior to 12:00 AM";
            return response;
        }

        if(DateTime.Parse(phrase.Time) > DateTime.Parse("00:00:03 AM"))
        {
            response.HasError = true;
            response.ErrorMessage = "Quote changed after 12:00 AM";
            return response;
        }     

        return response;           
    }
    
    public async Task<Response> ICheckPhrase(Phrase phrase)
    {
        var response = new Response();

        //var newQuote = $"SELECT Quote FROM LifelogQuote ORDER BY ID ASC LIMIT 1 OFFSET 1";
        
        //var newAuthor = $"SELECT Author FROM LifelogQuote ORDER BY ID ASC LIMIT 1 OFFSET 1";
        
        
        if (string.IsNullOrEmpty(phrase.Quote))
        {
            response.HasError = true;
            response.ErrorMessage = "The quote cannot be empty.";
            return response;
        }

        if (string.IsNullOrEmpty(phrase.Author)) 
        {
            response.HasError = true;
            response.ErrorMessage = "The author cannot be empty.";
            return response;
        }

        if (ICheckQuote(phrase) == false)
        {
            response = ICheckQuote(phrase);
            return response;
        }

        if (ICheckAuthor(phrase) == false)
        {
            response = ICheckAuthor(phrase);
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

    public async Task<Response> ISendPhrase(Phrase phrase)
    {
        var response = new Response();

        
    }
}