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
        string message = "The Note is successfully been Created";

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
        var validateDeletion = await checkIfPersonalNoteInputIsValid(userHash, personalnote);

        if (validateDeletion.HasError)
        {
            return validateDeletion;
        }




        // Modify Data Base
        #region Delete Personal Note in DB

        var sql = $"DELETE FROM PersonalNote WHERE userHash = \"{userHash}\" AND NoteDate = \"{personalnote.NoteDate}\"";

        deletePersonalNoteResponse = await this.deleteDataOnlyDAO.DeleteData(sql);
        timer.Stop();


        #endregion

        // Log
        string message = "The Note is successfully deleted";

        CreateLog(userHash, message, timer);


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

        var sql = $"SELECT FROM PersonalNote WHERE userHash = \"{userHash}\" AND NoteDate = \"{personalnote.NoteDate}\"";

        viewPersonalNoteResponse = await this.readDataOnlyDAO.ReadData(sql);
        timer.Stop();


        #endregion

        // Log
        string message = "The Note is successfully fetched";

        CreateLog(userHash, message, timer);


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

        var checkNoteSql = $"SELECT FROM PersonalNote WHERE userHash = \"{userHash}\" AND NoteDate = \"{personalnote.NoteDate}\"";
        updatePersonalNoteResponse = await this.readDataOnlyDAO.ReadData(checkNoteSql);


        string updateNoteSql = "UPDATE PersonalNote SET "
        + (personalnote.NoteContent != string.Empty ? $"NoteContent = \"{personalnote.NoteContent}\"," : "");

        updateNoteSql = updateNoteSql.Remove(updateNoteSql.Length - 1);

        updateNoteSql += $" WHERE UserHash = \"{userHash}\" AND NoteDate= \"{personalnote.NoteDate}\",";

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
            var personalNoteYear = Convert.ToInt32(personalnote.NoteDate.Substring(0, 4));

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

















    #endregion
}