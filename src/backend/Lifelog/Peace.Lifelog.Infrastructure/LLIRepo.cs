using DomainModels;
using Peace.Lifelog.DataAccess;

namespace Peace.Lifelog.Infrastructure;

public class LLIRepo : ILLIRepo
{
    private ICreateDataOnlyDAO createDataOnlyDAO;
    private IReadDataOnlyDAO readDataOnlyDAO;
    private IUpdateDataOnlyDAO updateDataOnlyDAO;
    private IDeleteDataOnlyDAO deleteDataOnlyDAO;
    public LLIRepo(ICreateDataOnlyDAO createDataOnlyDAO, IReadDataOnlyDAO readDataOnlyDAO, IUpdateDataOnlyDAO updateDataOnlyDAO, IDeleteDataOnlyDAO deleteDataOnlyDAO)
    {
        this.createDataOnlyDAO = createDataOnlyDAO;
        this.readDataOnlyDAO = readDataOnlyDAO;
        this.updateDataOnlyDAO = updateDataOnlyDAO;
        this.deleteDataOnlyDAO = deleteDataOnlyDAO;
    }

    public async Task<Response> CreateLLI(string userHash, LLIDB lli)
    {
        var sql = "INSERT INTO LLI (UserHash, Title, Description, Status, Visibility, Deadline, Cost, RecurrenceStatus, RecurrenceFrequency, CreationDate, Category1, Category2, Category3, CompletionDate) VALUES ("
        + $"\"{userHash}\", "
        + $"\"{lli.Title}\", "
        + $"\"{lli.Description}\", "
        + $"\"{lli.Status}\", "
        + $"\"{lli.Visibility}\", "
        + $"\"{lli.Deadline}\", "
        + $"{lli.Cost}, "
        + $"\"{lli.RecurrenceStatus}\", "
        + $"\"{lli.RecurrenceFrequency}\", "
        + $"\"{DateTime.Today.ToString("yyyy-MM-dd")}\", "
        + $"\"{lli.Category1}\", ";

        if (lli.Category2 != null)
        {
            sql += $"\"{lli.Category2}\", ";
        }
        else
        {
            sql += "null, ";
        }

        if (lli.Category3 != null)
        {
            sql += $"\"{lli.Category3}\", ";
        }
        else
        {
            sql += "null, ";
        }

        if (lli.CompletionDate != "")
        {
            sql += $"\"{DateTime.Today.ToString("yyyy-MM-dd")}\"";
        }
        else
        {
            sql += "null";
        }

        sql += ");";

        return await this.createDataOnlyDAO.CreateData(sql);
    }

    public async Task<Response> ReadAllLLI(string userHash)
    {
        var readLLISql = $"SELECT * FROM LLI WHERE userHash = \"{userHash}\"";

        return await this.readDataOnlyDAO.ReadData(readLLISql, count: null);
    }

    public async Task<Response> ReadLLICompletionStatus(string userHash, string title, string? lliId)
    {
        var completionDateCheckSql = "";

        if (lliId != string.Empty)
        {
            completionDateCheckSql = "SELECT CompletionDate "
            + $"FROM LLI WHERE UserHash=\"{userHash}\" AND Title=\"{title}\" AND LLIId != {lliId}";
        }
        else
        {
            completionDateCheckSql = "SELECT CompletionDate "
            + $"FROM LLI WHERE UserHash=\"{userHash}\" AND Title=\"{title}\"";
        }


        return await this.readDataOnlyDAO.ReadData(completionDateCheckSql);
    }

    public async Task<Response> ReadMostCommonLLICategory()
    {
        var sql = "SELECT "
            + "Category1 AS Most_Common_Category "
        + "FROM "
            + "RecSummary "
        + "GROUP BY "
            + "Category1 "
        + "ORDER BY "
            + "COUNT(*) DESC";

        return await readDataOnlyDAO.ReadData(sql, count: 1);
    }

    public async Task<Response> ReadNumberOfLLI(int period)
    {
        var sql = "SELECT COUNT(LLIId) AS TotalCount "
        + "FROM LLI "
        + $"WHERE CreationDate >= DATE_SUB(CURDATE(), INTERVAL {period} MONTH)";

        return await readDataOnlyDAO.ReadData(sql);
    }

