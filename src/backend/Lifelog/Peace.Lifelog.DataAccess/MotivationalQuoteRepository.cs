namespace Peace.Lifelog.DataAccess;

using DomainModels;

public class MotivationalQuoteRepository : IMotivationalQuoteRepository
{
    ReadDataOnlyDAO readDataOnlyDAO = new ReadDataOnlyDAO();
    CreateDataOnlyDAO createDataOnlyDAO = new CreateDataOnlyDAO();
    public async Task<Response> CheckTodayQuote()
    {
        try
        {
            //ReadDataOnlyDAO readDataOnlyDAO = new ReadDataOnlyDAO();
            var todayQuote = $"SELECT LifelogQuote_ID FROM LifelogQuoteOfTheDay WHERE Date = CURRENT_DATE";
            var todayQuoteResponse = await readDataOnlyDAO.ReadData(todayQuote);
            return todayQuoteResponse;

        }
        catch (Exception ex)
        {
            var createDataOnlyDAO = new CreateDataOnlyDAO();
            var logTarget = new LogTarget(createDataOnlyDAO);
            var logging = new Logging(logTarget);
            var logResponse = logging.CreateLog("Logs", "TxT3KzlpTG0ExziT6GhXfJDStrAssjrEZjbe14UBfvU=", "", "", "");
            throw;
        }
    }

    public async Task<Response> GetTodayQuote(string todayID)
    {
        try
        {
            //ReadDataOnlyDAO readDataOnlyDAO = new ReadDataOnlyDAO();
            var todayQuote = $"SELECT Quote, Author FROM LifelogQuote WHERE ID = '{todayID}'";
            var todayQuoteResponse = await readDataOnlyDAO.ReadData(todayQuote);
            return todayQuoteResponse;

        }
        catch (Exception ex)
        {
            throw;
        }
    }

    public async Task<Response> LastEntryOfTheDay()
    {
        try
        {
            //ReadDataOnlyDAO readDataOnlyDAO = new ReadDataOnlyDAO();
            var lastDayQuote = $"SELECT Date, LifelogQuote_ID FROM LifelogQuoteOfTheDay WHERE Date = DATE_SUB(CURRENT_DATE, INTERVAL 1 DAY)";
            var lastDayQuoteResponse = await readDataOnlyDAO.ReadData(lastDayQuote);
            return lastDayQuoteResponse;

        }
        catch (Exception ex)
        {
            throw;
        }
    }

    public async Task<Response> LastEntryQuote(string currentID)
    {
        try
        {
            //ReadDataOnlyDAO readDataOnlyDAO = new ReadDataOnlyDAO();
            var lastQuote = $"SELECT Quote, Author FROM LifelogQuote WHERE ID = '{currentID}'";
            var lastQuoteResponse = await readDataOnlyDAO.ReadData(lastQuote);
            return lastQuoteResponse;

        }
        catch (Exception ex)
        {
            throw;
        }
    }

    public async Task<Response> IsLastID()
    {
        try
        {
            //ReadDataOnlyDAO readDataOnlyDAO = new ReadDataOnlyDAO();
            var isLastID = $"SELECT COUNT(ID) FROM LifelogQuote";
            var lastIDResponse = await readDataOnlyDAO.ReadData(isLastID);
            return lastIDResponse;

        }
        catch (Exception ex)
        {
            throw;
        }
    }

    public async Task<Response> NewEntryQuote(int currentIDNum)
    {
        try
        {
            //ReadDataOnlyDAO readDataOnlyDAO = new ReadDataOnlyDAO();
            var newEntryQuote = $"SELECT Quote, Author FROM LifelogQuote ORDER BY ID ASC";
            var newQuoteResponse = await readDataOnlyDAO.ReadData(newEntryQuote, 1, currentIDNum);
            return newQuoteResponse;

        }
        catch (Exception ex)
        {
            throw;
        }
    }

    public async Task<Response> InsertQuote(string newID)
    {
        try
        {
            var insertQuote = $"INSERT INTO lifelogquoteoftheday(Date, LifelogQuote_ID) VALUES(CURRENT_DATE, '{newID}')";
            var insertResponse = await createDataOnlyDAO.CreateData(insertQuote);
            return insertResponse;

        }
        catch (Exception ex)
        {
            throw;
        }
    }

    public async Task<Response> PlaceholderRetrieve()
    {
        try
        {
            //ReadDataOnlyDAO readDataOnlyDAO = new ReadDataOnlyDAO();
            var placeholderRetrieve = $"SELECT Quote, Author FROM LifelogQuote ORDER BY ID DESC";
            var placeholderRetrieveResponse = await readDataOnlyDAO.ReadData(placeholderRetrieve, 1);
            return placeholderRetrieveResponse;

        }
        catch (Exception ex)
        {
            throw;
        }
    }

    public async Task<Response> QuoteChecker(string quote)
    {
        try
        {
            //ReadDataOnlyDAO readDataOnlyDAO = new ReadDataOnlyDAO();
            var quoteChecker = $"SELECT ID FROM LifelogQuote WHERE Quote = \"{quote}\"";
            var quoteCheckerResponse = await readDataOnlyDAO.ReadData(quoteChecker);
            return quoteCheckerResponse;

        }
        catch (Exception ex)
        {
            throw;
        }
    }

    public async Task<Response> AuthorChecker(string author)
    {
        try
        {
            //ReadDataOnlyDAO readDataOnlyDAO = new ReadDataOnlyDAO();
            var authorChecker = $"SELECT ID FROM LifelogQuote WHERE Author = '{author}'";
            var authorCheckerResponse = await readDataOnlyDAO.ReadData(authorChecker);
            return authorCheckerResponse;

        }
        catch (Exception ex)
        {
            throw;
        }
    }

    public async Task<Response> MonthChecker(string currentID)
    {
        try
        {
            //ReadDataOnlyDAO readDataOnlyDAO = new ReadDataOnlyDAO();
            var monthChecker = $"SELECT Date From lifelogquoteoftheday WHERE Date < DATE_SUB(CURRENT_DATE, INTERVAL 180 DAY) AND LifelogQuote_ID = '{currentID}' ORDER BY Date DESC";
            var monthCheckerResponse = await readDataOnlyDAO.ReadData(monthChecker);
            return monthCheckerResponse;

        }
        catch (Exception ex)
        {
            throw;
        }
    }

}
