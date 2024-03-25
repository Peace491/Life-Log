namespace Peace.Lifelog.PersonalNote;

using DomainModels;

public interface ICreatePersonalNote
{
    Task<Response> CreatePersonalNote(string userHash, PN personalnote);
}
