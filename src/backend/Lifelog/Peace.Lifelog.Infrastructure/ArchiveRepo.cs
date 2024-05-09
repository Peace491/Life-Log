﻿namespace Peace.Lifelog.Infrastructure;
using DomainModels;
using Peace.Lifelog.DataAccess;

public class ArchiveRepo
{
    private IReadDataOnlyDAO readDataOnlyDAO;
    private IDeleteDataOnlyDAO deleteDataOnlyDAO;

    public ArchiveRepo (IReadDataOnlyDAO readDataOnlyDAO, IDeleteDataOnlyDAO deleteDataOnlyDAO)
    {
        this.readDataOnlyDAO = readDataOnlyDAO;
        this.deleteDataOnlyDAO = deleteDataOnlyDAO;
    }

    public async Task<Response> SelectArchivableLogs(string tableName)
    {
        var selectLogs = $"SELECT * FROM {tableName} WHERE Timestamp < DATE_SUB(CURRENT_DATE, INTERVAL 30 DAY)";
        var selectLogsResponse = await readDataOnlyDAO.ReadData(selectLogs, null);
        return selectLogsResponse;
    }

    public async Task<Response> DeleteArchivedLogs(string tableName)
    {
        var deleteLogs = $"DELETE FROM {tableName} WHERE Timestamp < DATE_SUB(CURRENT_DATE, INTERVAL 30 DAY)";
        var deleteLogsResponse = await deleteDataOnlyDAO.DeleteData(deleteLogs);
        return deleteLogsResponse;
    }
}