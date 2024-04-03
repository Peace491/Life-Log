using DomainModels;
using Peace.Lifelog.DataAccess;

namespace Peace.Lifelog.Infrastructure;

public class PersonalNoteRepo : IPersonalNoteRepo
{
    private ICreateDataOnlyDAO createDataOnlyDAO;
    private IReadDataOnlyDAO readDataOnlyDAO;
    private IUpdateDataOnlyDAO updateDataOnlyDAO;
    private IDeleteDataOnlyDAO deleteDataOnlyDAO;

    public PersonalNoteRepo(ICreateDataOnlyDAO createDataOnlyDAO, IReadDataOnlyDAO readDataOnlyDAO, IUpdateDataOnlyDAO updateDataOnlyDAO, IDeleteDataOnlyDAO deleteDataOnlyDAO)
    {
        this.createDataOnlyDAO = createDataOnlyDAO;
        this.readDataOnlyDAO = readDataOnlyDAO;
        this.updateDataOnlyDAO = updateDataOnlyDAO;
        this.deleteDataOnlyDAO = deleteDataOnlyDAO;
    }

    public async Task<Response> CreatePersonalNoteInDB(string userHash, string noteContent, string noteDate)
    {
        var createPersonalNoteResponse = new Response();

        string sql = "INSERT INTO PersonalNote (UserHash, NoteContent, NoteDate) VALUES ("
        + $"\"{userHash}\", "
        + $"\"{noteContent}\", "
        + $"\"{noteDate}\");";

        try
        {
            createPersonalNoteResponse = await this.createDataOnlyDAO.CreateData(sql);

            if (createPersonalNoteResponse.HasError)
            {
                throw new Exception(createPersonalNoteResponse.ErrorMessage);
            }
        }
        catch (Exception error)
        {
            createPersonalNoteResponse.HasError = true;
            createPersonalNoteResponse.ErrorMessage = error.Message;
            createPersonalNoteResponse.Output = null;
        }

        return createPersonalNoteResponse;
    }

    public async Task<Response> ReadPersonalNoteInDB(string userHash, string noteDate)
    {
        var readResponse = new Response();

        string sql = $"SELECT * FROM PersonalNote WHERE UserHash = \"{userHash}\" AND NoteDate = \"{noteDate}\";";

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

    public async Task<Response> ReadAllPersonalNoteInDB(string userHash)
    {
        var readResponse = new Response();

        string sql = $"SELECT * FROM PersonalNote WHERE userHash = \"{userHash}\"";

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

    public async Task<Response> UpdatePersonalNoteInDB(string noteContent, string noteDate, string noteId)
    {
        var updatePersonalNoteResponse = new Response();

        #region Creating the sql statement

        string sql = "UPDATE PersonalNote SET "
        + (noteContent != string.Empty ? $"NoteContent = \"{noteContent}\"," : "")
        + (noteDate != null && noteDate != string.Empty ? $"NoteDate = \"{noteDate}\"," : "");

        sql = sql.Remove(sql.Length - 1);

        sql += $" WHERE NoteId = \"{noteId}\";";
        #endregion

        try
        {
            updatePersonalNoteResponse = await updateDataOnlyDAO.UpdateData(sql);
        }
        catch (Exception error)
        {
            updatePersonalNoteResponse.HasError = true;
            updatePersonalNoteResponse.ErrorMessage = error.Message;
            updatePersonalNoteResponse.Output = null;
        }

        return updatePersonalNoteResponse;
    }

    public async Task<Response> DeletePersonalNoteInDB(string userHash, string noteId)
    {
        var deleteResponse = new Response();

        #region Creating the sql statement

        string sql = $"DELETE FROM PersonalNote WHERE userHash = \"{userHash}\" AND NoteId = \"{noteId}\";";
        #endregion

        try
        {
            deleteResponse = await deleteDataOnlyDAO.DeleteData(sql); ;
        }
        catch (Exception error)
        {
            deleteResponse.HasError = true;
            deleteResponse.ErrorMessage = error.Message;
            deleteResponse.Output = null;
        }

        return deleteResponse;
    }
}
