using DomainModels;
using Peace.Lifelog.Infrastructure;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Peace.Lifelog.PersonalNote;

public class PersonalNoteService : IPersonalNoteService
{
    private static int WARNING_TIME_LIMIT_IN_SECOND = 3;
    private static int ERROR_TIME_LIMIT_IN_SECOND = 5;
    private static int MAX_NOTE_LENGTH_IN_CHAR = 1200;
    private static int EARLIEST_NOTE_YEAR = 1960;
    private static int LATEST_NOTE_YEAR = DateTime.Today.Year;
    private IPersonalNoteRepo personalNoteRepo;
    private Logging.ILogging logging;

    public PersonalNoteService(IPersonalNoteRepo personalNoteRepo, Logging.ILogging logging)
    {
        this.personalNoteRepo = personalNoteRepo;
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

        // Create Personal Note in DB

        createPersonalNoteResponse = await this.personalNoteRepo.CreatePersonalNoteInDB(userHash, personalnote.NoteContent, personalnote.NoteDate);
        timer.Stop();

        // Log
        string message = "The Note has successfully been Created";

        CreateLog(userHash, message, timer);


        return createPersonalNoteResponse;
    }

    // Delete Note
    public async Task<Response> DeletePersonalNote(string userHash, string noteId)
    {
        var deletePersonalNoteResponse = new Response();
        var timer = new Stopwatch();
        timer.Start();

        // Validate Input 

        if (userHash == string.Empty)
        {
            deletePersonalNoteResponse.HasError = true;
            deletePersonalNoteResponse.ErrorMessage = "User Hash must not be empty";
            var errorMessage = "The Personal Note User Hash is invalid";
            var logResponse = this.logging.CreateLog("Logs", userHash, "Warning", "Persistent Data Store", errorMessage);
            return deletePersonalNoteResponse;
        }

        if (noteId == string.Empty || noteId is null)
        {
            deletePersonalNoteResponse.HasError = true;
            deletePersonalNoteResponse.ErrorMessage = "NoteId can not be empty";
            return deletePersonalNoteResponse;
        }

        // Modify Data Base
        // Delete Personal Note in DB
        var deleteResponse = await this.personalNoteRepo.DeletePersonalNoteInDB(userHash, noteId);
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

        #region Validate Input 

        if (userHash == string.Empty)
        {
            viewPersonalNoteResponse.HasError = true;
            viewPersonalNoteResponse.ErrorMessage = "User Hash must not be empty";
            var errorMessage = "The Personal Note User Hash is invalid";
            var logResponse = this.logging.CreateLog("Logs", userHash, "Warning", "Persistent Data Store", errorMessage);
            return viewPersonalNoteResponse;
        }

        if (DateTime.Parse(personalnote.NoteDate) > DateTime.Today)
        {
            viewPersonalNoteResponse.HasError = true;
            viewPersonalNoteResponse.ErrorMessage = "The Date is Invalid";
            var errorMessage = "The Date is invalid";
            var logResponse = this.logging.CreateLog("Logs", userHash, "Warning", "Persistent Data Store", errorMessage);
            return viewPersonalNoteResponse;
        }

        if (personalnote.NoteDate != string.Empty)
        {

            var personalNoteYear = DateTime.Parse(personalnote.NoteDate).Year;

            if (personalNoteYear < EARLIEST_NOTE_YEAR || personalNoteYear > LATEST_NOTE_YEAR)
            {
                viewPersonalNoteResponse.HasError = true;
                viewPersonalNoteResponse.ErrorMessage = "LLI Deadline is out of range";
                var errorMessage = "The LLI deadline is invalid";
                var logResponse = this.logging.CreateLog("Logs", userHash, "Warning", "Persistent Data Store", errorMessage);
                return viewPersonalNoteResponse;
            }
        }
        #endregion

        // Retrive Personal Note from DB
        var readPersonalNoteResponse = await this.personalNoteRepo.ReadPersonalNoteInDB(userHash, personalnote.NoteDate);

        timer.Stop();

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

        // Update Personal Note in DB
        updatePersonalNoteResponse = await this.personalNoteRepo.UpdatePersonalNoteInDB(personalnote.NoteContent, personalnote.NoteDate, personalnote.NoteId);

        if (updatePersonalNoteResponse.HasError)
        {
            updatePersonalNoteResponse.ErrorMessage = "Personal Note fields are invalid";

            var errorMessage = updatePersonalNoteResponse.ErrorMessage;
            var logResponse = this.logging.CreateLog("Logs", userHash, "ERROR", "Persistent Data Store", errorMessage);
        }

        timer.Stop();

        // Log
        string message = "The Note is successfully updated";

        CreateLog(userHash, message, timer);

        updatePersonalNoteResponse.HasError = false;
        return updatePersonalNoteResponse;
    }

