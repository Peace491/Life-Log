namespace Peace.Lifelog.MotivationalQuote;

using DomainModels;
using Peace.Lifelog.DataAccess;
using Peace.Lifelog.Logging;

public class MotivationalQuoteServiceShould : IGetQuote
{
    private Phrase previous = new Phrase();
    
    private Phrase current = new Phrase();
    
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

        if(!CheckTime(phrase))
        {
            response = CheckTime(phrase);
            //return response;
        }

        else if(!CheckPhrase(phrase))
        {
            response = CheckPhrase(phrase);
            //return response;
        }

        else if(!SendPhrase(phrase))
        {
            response = SendPhrase(phrase);
            //return response;
        }
        else
        {
            response = SendPhrase(phrase);
            previous = phrase;
            //return response;
        } 
        var currentOutput = ConvertDatabaseResponseOutputToPhraseObjectList(response);
        response.Output = currentOutput;
        return response;           
    }
    
    public async Task<Response> CheckTime(Phrase phrase)
    {
        var response = new Response();

        if(DateTime.Parse(phrase.Time) < DateTime.Parse("11:59:59 PM"))
        {
            response.HasError = true;
            response.ErrorMessage = "Quote changed prior to 12:00 AM";
            //return response;
        }

        if(DateTime.Parse(phrase.Time) > DateTime.Parse("00:00:03 AM"))
        {
            response.HasError = true;
            response.ErrorMessage = "Quote changed after 12:00 AM";
            //return response;
        }
        /* Need to do
        #region Log
        var createDataOnlyDAO = new CreateDataOnlyDAO();
        var logTarget = new LogTarget(createDataOnlyDAO);
        var logging = new Logging.Logging(logTarget);

        if (response.HasError) 
        {
            var errorMessage = response.ErrorMessage;
            var logResponse = logging.CreateLog("Logs", "Debug", "Business", errorMessage);
        }
        /*else 
        {
            var logResponse =  logging.CreateLog();
        }*/
        #endregion     

        return response;           
    }
    
    public async Task<Response> CheckPhrase(Phrase phrase)
    {
        var response = new Response();
        /* Need To Restructure
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
        }*/

        if (!CheckQuote(phrase))
        {
            response = CheckQuote(phrase);
            return response;
        }

        if (!CheckAuthor(phrase))
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
        
        var readDataOnlyDAO = new ReadDataOnlyDAO();

        var quoteCheckResponse = await readDataOnlyDAO.ReadData(newQuote);
        
        foreach(List<Object> phraseOutput in quoteCheckResponse.Output)
        {
            foreach(var attribute in phraseOutput)
            {
                if(previous.Quote == newQuote)
                {
                    response.HasError = true;
                    response.ErrorMessage = "The Quote has been reused.";
                    return response;
                }

                else if(phrase.Quote != current.Quote)
                {
                    response.HasError = true;
                    response.ErrorMessage = "The Quote has not been pulled properly.";
                    return response;
                }
            }
        }
        /* Need to do
        #region Log
        var createDataOnlyDAO = new CreateDataOnlyDAO();
        var logTarget = new LogTarget(createDataOnlyDAO);
        var logging = new Logging.Logging(logTarget);

        if (response.HasError) 
        {
            var errorMessage = response.ErrorMessage;
            var logResponse = logging.CreateLog("Logs", "Debug", "Business", errorMessage);
        }
        /*else 
        {
            var logResponse =  logging.CreateLog();
        }*/
        #endregion

        return response;
    }

    public async Task<Response> CheckAuthor(Phrase phrase)
    {
        var response = new Response();

        var newAuthor = $"SELECT Author FROM LifelogQuote ORDER BY ID ASC LIMIT 1 OFFSET \"{offset}\"";

        var readDataOnlyDAO = new ReadDataOnlyDAO();

        var authorCheckResponse = await readDataOnlyDAO.ReadData(newAuthor);
        
        foreach(List<Object> phraseOutput in authorCheckResponse.Output)
        {
            foreach(var attribute in phraseOutput)
            {
                if(phrase.Author != newAuthor)
                {
                    response.HasError = true;
                    response.ErrorMessage = "The Author has not been pulled properly.";
                    return response;
                }
            }
        }

        /* Need to do
        #region Log
        var createDataOnlyDAO = new CreateDataOnlyDAO();
        var logTarget = new LogTarget(createDataOnlyDAO);
        var logging = new Logging.Logging(logTarget);

        if (response.HasError) 
        {
            var errorMessage = response.ErrorMessage;
            var logResponse = logging.CreateLog("Logs", "Debug", "Business", errorMessage);
        }
        /*else 
        {
            var logResponse =  logging.CreateLog();
        }*/
        #endregion

        return response;  
    }

    public async Task<Response> SendPhrase(Phrase phrase)
    {
        var response = new Response();

        var newQuote = $"SELECT Quote FROM LifelogQuote ORDER BY ID ASC LIMIT 1 OFFSET \"{offset}\"";
        var newAuthor = $"SELECT Author FROM LifelogQuote ORDER BY ID ASC LIMIT 1 OFFSET \"{offset}\"";
        var newTime = DateTime.Now.ToString("hh:mm:ss tt");

        var placeHolderQuote = $"SELECT Quote FROM LifelogQuote ORDER BY ID ASC LIMIT 1 OFFSET 188";
        var placeHolderAuthor = $"SELECT Author FROM LifelogQuote ORDER BY ID ASC LIMIT 1 OFFSET 188";

        current.Quote = newQuote;
        current.Author = newAuthor;

        if (!CheckQuote(phrase))
        {
            response = CheckQuote(current);
            //return response;
        }

        if (!CheckAuthor(current))
        {
            response = CheckAuthor(current);
            //return response;
        }
        
        if(response.HasError == true)
        {
            //implement placeholder message in database and here
            current.Quote = placeHolderQuote;
            current.Author = placeHolderAuthor;
            var currentOutput = ConvertDatabaseResponseOutputToPhraseObjectList(response);
            response.Output = currentOutput;
            return response;
        }
        /* Need to do
        #region Log
        var createDataOnlyDAO = new CreateDataOnlyDAO();
        var logTarget = new LogTarget(createDataOnlyDAO);
        var logging = new Logging.Logging(logTarget);

        if (response.HasError) 
        {
            var errorMessage = response.ErrorMessage;
            var logResponse = logging.CreateLog("Logs", "Debug", "Business", errorMessage);
        }
        /*else 
        {
            var logResponse =  logging.CreateLog();
        }*/
        #endregion

        var currentOutput = ConvertDatabaseResponseOutputToPhraseObjectList(response);
        response.output = currentOutput;
        return response;
        
    }

    private List<Object>? ConvertDatabaseResponseOutputToPhraseObjectList(Response Response)
    {
        List<Object> phraseList = new List<Object>();

        if(response.Output == null)
        {
            return null;
        }

        foreach (List<Object> Phrase in response.Output)
        {

            //var phrase = new Phrase();

            int index = 0;

            foreach (var attribute in Phrase)
            {
                if(attribute is null) continue;

                switch(index)
                {
                    case 0:
                        current.Quote = attribute.ToString() ?? "";
                        break;
                    case 1:
                        current.Author = attribute.ToString() ?? "";
                        break;
                    case 2:
                        current.Time = attribute.ToString() ?? "";
                        break;
                    default:
                        break;
                }
                index++;

            }

            phraseList.Add(current);
        }

        return phraseList;
    }
}