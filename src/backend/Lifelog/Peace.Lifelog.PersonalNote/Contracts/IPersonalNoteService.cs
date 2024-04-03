using DomainModels;

namespace Peace.Lifelog.PersonalNote;

public interface IPersonalNoteService
{
    public Task<Response> CreatePersonalNote(string userHash, PN personalnote);
    public Task<Response> DeletePersonalNote(string userHash, string noteId);
    public Task<Response> ViewPersonalNote(string userHash, PN personalnote);
    public Task<Response> UpdatePersonalNote(string userHash, PN personalnote);
    public Task<Response> GetAllPersonalNotesFromUser(string userHash);
}
