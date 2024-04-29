using DomainModels;

namespace Peace.Lifelog.Infrastructure;

public interface ILifelogReminderRepo
{
    public Task<Response> CheckIfUserHashInDB(string userHash)
    public Task<Response> AddUserHashAndDate(string userHash);
    public Task<Response> UpdateCurrentDate(string userHash);
    public Task<Response> UpdateContentAndFrequency(string userHash, string content, string frequency);
    public Task<Response> CheckCurrentReminder(string userHash, int days);
    public Task<Response> GetContentAndFrequency(string userHash);
    public Task<Response> GetUserID(string userHash);   
}
