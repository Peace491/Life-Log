using DomainModels;
using Peace.Lifelog.DataAccess;

namespace Peace.Lifelog.Infrastructure;

public class MapRepo : IMapRepo
{
    private ICreateDataOnlyDAO createDataOnlyDAO;
    private IReadDataOnlyDAO readDataOnlyDAO;
    private IUpdateDataOnlyDAO updateDataOnlyDAO;
    private IDeleteDataOnlyDAO deleteDataOnlyDAO;

    public MapRepo(ICreateDataOnlyDAO createDataOnlyDAO, IReadDataOnlyDAO readDataOnlyDAO, IUpdateDataOnlyDAO updateDataOnlyDAO, IDeleteDataOnlyDAO deleteDataOnlyDAO)
    {
        this.createDataOnlyDAO = createDataOnlyDAO;
        this.readDataOnlyDAO = readDataOnlyDAO;
        this.updateDataOnlyDAO = updateDataOnlyDAO;
        this.deleteDataOnlyDAO = deleteDataOnlyDAO;
    }

    public async Task<Response> CreatePinInDB(string LLIId, string UserHash, string Address, double Latitude, double Longitude)
    {
        var createPinResponse = new Response();

        string sql = "INSERT INTO MapPin "
        + "(LLIId, UserHash, Address, Latitude, Longitude) "
        + "VALUES "
        + "("
        + $"\"{LLIId}\", "
        + $"\"{UserHash}\", "
        + $"\"{Address}\", "
        + $"{Latitude}, "
        + $"{Longitude}"
        + ");";

        Console.WriteLine(sql);
        try
        {
            createPinResponse = await this.createDataOnlyDAO.CreateData(sql);

            if (createPinResponse.HasError)
            {
                throw new Exception(createPinResponse.ErrorMessage);
            }
        }
        catch (Exception error)
        {
            createPinResponse.HasError = true;
            createPinResponse.ErrorMessage = error.Message;
            createPinResponse.Output = null;
        }

        return createPinResponse;
    }

    public async Task<Response> ReadAllUserPinInDB(string UserHash)
    {
        var readResponse = new Response();

        string sql = "SELECT *"
        + $"FROM MapPin Where UserHash=\"{UserHash}\"";

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
        Console.WriteLine(sql);

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

    public async Task<Response> UpdatePinInDB(string PinId, string Address, double Latitude, double Longitude)
    {
        var updatePinResponse = new Response();

        string sql = "UPDATE MapPin SET ";

        string parametersAndValues = "";

        if (Address != string.Empty)
        {
            parametersAndValues += $"Address = {Address}, ";
        }
        if (Latitude != 0)
        {
            parametersAndValues += $"Latitude = {Latitude}, ";
        }
        if (Longitude != 0)
        {
            parametersAndValues += $"Longitude = {Longitude}, ";
        }

        // Removing the trailing comma and space
        parametersAndValues = parametersAndValues.TrimEnd(' ', ',');

        sql += parametersAndValues + $" WHERE PinId=\"{PinId}\"";

        try
        {
            updatePinResponse = await updateDataOnlyDAO.UpdateData(sql);
        }
        catch (Exception error)
        {
            updatePinResponse.HasError = true;
            updatePinResponse.ErrorMessage = error.Message;
            updatePinResponse.Output = null;
        }

        return updatePinResponse;

    }

    public async Task<Response> DeletePinInDB(string PinId)
    {
        var deletePinResponse = new Response();
        string sql = $"DELETE FROM MapPin WHERE PinId = \"{PinId}\";";

        try
        {
            deletePinResponse = await deleteDataOnlyDAO.DeleteData(sql); ;
        }
        catch (Exception error)
        {
            deletePinResponse.HasError = true;
            deletePinResponse.ErrorMessage = error.Message;
            deletePinResponse.Output = null;
        }

        return deletePinResponse;


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
}