    // Get all User Notes
    public async Task<Response> GetAllPersonalNotesFromUser(string userHash)
    {
        var readPersonalNoteResponse = new Response();

        #region Input Validation
        if (userHash == string.Empty)
        {
            readPersonalNoteResponse.HasError = true;
            readPersonalNoteResponse.ErrorMessage = "UserHash can not be empty";
            return readPersonalNoteResponse;
        }
        #endregion

        var timer = new Stopwatch();
        timer.Start();

        // Retrive all Personal Notes from DB
        readPersonalNoteResponse = await this.personalNoteRepo.ReadAllPersonalNoteInDB(userHash);

        if (readPersonalNoteResponse.Output == null)
        {
            var message = "There is no personal Note associated with the account";
            var logResponse = this.logging.CreateLog("Logs", userHash, "ERROR", "Persistent Data Store", message);
            return readPersonalNoteResponse;
        }

        timer.Stop();

        #region Log

        if (readPersonalNoteResponse.HasError)
        {
            readPersonalNoteResponse.ErrorMessage = "Personal Notes fields are invalid";

            var errorMessage = "Failed to fetch user's Personal Notes";
            var logResponse = this.logging.CreateLog("Logs", userHash, "ERROR", "Persistent Data Store", errorMessage);
        }
        else
        {
            var logResponse = this.logging.CreateLog("Logs", userHash, "Info", "Persistent Data Store", $"{userHash} get all Personal Notes.");
        }

        if (timer.Elapsed.TotalSeconds > WARNING_TIME_LIMIT_IN_SECOND && timer.Elapsed.TotalSeconds < ERROR_TIME_LIMIT_IN_SECOND)
        {
            var errorMessage = "Operation exceeded time frame";
            var logResponse = this.logging.CreateLog("Logs", userHash, "Warning", "Persistent Data Store", errorMessage);
        }
        else if (timer.Elapsed.TotalSeconds > ERROR_TIME_LIMIT_IN_SECOND)
        {
            var errorMessage = "Operation took too long";
            var logResponse = this.logging.CreateLog("Logs", userHash, "ERROR", "Persistent Data Store", errorMessage);
        }
        #endregion

        var personalNoteOutput = ConvertDatabaseResponseOutputToPersonalNoteObjectList(readPersonalNoteResponse);

        readPersonalNoteResponse.Output = personalNoteOutput;

        return readPersonalNoteResponse;
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
            var logResponse = await this.logging.CreateLog("Logs", userHash, "Warning", "Persistent Data Store", errorMessage);
            return validationResponse;
        }

        if (personalnote.NoteDate == null || personalnote.NoteContent == null)
        {
            validationResponse.HasError = true;
            validationResponse.ErrorMessage = "The non-nullable Personal Note input is null";
            var errorMessage = "The non-nullable Personal Note input is null";
            var logResponse = await this.logging.CreateLog("Logs", userHash, "Warning", "Persistent Data Store", errorMessage);
            return validationResponse;
        }

        if (personalnote.NoteContent == null || personalnote.NoteContent.Length > MAX_NOTE_LENGTH_IN_CHAR)
        {
            validationResponse.HasError = true;
            validationResponse.ErrorMessage = "The Personal Note content is too long";
            var errorMessage = "The personal note contents is invalid";
            var logResponse = await this.logging.CreateLog("Logs", userHash, "Warning", "Persistent Data Store", errorMessage);
            return validationResponse;
        }

        if (personalnote.NoteContent != null)
        {
            if (!Regex.IsMatch(personalnote.NoteContent.Replace(" ", ""), @"^[a-zA-Z0-9]+$"))
            {
                validationResponse.HasError = true;
                validationResponse.ErrorMessage = "The personal note content has invalid nonalphanumeric characters";
                var errorMessage = "Note contains nonalphanumeric characters";
                var logResponse = await this.logging.CreateLog("Logs", userHash, "Warning", "Persistent Data Store", errorMessage);
                return validationResponse;
            }
        }

        if (DateTime.Parse(personalnote.NoteDate) > DateTime.Today)
        {
            validationResponse.HasError = true;
            validationResponse.ErrorMessage = "The Date is Invalid";
            var errorMessage = "The Date is invalid";
            var logResponse = await this.logging.CreateLog("Logs", userHash, "Warning", "Persistent Data Store", errorMessage);
            return validationResponse;
        }

        if (personalnote.NoteDate != string.Empty)
        {

            var personalNoteYear = DateTime.Parse(personalnote.NoteDate).Year;

            if (personalNoteYear < EARLIEST_NOTE_YEAR || personalNoteYear > LATEST_NOTE_YEAR)
            {
                validationResponse.HasError = true;
                validationResponse.ErrorMessage = "The Note date is out of range";
                var errorMessage = "The Note date is invalid";
                var logResponse = await this.logging.CreateLog("Logs", userHash, "Warning", "Persistent Data Store", errorMessage);
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