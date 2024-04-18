using DomainModels;

namespace Peace.Lifelog.Infrastructure;

public interface IMapRepo
{
    public Task<Response> CreatePinInDB(string LLIId, string Address, double Latitude, double Longitude);
    public Task<Response> ReadPinInDB(string PinId);
    public Task<Response> UpdatePinInDB(string PinId, string Address, double Latitude, double Longitude);
    public Task<Response> DeletePinInDB(string PinId);
    public Task<Response> ReadLLIInDB(string LLIId);
    public Task<Response> ReadAllUserPinInDB(string UserHash);
    public Task<Response> ReadAllPinForLLIInDB(string LLIId);
}
