using DomainModels;

namespace Peace.Lifelog.Map;

public interface IPinService
{
    public Task<Response> CreatePin(
        CreatePinRequest createPinRequest
    );
    public Task<Response> UpdatePin(
        UpdatePinRequest updatePinRequest
    );
    public Task<Response> DeletePin(string pinId, string userHash);
    public Task<Response> ViewPin(
        ViewPinRequest viewPinRequest
    );
    public Task<Response> FetchPinStatus(string LLIId, string userHash);
    public Task<Response> GetAllPinFromUser(string userHash);
    public Task<Response> updateLog(
        UpdateLogRequest updateLogRequest
    );
}
