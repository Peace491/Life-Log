using DomainModels;
using Peace.Lifelog.DataAccess;

namespace Peace.Lifelog.Infrastructure;

public class LifelogReminderRepo : ILifelogReminderRepo
{
    private ICreateDataOnlyDAO createDataOnlyDAO;
    private IReadDataOnlyDAO readDataOnlyDAO;
    private IUpdateDataOnlyDAO updateDataOnlyDAO;
    private IDeleteDataOnlyDAO deleteDataOnlyDAO;

    public LifelogReminderRepo(ICreateDataOnlyDAO createDataOnlyDAO, IReadDataOnlyDAO readDataOnlyDAO, IUpdateDataOnlyDAO updateDataOnlyDAO, IDeleteDataOnlyDAO deleteDataOnlyDAO)
    {
        this.createDataOnlyDAO = createDataOnlyDAO;
        this.readDataOnlyDAO = readDataOnlyDAO;
        this.updateDataOnlyDAO = updateDataOnlyDAO;
        this.deleteDataOnlyDAO = deleteDataOnlyDAO;
    }

    public async Task<Response> CheckIfUserHashInDB(string userHash)
    {
        var checkUserHash = $"SELECT UserHash FROM LifelogReminder WHERE UserHash = '{userHash}'";
        var checkUserHashResponse = await readDataOnlyDAO.ReadData(checkUserHash);
        return checkUserHashResponse;
    }
    public async Task<Response> AddUserHashAndDate(string userHash)
    {
        var writeUserHashAndDate = $"INSERT INTO LifelogReminder(UserHash, Content, Frequency, Date) VALUES('{userHash}', 'Active', 'Weekly', DATE_SUB(CURRENT_DATE(), INTERVAL 32 DAY))";
        var writeUserHashAndDateResponse = await createDataOnlyDAO.CreateData(writeUserHashAndDate);
        return writeUserHashAndDateResponse;
    }
    public async Task<Response> UpdateCurrentDate(string userHash)
    {
        var updateDate = $"UPDATE LifelogReminder SET Date = CURRENT_DATE WHERE UserHash = '{userHash}'";
        var updateDateResponse = await updateDataOnlyDAO.UpdateData(updateDate);
        return updateDateResponse;
    }
    public async Task<Response> UpdateContentAndFrequency(string userHash, string content, string frequency)
    {
        var updateTable = $"UPDATE LifelogReminder SET Content = '{content}', Frequency = '{frequency}' WHERE UserHash = '{userHash}'";
        var updateTableResponse = await updateDataOnlyDAO.UpdateData(updateTable);
        return updateTableResponse;
    }
    public async Task<Response> CheckCurrentReminder(string userHash, int days)
    {
        var checkTodayDate = $"SELECT Date From LifelogReminder WHERE Date > DATE_SUB(CURRENT_DATE, INTERVAL '{days}' DAY) AND UserHash = '{userHash}'";
        var checkTodayDateResponse = await readDataOnlyDAO.ReadData(checkTodayDate);
        return checkTodayDateResponse;
    }

    public async Task<Response> GetContentAndFrequency(string userHash)
    {
        var checkTodayDate = $"SELECT Content, Frequency From LifelogReminder WHERE UserHash = '{userHash}'";
        var checkTodayDateResponse = await readDataOnlyDAO.ReadData(checkTodayDate);
        return checkTodayDateResponse;
    }
    public async Task<Response> GetUserID(string userHash)
    {
        string selectEmailSql = $"SELECT UserId FROM LifelogAccount WHERE UserHash = \"{userHash}\"";
        var selectEmailSqlResponse = await readDataOnlyDAO.ReadData(selectEmailSql);
        return selectEmailSqlResponse;
    }
}