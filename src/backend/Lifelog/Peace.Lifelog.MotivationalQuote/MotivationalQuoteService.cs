namespace Peace.Lifelog.MotivationalQuote;

using DomainModels;
using Peace.Lifelog.DataAccess;
using Peace.Lifelog.Logging;

public class MotivationalQuoteServiceShould : IGetQuote, ICheckTime, ICheckQuote, ICheckAuthor, ICheckPhrase
{
    private var previous = new Phrase();
    private var current = new Phrase();
    var offset = 1;
    public async Task<Response> GetQuote(Phrase phrase)
    {
        var response = new Response();
        
        previous = phrase;

        current = phrase;

        if(offset == 187)
        {
            offset = 1;
        }

        if(CheckTime(phrase) == false)
        {
            response = CheckTime(phrase);
            return response;
        }

        /*if(ICheckQuote(phrase) == false)
        {
            response = ICheckQuote(phrase);
            return response;
        }*/

        /*if(ICheckAuthor(phrase) == false)
        {
            response = ICheckAuthor(phrase);
            return response;
        }*/

        if(CheckPhrase(phrase) == false)
        {
            response = CheckPhrase(phrase);
            return response;
        }

        if(SendPhrase(phrase) == false)
        {
            response = SendPhrase(phrase);
            return response;
        }
        else
        {
            response = SendPhrase(phrase);
            previous = phrase;
            return response;
        }            
    }
    
    public async Task<Response> CheckTime(Phrase phrase)
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
    
    public async Task<Response> CheckPhrase(Phrase phrase)
    {
        var response = new Response();

        //var newQuote = $"SELECT Quote FROM LifelogQuote ORDER BY ID ASC LIMIT 1 OFFSET \"{offset}\"";
        
        //var newAuthor = $"SELECT Author FROM LifelogQuote ORDER BY ID ASC LIMIT 1 OFFSET \"{offset}\"";
        
        
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

        if (CheckQuote(phrase) == false)
        {
            response = CheckQuote(phrase);
            return response;
        }

        if (CheckAuthor(phrase) == false)
        {
            response = CheckAuthor(phrase);
            return response;
        }

        return response;
    }

    public async Task<Response> CheckQuote(Phrase phrase)
    {
        var response = new Response();

        var newQuote = $"SELECT Quote FROM LifelogQuote ORDER BY ID ASC LIMIT 1 OFFSET \"{offset}\"";
        
        if(previous.Quote == newQuote)
        {
            response.HasError = true;
            response.ErrorMessage = "The Quote has been reused.";
            return response;
        }

        if(phrase.Quote != current.Quote)
        {
            response.HasError = true;
            response.ErrorMessage = "The Quote has not been pulled properly.";
            return response;
        }

        return response;
    }

    public async Task<Response> CheckAuthor(Phrase phrase)
    {
        var response = new Response();

        var newAuthor = $"SELECT Author FROM LifelogQuote ORDER BY ID ASC LIMIT 1 OFFSET \"{offset}\""+;

        if(phrase.Author != newAuthor)
        {
            response.HasError = true;
            response.ErrorMessage = "The Author has not been pulled properly.";
            return response;
        }

        return response;  
    }

    public async Task<Response> SendPhrase(Phrase phrase)
    {
        var response = new Response();

        var newQuote = $"SELECT Quote FROM LifelogQuote ORDER BY ID ASC LIMIT 1 OFFSET \"{offset}\"";
        var newAuthor = $"SELECT Author FROM LifelogQuote ORDER BY ID ASC LIMIT 1 OFFSET \"{offset}\"";
        var newTime = DateTime.Now.ToString("hh:mm:ss tt");

        current.Quote = newQuote;
        current.Author = newAuthor;

        if (CheckQuote(phrase) == false)
        {
            response = ICheckQuote(current);
            //return response;
        }

        if (CheckAuthor(current) == false)
        {
            response = ICheckAuthor(current);
            //return response;
        }
        
        if(response.HasError == true)
        {
            /*
            implement placeholder message in database and here
            current.Quote = $"SELECT Quote FROM LifelogQuote ORDER BY ID ASC LIMIT 1 OFFSET 188";
            current.Author = $"SELECT Author FROM LifelogQuote ORDER BY ID ASC LIMIT 1 OFFSET 188";
            */
        }
        response.output = current;
        return response;
        
    }
}