using System.Net.NetworkInformation;
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
        checkUserHashResponse checkTodayDateResponse = new Response();
        var checkUserHash = $"SELECT UserHash FROM LifelogReminder WHERE UserHash = '{userHash}'";
        var checkUserHashResponse = await readDataOnlyDAO.ReadData(checkUserHash);
        return checkUserHashResponse;
    }
    public async Task<Response> AddUserHashAndDate(string userHash)
    {   
        checkUserHashResponse checkTodayDateResponse = new Response();
        var writeUserHashAndDate = $"INSERT INTO LifelogReminder(Date) VALUES(DATE_SUB(CURRENT_DATE(), INTERVAL 31 DAY)) WHERE UserHash = '{userHash}'";
        writeUserHashAndDateResponse = await createDataOnlyDAO.CreateData(writeUserHashAndDate);
        return writeUserHashAndDateResponse;
    }
    public async Task<Response> UpdateCurrentDate(string userHash)
    {
        var updateDate = $"UPDATE LifelogReminder SET Date = CURRENT_DATE WHERE UserHash = '{userHash}'"
        var updateDateResponse = await updateDataOnlyDAO.UpdateData(writeDate);
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
        Response response = new Response();
        var checkTodayDate = $"SELECT Date From LifelogReminder WHERE Date < DATE_SUB(CURRENT_DATE, INTERVAL '{days}' DAY) AND UserHash = '{userHash}'";
        var checkTodayDateResponse = await readDataOnlyDAO.ReadData(checkTodayDate);
        return checkTodayDateResponse;
    }

    public async Task<Response> GetContentAndFrequency(string userHash)
    {
        Response response = new Response();
        var checkTodayDate = $"SELECT Content, Frequency From LifelogReminder WHERE UserHash = '{userHash}'";
        var checkTodayDateResponse = await readDataOnlyDAO.ReadData(checkTodayDate);
        return checkTodayDateResponse;
    }
    public Task<Response> GetUserID(string userHash)
    {
        Response response = new Response();
        string selectEmailSql = $"SELECT UserId FROM LifelogAccount WHERE UserHash = \"{userHash}\""; 
        var selectEmailSqlResponse = await readDataOnlyDao.ReadData(selectEmailSql);
        return selectEmailSqlResponse;
    }
}