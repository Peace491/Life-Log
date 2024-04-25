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
