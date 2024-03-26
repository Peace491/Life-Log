using DomainModels;
using Peace.Lifelog.DataAccess;
using System.Diagnostics;

namespace Peace.Lifelog.PersonalNote;

public class PersonalNoteService : ICreatePersonalNote
{
    private static int ERROR_TIME_LIMIT_IN_SECOND = 3;
    private static int MAX_NOTE_LENGTH_IN_CHAR = 1200;
    private static int EARLIEST_NOTE_YEAR = 1960;
    private static int LATEST_NOTE_YEAR = DateTime.Today.Year;
    private CreateDataOnlyDAO createDataOnlyDAO;
    private ReadDataOnlyDAO readDataOnlyDAO;
    private UpdateDataOnlyDAO updateDataOnlyDAO;
    private DeleteDataOnlyDAO deleteDataOnlyDAO;
    private Logging.Logging logging;

    public PersonalNoteService(CreateDataOnlyDAO createDataOnlyDAO, ReadDataOnlyDAO readDataOnlyDAO, UpdateDataOnlyDAO updateDataOnlyDAO, DeleteDataOnlyDAO deleteDataOnlyDAO, Logging.Logging logging)
    {
        this.createDataOnlyDAO = createDataOnlyDAO;
        this.readDataOnlyDAO = readDataOnlyDAO;
        this.updateDataOnlyDAO = updateDataOnlyDAO;
        this.deleteDataOnlyDAO = deleteDataOnlyDAO;
        this.logging = logging;
    }

    // Create Note
    public async Task<Response> CreatePersonalNote(string userHash, PN personalnote)
    {
        var createPersonalNoteResponse = new Response();
        var timer = new Stopwatch();
        timer.Start();

        // Validate Input 
        var validateCreation = await checkIfPersonalNoteInputIsValid(userHash, personalnote);

        if (validateCreation.HasError)
        {
            return validateCreation;
        }

        // Populate Data Base
        #region Create Personal Note in DB

        var sql = "INSERT INTO PersonalNote (UserHash, NoteContent, NoteDate) VALUES ("
        + $"\"{userHash}\", "
        + $"\"{personalnote.NoteContent}\", "
        + $"\"{personalnote.NoteDate}\");";

        createPersonalNoteResponse = await this.createDataOnlyDAO.CreateData(sql);
        timer.Stop();


        #endregion

        // Log
        string message = "The Note has successfully been Created";

        CreateLog(userHash, message, timer);


        return createPersonalNoteResponse;
    }

    // Delete Note
    public async Task<Response> DeletePersonalNote(string userHash, PN personalnote)
    {
        var deletePersonalNoteResponse = new Response();
        var timer = new Stopwatch();
        timer.Start();

        // Validate Input 

        if (personalnote.NoteId == string.Empty || personalnote.NoteId is null)
        {
            deletePersonalNoteResponse.HasError = true;
            deletePersonalNoteResponse.ErrorMessage = "NoteId can not be empty";
            return deletePersonalNoteResponse;
        }

        // Modify Data Base
        #region Delete Personal Note in DB

        var sql = $"DELETE FROM PersonalNote WHERE userHash = \"{userHash}\" AND NoteId = \"{personalnote.NoteId}\";";

        var deleteResponse = await this.deleteDataOnlyDAO.DeleteData(sql);
        timer.Stop();

        if (deleteResponse.Output != null)
        {
            foreach (int rowsAffected in deleteResponse.Output)
            {
                if (rowsAffected == 0)
                {
                    deletePersonalNoteResponse.HasError = true;
                    deletePersonalNoteResponse.ErrorMessage = "Failed to delete Personal Note";
                    return deletePersonalNoteResponse;
                }
            }
        }


        #endregion

        // Log
        string message = "The Note is successfully deleted";

        CreateLog(userHash, message, timer);

        deletePersonalNoteResponse = deleteResponse;

        return deletePersonalNoteResponse;
    }

    // View Note
    public async Task<Response> ViewPersonalNote(string userHash, PN personalnote)
    {
        var viewPersonalNoteResponse = new Response();

        var timer = new Stopwatch();
        timer.Start();

        // Validate Input 
        var validateViewing = await checkIfPersonalNoteInputIsValid(userHash, personalnote);

        if (validateViewing.HasError)
        {
            return validateViewing;
        }

        // Modify Data Base
        #region Retrive Personal Note from DB

        var sql = $"SELECT * FROM PersonalNote WHERE UserHash = \"{personalnote.UserHash}\" AND NoteDate = \"{personalnote.NoteDate}\";";

        var readPersonalNoteResponse = await this.readDataOnlyDAO.ReadData(sql);
        timer.Stop();


        #endregion

        // Log
        string message = "The Note is successfully fetched";

        CreateLog(userHash, message, timer);

        var personalNoteOutput = ConvertDatabaseResponseOutputToPersonalNoteObjectList(readPersonalNoteResponse);

        viewPersonalNoteResponse.Output = personalNoteOutput;

        viewPersonalNoteResponse.HasError = false;

        return viewPersonalNoteResponse;
    }

