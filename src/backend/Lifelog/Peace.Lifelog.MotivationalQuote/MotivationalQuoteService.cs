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


        // Check if there's already a quote for today  '{today:yyyy-MM-dd}'
        var checkTodayQuote = $"SELECT LifelogQuote_ID FROM LifelogQuoteOfTheDay WHERE Date = CURRENT_DATE";
        var todayQuoteResponse = await readDataOnlyDAO.ReadData(checkTodayQuote);

        var todayID = "";

        if (todayQuoteResponse.Output != null)
        {
            // There is already a quote for today
            foreach (List<object> output in todayQuoteResponse.Output)
            {
                todayID = output[0].ToString();
            }
            var getTodayQuote = $"SELECT Quote, Author FROM LifelogQuote WHERE ID = '{todayID}'";
            var getTodayQuoteResponse = await readDataOnlyDAO.ReadData(getTodayQuote);
            if (getTodayQuoteResponse.Output != null)
            {
                // There is already a quote for today
                response.Output = getTodayQuoteResponse.Output;
                response.ErrorMessage = "There is already a Quote for today";
                return response;
            }
        }



        // There's no quote for today, so let's retrieve a new one
        // Check the last entry in QuoteOfTheDay to ensure we don't repeat the same quote
        //WHERE Date = DATE_SUB(CURRENT_DATE, INTERVAL 1 DAY)
        var lastEntryOfTheDay = $"SELECT Date, LifelogQuote_ID FROM LifelogQuoteOfTheDay ORDER BY Date DESC";
        var lastDayResponse = await readDataOnlyDAO.ReadData(lastEntryOfTheDay, 1);

        var currentDate = "";
        var currentID = "";
        int currentIDNum = 0;

        if (lastDayResponse.Output != null)
        {
            foreach (List<object> output in lastDayResponse.Output)
            {
                currentDate = output[0].ToString();
                currentID = output[1].ToString();
            }
            if (currentID != null)
            {
                currentIDNum = int.Parse(currentID);
            }
        }

        var lastEntryQuote = $"SELECT Quote, Author FROM LifelogQuote WHERE ID = '{currentID}'";
        var lastQuoteResponse = await readDataOnlyDAO.ReadData(lastEntryQuote);


        if (lastQuoteResponse.Output != null)
        {
            foreach (List<object> output in lastQuoteResponse.Output)
            {
                lastPhrase.Quote = output[0].ToString()!;
                lastPhrase.Author = output[1].ToString()!;
            }
        }

        //WHERE ID = '{currentID}'SELECT COUNT(ID) FROM LifelogQuote
        var isLastID = $"SELECT COUNT(ID) FROM LifelogQuote";
        var lastIDResponse = await readDataOnlyDAO.ReadData(isLastID);
        var lastID = "";
        int lastIDNum = 0;
        if (lastIDResponse.Output != null)
        {
            foreach (List<object> output in lastIDResponse.Output)
            {
                lastID = output[0].ToString();
            }
            if (lastID != null)
            {
                lastIDNum = int.Parse(lastID);
                lastIDNum--;
            }
        }
        if (lastIDNum.ToString() == currentID)
        {
            currentIDNum = 1;
        }

        var newEntryQuote = $"SELECT Quote, Author FROM LifelogQuote ORDER BY ID ASC";
        var newQuoteResponse = await readDataOnlyDAO.ReadData(newEntryQuote, 1, currentIDNum);

        string newID = "";
        int newIDNum = 0;

        newIDNum = currentIDNum + 1;
        newID = newIDNum.ToString();

        if (newQuoteResponse.Output != null)
        {
            foreach (List<object> output in newQuoteResponse.Output)
            {
                phrase.Quote = output[0].ToString()!;
                phrase.Author = output[1].ToString()!;
            }
        }
        else
        {
            response.HasError = true;
            response.ErrorMessage = "Unidentifiable issue with Data Repository which resulted in ERROR";
            MotivationalLogger("ERROR", "Data", response.ErrorMessage);
        }


        ////previous placement for placeholder


        // We have a new quote, let's insert it into QuoteOfTheDay

        var insertQuote = $"INSERT INTO lifelogquoteoftheday(Date, LifelogQuote_ID) VALUES(CURRENT_DATE, '{newID}')";
        var insertResponse = await createDataOnlyDAO.CreateData(insertQuote);

        if (insertResponse.HasError == true)
        {
            // Handle error - could not insert the new quote of the day
            response.ErrorMessage = "Error inserting new quote of the day.";
            MotivationalLogger("ERROR", "Data", response.ErrorMessage);
        }

        //response.HasError = true;

        response = await CheckBusinessRules(phrase, lastPhrase, response, currentID!);

        if (response.HasError == true)
        {
            var placeholderRetrieve = $"SELECT Quote, Author FROM LifelogQuote ORDER BY ID DESC";
            var placeholderRetrieveResponse = await readDataOnlyDAO.ReadData(placeholderRetrieve, 1);

            response.HasError = false;
            response.ErrorMessage = "A placeholder message was displayed in place of a quote";
            MotivationalLogger("Warning", "business", response.ErrorMessage);
            response.Output = placeholderRetrieveResponse.Output;
            return response;
        }
        else
        {
            response.Output = newQuoteResponse.Output;
            return response;
        }

    }

    private async Task<Response> CheckBusinessRules(Phrase phrase, Phrase lastPhrase, Response response, string currentID)
    {
        var readDataOnlyDAO = new ReadDataOnlyDAO();

        var quoteChecker = $"SELECT ID FROM LifelogQuote WHERE Quote = \"{phrase.Quote}\"";
        var quoteCheckerResponse = await readDataOnlyDAO.ReadData(quoteChecker);

        if (quoteCheckerResponse.Output == null)
        {
            response.HasError = true;
            response.ErrorMessage = "A quote from the datastore was not displayed or partially displayed";
            MotivationalLogger("ERROR", "Data", response.ErrorMessage);
        }

        var authorChecker = $"SELECT ID FROM LifelogQuote WHERE Author = '{phrase.Author}'";
        var authorCheckerResponse = await readDataOnlyDAO.ReadData(authorChecker);

        if (authorCheckerResponse.Output == null)
        {
            response.ErrorMessage = "A quote from the datastore did not include the associated author";
            MotivationalLogger("Warning", "Data", response.ErrorMessage);
        }

        if (lastPhrase.Quote == phrase.Quote)
        {
            // Handle error - could not retrieve a new quote
            response.HasError = true;
            response.ErrorMessage = "The quotes have not been refreshed/changed.";
            MotivationalLogger("ERROR", "Data", response.ErrorMessage);
        }

        var monthChecker = $"SELECT Date From lifelogquoteoftheday WHERE Date < DATE_SUB(CURRENT_DATE, INTERVAL 180 DAY) AND LifelogQuote_ID = '{currentID}' ORDER BY Date DESC";
        var monthCheckerResponse = await readDataOnlyDAO.ReadData(monthChecker);
        if (monthCheckerResponse.Output != null)
        {
            response.HasError = true;
            response.ErrorMessage = "The quotes have not been recycled";
            MotivationalLogger("ERROR", "Data", response.ErrorMessage);
        }


        if (DateTime.Parse(phrase.Time) < DateTime.Parse("11:59:59 PM") && DateTime.Parse(phrase.Time) > DateTime.Parse("12:00:00 PM"))
        {
            //response.HasError = true;
            response.ErrorMessage = "The quotes has changed prior to 12:00 am PST";
            MotivationalLogger("Debug", "Business", response.ErrorMessage);
        }

        if (DateTime.Parse(phrase.Time) > DateTime.Parse("00:00:03 AM") && DateTime.Parse(phrase.Time) < DateTime.Parse("11:59:00 AM"))
        {
            //response.HasError = true;
            response.ErrorMessage = "The quotes has changed after 12:00 am PST";
            MotivationalLogger("Debug", "Business", response.ErrorMessage);
        }


        return response;
    }

    private void MotivationalLogger(string issueType, string logType, string errorMessage)
    {
        var createDataOnlyDAO = new CreateDataOnlyDAO();
        var readDataOnlyDAO = new ReadDataOnlyDAO();
        var logTarget = new LogTarget(createDataOnlyDAO, readDataOnlyDAO);
        var logging = new Logging(logTarget);
        var logResponse = logging.CreateLog("Logs", "TxT3KzlpTG0ExziT6GhXfJDStrAssjrEZjbe14UBfvU=", issueType, logType, errorMessage);
    }

}

