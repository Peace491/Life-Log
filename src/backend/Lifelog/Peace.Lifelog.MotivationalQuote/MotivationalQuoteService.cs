namespace Peace.Lifelog.MotivationalQuote;

using DomainModels;
using Peace.Lifelog.DataAccess;
using Peace.Lifelog.Logging;

public class MotivationalQuoteServiceShould : IGetPhrase//ICheckTime, ICheckQuote, ICheckAuthor, ISendPhrase
{
    private Phrase previous = new Phrase();
    private Phrase current = new Phrase();
    int offset = 1;

    public async Task<Response> GetPhrase(Phrase phrase)
    {
        var response = new Response();
        
        // Check time
        if (DateTime.Parse(current.Time) < DateTime.Parse("11:59:59 PM") || DateTime.Parse(current.Time) > DateTime.Parse("00:00:03 AM"))
        {
            response.HasError = true;
            response.ErrorMessage = "Quote changed outside the allowed time window.";
            LogError(response.ErrorMessage, "Business");
            return response;
        }

        // Check and get new quote and author
        var newQuote = $"SELECT Quote, Author FROM LifelogQuote ORDER BY ID ASC LIMIT 1 OFFSET \"{offset + 1}\"";
        var readDataOnlyDAO = new ReadDataOnlyDAO();
        var quoteResponse = await readDataOnlyDAO.ReadData(newQuote);

        if (quoteResponse.HasError)
        {
            response.HasError = true;
            response.ErrorMessage = "Error retrieving new quote and author.";
            LogError(response.ErrorMessage, "Data");
            return response;
        }

        var responseQuote = "";
        if (quoteResponse.Output != null)
        {
            responseQuote = UpdateCurrentPhrase(quoteResponse.Output);
        }


        // Check for errors in the new quote and author
        if (string.IsNullOrEmpty(current.Quote) || string.IsNullOrEmpty(current.Author))
        {
            response.HasError = true;
            response.ErrorMessage = "The quote or author cannot be empty.";
            LogError(response.ErrorMessage, "Data");
            return response;
        }

        if (previous.Quote == current.Quote)
        {
            response.HasError = true;
            response.ErrorMessage = "The Quote has been reused.";
            LogError(response.ErrorMessage, "Data");
            return response;
        }

        // Update previous phrase and increment offset
        previous = current;
        offset = (offset == 186) ? 1 : offset + 1;

        // Set the response output
        response.Output = new List<Object> { current };
        return response;
    }

    private List<Object>? UpdateCurrentPhrase(Response Response)
    {

        List<Object> phraseList = new List<Object>();

        if(Response.Output == null)
        {
            return null;
        }

        foreach (List<Object> Phrase in Response.Output)
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

    private void LogError(string errorMessage, string logType)
    {
        var createDataOnlyDAO = new CreateDataOnlyDAO();
        var logTarget = new LogTarget(createDataOnlyDAO);
        var logging = new Logging(logTarget);
        var logResponse = logging.CreateLog("Logs", "TxT3KzlpTG0ExziT6GhXfJDStrAssjrEZjbe14UBfvU=", "ERROR", logType, errorMessage);
    }
}