    public async Task<Response> ReadMostExpensiveLLI(int period)
    {
        var sql = "SELECT "
                + "Title AS Title_With_Highest_Cost, "
                + "Cost AS Highest_Cost "
            + "FROM "
                + "LLI "
            + "WHERE "
                + $"CreationDate >= DATE_SUB(NOW(), INTERVAL {period} MONTH) "
            + "ORDER BY "
                + "Cost DESC ";

        return await readDataOnlyDAO.ReadData(sql, count: 1);
    }

    public async Task<Response> UpdateLLIRecurrenceStatus(string userHash)
    {

        var updateLLISql = "UPDATE LLI "
        + "SET Status = 'Active', CompletionDate = NULL "
        + $"WHERE (RecurrenceFrequency = 'Weekly' AND UserHash=\"{userHash}\" AND CompletionDate IS NOT NULL AND DATE_ADD(CompletionDate, INTERVAL 1 WEEK) <= CURDATE()) "
        + $"OR (RecurrenceFrequency = 'Monthly' AND UserHash=\"{userHash}\" AND CompletionDate IS NOT NULL AND DATE_ADD(CompletionDate, INTERVAL 1 MONTH) <= CURDATE()) "
        + $"OR (RecurrenceFrequency = 'Yearly' AND UserHash=\"{userHash}\" AND CompletionDate IS NOT NULL AND DATE_ADD(CompletionDate, INTERVAL 1 YEAR) <= CURDATE())";

        return await this.updateDataOnlyDAO.UpdateData(updateLLISql);
    }

    public async Task<Response> UpdateLLI(string userHash, LLIDB lli)
    {
        if (lli.Status == "Completed")
        {
            lli.CompletionDate = DateTime.Today.ToString("yyyy-MM-dd");
        }

        string updateLLISql = "UPDATE LLI SET "
        + (lli.Title != string.Empty && lli.Title != string.Empty ? $"Title = \"{lli.Title}\"," : "")
        + (lli.Description != null && lli.Description != string.Empty ? $"Description = \"{lli.Description}\"," : "")
        + (lli.Status != null && lli.Status != string.Empty ? $"Status = \"{lli.Status}\"," : "")
        + (lli.Visibility != null && lli.Visibility != string.Empty ? $"Visibility = \"{lli.Visibility}\"," : "")
        + (lli.Deadline != string.Empty && lli.Deadline != string.Empty ? $"Deadline = \"{lli.Deadline}\"," : "")
        + (lli.Cost != null ? $"Cost = {lli.Cost}," : "")
        + (lli.RecurrenceStatus != null && lli.RecurrenceStatus != string.Empty ? $"RecurrenceStatus = \"{lli.RecurrenceStatus}\"," : "")
        + (lli.RecurrenceFrequency != null && lli.RecurrenceFrequency != string.Empty ? $"RecurrenceFrequency = \"{lli.RecurrenceFrequency}\"," : "")
        + (lli.CompletionDate != null && lli.CompletionDate != string.Empty ? $"CompletionDate = \"{lli.CompletionDate}\"," : "")
        + (lli.Category1 != null && lli.Category1 != string.Empty ? $"Category1 = \"{lli.Category1}\"," : "")
        + (lli.Category2 != null && lli.Category2 != string.Empty ? $"Category2 = \"{lli.Category2}\"," : "")
        + (lli.Category3 != null && lli.Category3 != string.Empty ? $"Category3 = \"{lli.Category3}\"," : "");

        updateLLISql = updateLLISql.Remove(updateLLISql.Length - 1);

        updateLLISql += $" WHERE LLIId = \"{lli.LLIID}\";";

        return await this.updateDataOnlyDAO.UpdateData(updateLLISql);
    }

    public async Task<Response> DeleteLLI(string userHash, string lliId)
    {
        var sql = $"DELETE FROM LLI WHERE userHash = \"{userHash}\" AND LLIId = \"{lliId}\";";

        return await this.deleteDataOnlyDAO.DeleteData(sql);
    }

}
