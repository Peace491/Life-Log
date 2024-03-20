namespace Peace.Lifelog.MotivationalQuote;

using System.Collections.Generic;
using DomainModels;
using Peace.Lifelog.DataAccess;
using Peace.Lifelog.Logging;

public class MotivationalQuoteService : IGetPhrase
{


    public async Task<Response> GetPhrase()
    {
        var response = new Response();
        var newResponse = new Response();

        Phrase phrase = new Phrase();
        DateTime dateTime = DateTime.Now;
        phrase.Time = dateTime.ToString("hh:mm:ss tt");

        Phrase lastPhrase = new Phrase();

        var today = DateTime.Today;
        var readDataOnlyDAO = new ReadDataOnlyDAO();
        var createDataOnlyDAO = new CreateDataOnlyDAO();

        response.HasError = false;


        // Check if there's already a quote for today
        var checkTodayQuote = $"SELECT Date, LifelogQuote_ID FROM LifelogQuoteOfTheDay WHERE Date = '{today:yyyy-MM-dd}'";
        var todayQuoteResponse = await readDataOnlyDAO.ReadData(checkTodayQuote);

        if (todayQuoteResponse.Output != null)
        {
            // There is already a quote for today
            response.Output = new List<object> { todayQuoteResponse.Output };
            response.ErrorMessage = "There is already a Quote for today";
            return response;
        }

        /*
        if (todayQuoteResponse.Output == null)
        {
            // There is already a quote for today
            response.Output = new List<object> { todayQuoteResponse.Output };
            response.ErrorMessage = "There is already a Quote for today";
            return response;
        }
        */

        // There's no quote for today, so let's retrieve a new one
        // Check the last entry in QuoteOfTheDay to ensure we don't repeat the same quote
        var lastEntryOfTheDay = $"SELECT Date, LifelogQuote_ID FROM LifelogQuoteOfTheDay WHERE Date = DATE_SUB(CURRENT_DATE, INTERVAL 1 DAY)";
        var lastDayResponse = await readDataOnlyDAO.ReadData(lastEntryOfTheDay);

        string currentDate = "";
        string currentID = "";
        int currentIDNum = 0;

        if (lastDayResponse.Output != null)
        {
            foreach (List<object> output in lastDayResponse.Output)
            {
                currentDate = output[0].ToString();
                currentID = output[1].ToString();
            }
            currentIDNum = int.Parse(currentID);
        }

        var lastEntryQuote = $"SELECT Quote, Author FROM LifelogQuote WHERE ID = '{currentID}'";
        var lastQuoteResponse = await readDataOnlyDAO.ReadData(lastEntryQuote);

        string lastQuote = "";
        string lastAuthor = "";

        if (lastQuoteResponse.Output != null)
        {
            foreach (List<object> output in lastQuoteResponse.Output)
            {
                lastQuote = output[0].ToString();
                lastPhrase.Quote = output[0].ToString();
                lastAuthor = output[1].ToString();
                lastPhrase.Author = output[1].ToString();
            }
        }

        //WHERE ID = '{currentID}'
        var newEntryQuote = $"SELECT Quote, Author, ID FROM LifelogQuote ORDER BY ID ASC";
        var newQuoteResponse = await readDataOnlyDAO.ReadData(newEntryQuote, 1, currentIDNum);

        string newID = "";
        string newQuote = "";
        string newAuthor = "";
        int newIDNum = 0;

        newIDNum = currentIDNum + 1;
        newID = newIDNum.ToString();

        if (newQuoteResponse.Output != null)
        {
            foreach (List<object> output in newQuoteResponse.Output)
            {
                newQuote = output[0].ToString();
                phrase.Quote = output[0].ToString();
                newAuthor = output[1].ToString();
                phrase.Author = output[1].ToString();
            }
        }


        if (lastQuote == newQuote)
        {
            // Handle error - could not retrieve a new quote
            response.HasError = true;
            response.ErrorMessage = "Error retrieving new quote and author.";
            //MotivationalLogger("ERROR", "Data", response.ErrorMessage);
            /////////////////return response;
        }

        if (DateTime.Parse(phrase.Time) > DateTime.Parse("11:59:59 PM") || DateTime.Parse(phrase.Time) < DateTime.Parse("00:00:03 AM"))
        {
            response.HasError = true;
            response.ErrorMessage = "Quote changed outside the specified time window.";
            //MotivationalLogger("ERROR", "Business", response.ErrorMessage);
            //return response;
        }

        // We have a new quote, let's insert it into QuoteOfTheDay

        var insertQuote = $"INSERT INTO lifelogquoteoftheday(Date, LifelogQuote_ID) VALUES(CURRENT_DATE, '{newID}')";
        var insertResponse = await createDataOnlyDAO.CreateData(insertQuote);

        if (insertResponse.HasError)
        {
            // Handle error - could not insert the new quote of the day
            response.HasError = true;
            response.ErrorMessage = "Error inserting new quote of the day.";
            //MotivationalLogger("ERROR", "Data", response.ErrorMessage);
            ///////return response;
        }

        // Set the new quote as the output of the response  new List<Object> {  }
        response.Output = newQuoteResponse.Output;
        return response;
    }

    private void MotivationalLogger(string issueType, string logType, string errorMessage)
    {
        var createDataOnlyDAO = new CreateDataOnlyDAO();
        var logTarget = new LogTarget(createDataOnlyDAO);
        var logging = new Logging(logTarget);
        var logResponse = logging.CreateLog("Logs", "TxT3KzlpTG0ExziT6GhXfJDStrAssjrEZjbe14UBfvU=", issueType, logType, errorMessage);
    }

}

