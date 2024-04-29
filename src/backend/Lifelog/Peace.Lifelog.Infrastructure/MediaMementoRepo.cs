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
    private string blukUploadQuery = "UPDATE LLI SET MediaMemento = @binary WHERE Title = @title;";
    // private string getAllUserImagesQuery = "SELECT MediaMemento FROM LLI WHERE UserHash = @userhash  AND MediaMemento IS NOT NULL;";

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

    public async Task<Response> UploadMediaMementosFromCSV(List<List<string>> csvContent)
    {
        bool isFirstLine = true; // Flag to skip the first line (header)

        try
        {
            Console.WriteLine("Trying upload");
            Console.WriteLine("csvContent: " + csvContent.Count);
            foreach (var row in csvContent)
            {
                Console.WriteLine("For loop");
                if (isFirstLine)
                {
                    isFirstLine = false; // Skip the header row
                    continue;
                }

                // Make sure each row has at least two columns
                if (row.Count >= 2)
                {
                    string title = row[0].Trim();
                    string binaryData = row[1].Trim();

                    byte [] temp = Convert.FromBase64String(binaryData);
                    var hexString = "0x" + BitConverter.ToString(temp).Replace("-", "");

                    // Format the SQL query directly with string interpolation
                    string myquery = $"UPDATE LLI SET MediaMemento = {hexString} WHERE Title = '{title}';";

                    // Execute the query
                    var res = await _updateDataOnlyDAO.UpdateData(myquery);
                    Console.WriteLine("res: " + res.HasError);
                }
            }

            return new Response { HasError = false, ErrorMessage = null };
        }
        catch (Exception ex)
        {
            Console.WriteLine("upload fails");
            return new Response { HasError = true, ErrorMessage = ex.Message };
        }
    }
}
