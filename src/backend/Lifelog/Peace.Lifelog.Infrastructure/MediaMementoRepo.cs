using Peace.Lifelog.DataAccess;
using Peace.Lifelog.Logging;
using DomainModels;

namespace Peace.Lifelog.Infrastructure;

public class MediaMementoRepo : IMediaMementoRepo
{
    private readonly IUpdateDataOnlyDAO _updateDataOnlyDAO;


    // TODO : update and delete sql queries
    private string uploadQuery = "UPDATE LLI SET MediaMemento = @binary WHERE LLIId = @lliId;";
    private string deleteQuery = "UPDATE LLI SET MediaMemento = NULL WHERE LLIId = @lliId;";

    public MediaMementoRepo(IUpdateDataOnlyDAO updateDataOnlyDAO)
    {
        _updateDataOnlyDAO = updateDataOnlyDAO;
    }


    public async Task<Response> UploadMediaMemento(int lliId, string binary)
    {
        // Subsitute in values for the query 
        var updatedUploadQuery = uploadQuery.Replace("@lliId", lliId.ToString()).Replace("@binary", binary);
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
}
