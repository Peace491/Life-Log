using System.Net.NetworkInformation;
using DomainModels;
using Peace.Lifelog.DataAccess;

namespace Peace.Lifelog.Infrastructure;

public class LocationRecommendationRepo : ILocationRecommendationRepo
{
    private ICreateDataOnlyDAO createDataOnlyDAO;
    private IReadDataOnlyDAO readDataOnlyDAO;
    private IUpdateDataOnlyDAO updateDataOnlyDAO;
    private IDeleteDataOnlyDAO deleteDataOnlyDAO;

    public LocationRecommendationRepo(ICreateDataOnlyDAO createDataOnlyDAO, IReadDataOnlyDAO readDataOnlyDAO, IUpdateDataOnlyDAO updateDataOnlyDAO, IDeleteDataOnlyDAO deleteDataOnlyDAO)
    {
        this.createDataOnlyDAO = createDataOnlyDAO;
        this.readDataOnlyDAO = readDataOnlyDAO;
        this.updateDataOnlyDAO = updateDataOnlyDAO;
        this.deleteDataOnlyDAO = deleteDataOnlyDAO;
    }

    public async Task<Response> ReadAllUserPinInDB(string UserHash)
    {
        var readResponse = new Response();

        string sql = $"SELECT Latitude, Longitude FROM mappin Where UserHash=\"{UserHash}\"";

        try
        {
            readResponse = await readDataOnlyDAO.ReadData(sql);
        }
        catch (Exception error)
        {
            readResponse.HasError = true;
            readResponse.ErrorMessage = error.Message;
            readResponse.Output = null;
        }

        return readResponse;
    }
    public async Task<Response> ReadPinInDB(string PinId)
    {
        var readResponse = new Response();

        string sql = "SELECT LLIId "
        + $"FROM MapPin Where PinId=\"{PinId}\";";

        try
        {
            readResponse = await readDataOnlyDAO.ReadData(sql);
        }
        catch (Exception error)
        {
            readResponse.HasError = true;
            readResponse.ErrorMessage = error.Message;
            readResponse.Output = null;
        }

        return readResponse;
    }

    public async Task<Response> ReadLLIInDB(string LLIId)
    {
        var readResponse = new Response();

        string sql = $"SELECT LLIId, UserHash, Title, Description, Status, Visibility, Deadline, Cost, RecurrenceStatus, RecurrenceFrequency, CreationDate, CompletionDate, Category1, Category2, Category3" +
            $" FROM LLI WHERE LLIId=\"{LLIId}\";";

        try
        {
            readResponse = await readDataOnlyDAO.ReadData(sql);
        }
        catch (Exception error)
        {
            readResponse.HasError = true;
            readResponse.ErrorMessage = error.Message;
            readResponse.Output = null;
        }

        return readResponse;

    }

    public async Task<Response> ReadAllPinForLLIInDB(string LLIId)
    {
        var readResponse = new Response();

        string sql = $"SELECT LLIId, COUNT(*) AS count_of_pins FROM MapPin WHERE LLIId = '{LLIId}' GROUP BY LLIId;";

        try
        {
            readResponse = await readDataOnlyDAO.ReadData(sql);
        }
        catch (Exception error)
        {
            readResponse.HasError = true;
            readResponse.ErrorMessage = error.Message;
            readResponse.Output = null;
        }

        return readResponse;
    }
    public async Task<Response> GetPinId(object lat, object lng)
    {
        var readResponse = new Response();
        string sql = $"SELECT PinId FROM MapPin WHERE Latitude = '{lat}' AND Longitude = '{lng}'";

        try
        {
            readResponse = await readDataOnlyDAO.ReadData(sql);
        }
        catch (Exception error)
        {
            readResponse.HasError = true;
            readResponse.ErrorMessage = error.Message;
            readResponse.Output = null;
        }
        //object pin = new object();
        return readResponse;
    }
}
