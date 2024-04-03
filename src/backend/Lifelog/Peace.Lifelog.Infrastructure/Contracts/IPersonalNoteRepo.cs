using DomainModels;

namespace Peace.Lifelog.Infrastructure;

public interface IPersonalNoteRepo
{
    public Task<Response> CreatePersonalNoteInDB(string userHash, string noteContent, string noteDate);
    public Task<Response> ReadPersonalNoteInDB(string userHash, string noteDate);
    public Task<Response> UpdatePersonalNoteInDB(string noteContent, string noteDate, string noteId);
    public Task<Response> DeletePersonalNoteInDB(string userHash, string noteId);
    public Task<Response> ReadAllPersonalNoteInDB(string userHash);
}