    // Edit Note
    public async Task<Response> UpdatePersonalNote(string userHash, PN personalnote)
    {
        var updatePersonalNoteResponse = new Response();

        var timer = new Stopwatch();
        timer.Start();

        // Validate Input 
        var validateupdate = await checkIfPersonalNoteInputIsValid(userHash, personalnote);

        if (validateupdate.HasError)
        {
            return validateupdate;
        }

        // Modify Data Base
        #region Update Personal Note in DB

        var checkNoteSql = $"SELECT * FROM PersonalNote WHERE NoteDate = \"{personalnote.NoteDate}\"";
        updatePersonalNoteResponse = await this.readDataOnlyDAO.ReadData(checkNoteSql);


        string updateNoteSql = "UPDATE PersonalNote SET "
        + (personalnote.NoteContent != string.Empty ? $"NoteContent = \"{personalnote.NoteContent}\"," : "")
        + (personalnote.NoteDate != null && personalnote.NoteDate != string.Empty ? $"NoteDate = \"{personalnote.NoteDate}\"," : "");

        updateNoteSql = updateNoteSql.Remove(updateNoteSql.Length - 1);

        updateNoteSql += $" WHERE NoteId = \"{personalnote.NoteId}\";";

        updatePersonalNoteResponse = await this.updateDataOnlyDAO.UpdateData(updateNoteSql);

        if (updatePersonalNoteResponse.HasError)
        {
            updatePersonalNoteResponse.ErrorMessage = "Personal Note fields are invalid";

            var errorMessage = updatePersonalNoteResponse.ErrorMessage;
            var logResponse = this.logging.CreateLog("Logs", userHash, "ERROR", "Persistent Data Store", errorMessage);
        }
        timer.Stop();


        #endregion

        // Log
        string message = "The Note is successfully updated";

        CreateLog(userHash, message, timer);

        updatePersonalNoteResponse.HasError = false;
        return updatePersonalNoteResponse;
    }






    #region Helper Functions

    // Validate the Personal Note Information
    private async Task<Response> checkIfPersonalNoteInputIsValid(string userHash, PN personalnote)
    {
        var validationResponse = new Response();

        if (userHash == string.Empty)
        {
            validationResponse.HasError = true;
            validationResponse.ErrorMessage = "User Hash must not be empty";
            var errorMessage = "The Personal Note User Hash is invalid";
            var logResponse = this.logging.CreateLog("Logs", userHash, "Warning", "Persistent Data Store", errorMessage);
            return validationResponse;
        }

        if (personalnote.NoteDate == null || personalnote.NoteContent == null)
        {
            validationResponse.HasError = true;
            validationResponse.ErrorMessage = "The non-nullable Personal Note input is null";
            var errorMessage = "The non-nullable Personal Note input is null";
            var logResponse = this.logging.CreateLog("Logs", userHash, "Warning", "Persistent Data Store", errorMessage);
            return validationResponse;
        }

        if (personalnote.NoteContent == null || personalnote.NoteContent.Length > MAX_NOTE_LENGTH_IN_CHAR)
        {
            validationResponse.HasError = true;
            validationResponse.ErrorMessage = "The Personal Note Invalid";
            var errorMessage = "The personal note contents is invalid";
            var logResponse = this.logging.CreateLog("Logs", userHash, "Warning", "Persistent Data Store", errorMessage);
            return validationResponse;
        }

        if (DateTime.Parse(personalnote.NoteDate) > DateTime.Today)
        {
            validationResponse.HasError = true;
            validationResponse.ErrorMessage = "The Date is Invalid";
            var errorMessage = "The Date is invalid";
            var logResponse = this.logging.CreateLog("Logs", userHash, "Warning", "Persistent Data Store", errorMessage);
            return validationResponse;
        }

        if (personalnote.NoteDate != string.Empty)
        {

            var personalNoteYear = DateTime.Parse(personalnote.NoteDate).Year;

            if (personalNoteYear < EARLIEST_NOTE_YEAR || personalNoteYear > LATEST_NOTE_YEAR)
            {
                validationResponse.HasError = true;
                validationResponse.ErrorMessage = "LLI Deadline is out of range";
                var errorMessage = "The LLI deadline is invalid";
                var logResponse = this.logging.CreateLog("Logs", userHash, "Warning", "Persistent Data Store", errorMessage);
                return validationResponse;
            }
        }

        validationResponse.HasError = false;
        return validationResponse;
    }

    // Create Log
    public void CreateLog(string userHash, string message, Stopwatch timer)
    {
        var logResponse = this.logging.CreateLog("Logs", userHash, "Info", "Persistent Data Store", message);

        if (timer.Elapsed.TotalSeconds > ERROR_TIME_LIMIT_IN_SECOND)
        {
            var errorMessage = "Operation took too long";
            var errorLogResponse = this.logging.CreateLog("Logs", userHash, "Warning", "Persistent Data Store", errorMessage);
        }
    }


    // convert to PN object 
    private List<Object>? ConvertDatabaseResponseOutputToPersonalNoteObjectList(Response readPersonalNoteResponse)
    {
        List<Object> personalNoteList = new List<Object>();

        if (readPersonalNoteResponse.Output == null)
        {
            return null;
        }


        foreach (List<Object> PersonalNote in readPersonalNoteResponse.Output)
        {

            var personalNote = new PN();

            int index = 0;

            foreach (var attribute in PersonalNote)
            {
                if (attribute is null) continue;

                switch (index)
                {
                    case 0:
                        personalNote.NoteId = attribute.ToString() ?? "";
                        break;
                    case 1:
                        personalNote.UserHash = attribute.ToString() ?? "";
                        break;
                    case 2:
                        personalNote.NoteDate = attribute.ToString() ?? "";
                        break;
                    case 3:
                        personalNote.NoteContent = attribute.ToString() ?? "";
                        break;
                    default:
                        break;
                }
                index++;

            }

            personalNoteList.Add(personalNote);

        }

        return personalNoteList;
    }














    #endregion
}