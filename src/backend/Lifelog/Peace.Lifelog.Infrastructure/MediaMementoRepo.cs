using Peace.Lifelog.DataAccess;
using Peace.Lifelog.Logging;
using DomainModels;

namespace Peace.Lifelog.Infrastructure;

public class MediaMementoRepo : IMediaMementoRepo
{
    private readonly IUpdateDataOnlyDAO _updateDataOnlyDAO;
    private readonly IReadDataOnlyDAO _readDataOnlyDAO;

    // TODO : update and delete sql queries
    private string uploadQuery = "UPDATE LLI SET MediaMemento = @binary WHERE LLIId = @lliId;";
    private string deleteQuery = "UPDATE LLI SET MediaMemento = NULL WHERE LLIId = @lliId;";
    private string getAllUserImagesQuery = "SELECT MediaMemento FROM LLI WHERE UserHash = @userhash  AND MediaMemento IS NOT NULL;";

    public MediaMementoRepo(IUpdateDataOnlyDAO updateDataOnlyDAO, IReadDataOnlyDAO readDataOnlyDAO)
    {
        _updateDataOnlyDAO = updateDataOnlyDAO;
        _readDataOnlyDAO = readDataOnlyDAO;
    }


    public async Task<Response> UploadMediaMemento(int lliId, byte[] binary)
    {
        // Convert the byte array to a hexadecimal string
        var hexString = "0x" + BitConverter.ToString(binary).Replace("-", "");

        // Substitute in values for the query 
        var updatedUploadQuery = uploadQuery.Replace("@binary", hexString).Replace("@lliId", lliId.ToString());

        // Run and return the query results
        return await _updateDataOnlyDAO.UpdateData(updatedUploadQuery);
    }


    public async Task<Response> DeleteMediaMemento(int lliId)
    {
        // Subsitute in values for the query
        var updatedDeleteQuery = deleteQuery.Replace("@lliId", lliId.ToString());
        // Run and return Query results 
        return await _updateDataOnlyDAO.UpdateData(updatedDeleteQuery);
    }
    public async Task<Response> GetAllUserImages(string userhash)
    {
        var updatedGetAllUserImagesQuery = $"SELECT MediaMemento FROM LLI WHERE UserHash = '{userhash}' AND MediaMemento IS NOT NULL;";

        return await _readDataOnlyDAO.ReadData(updatedGetAllUserImagesQuery, null);
    }
}
