namespace Peace.Lifelog.CalendarService;

using DomainModels;
using Peace.Lifelog.DataAccess;
using Peace.Lifelog.Infrastructure;
using Peace.Lifelog.LLI;
using Peace.Lifelog.Logging;
using Peace.Lifelog.PersonalNote;
using System;
using System.Diagnostics;

public class CalendarService : IGetMonthLLI, IEditLLIWithCalendar, ICreateLLIWithCalendar, IGetMonthPN, ICreatePNWithCalendar, IUpdatePNWithCalendar
{

    // contructor to create LLIService and Personal Note Service
    #region Constructor Method
    private static int WARNING_TIME_LIMIT_IN_SECOND = 3;
    private static int ERROR_TIME_LIMIT_IN_SECOND = 5;
    private CreateDataOnlyDAO createDataOnlyDAO;
    private ReadDataOnlyDAO readDataOnlyDAO;
    private UpdateDataOnlyDAO updateDataOnlyDAO;
    private DeleteDataOnlyDAO deleteDataOnlyDAO;
    private LogTarget logTarget;
    private Logging logging;
    private LLIService lliService;
    private PersonalNoteService personalNoteService;
    private IPersonalNoteRepo personalNoteRepo;
    public CalendarService()
    {
        this.createDataOnlyDAO = new CreateDataOnlyDAO();
        this.readDataOnlyDAO = new ReadDataOnlyDAO();
        this.updateDataOnlyDAO = new UpdateDataOnlyDAO();
        this.deleteDataOnlyDAO = new DeleteDataOnlyDAO();
        this.logTarget = new LogTarget(this.createDataOnlyDAO, readDataOnlyDAO);
        this.logging = new Logging(this.logTarget);
        this.lliService = new LLIService(this.createDataOnlyDAO, this.readDataOnlyDAO, this.updateDataOnlyDAO, this.deleteDataOnlyDAO, this.logging);
        this.personalNoteRepo = new PersonalNoteRepo(createDataOnlyDAO, readDataOnlyDAO, updateDataOnlyDAO, deleteDataOnlyDAO);
        this.personalNoteService = new PersonalNoteService(personalNoteRepo, this.logging);


    }
    #endregion


    // get all LLI for a specified month
    public async Task<Response> GetMonthLLI(string userHash, int month, int year)
    {
        var timer = new Stopwatch();

        timer.Start();

        var getLLIResponse = new Response();

        getLLIResponse.Output = new List<object>();

        DateTime validDateTime = new DateTime(year, month, 1);


        var getAllLLIResponse = await lliService.GetAllLLIFromUser(userHash);



        if (getAllLLIResponse.Output is not null)
        {
            Console.WriteLine("lliservice works");
            foreach (LLI LLI in getAllLLIResponse.Output.Cast<LLI>())
            {
                var lliDeadline = LLI.Deadline!.Substring(0, LLI.Deadline!.IndexOf(' '));


                DateTime LLIdateTime;
                LLIdateTime = DateTime.ParseExact(lliDeadline, "M/d/yyyy", null);

                Console.WriteLine(LLIdateTime.Month);
                Console.WriteLine(LLIdateTime.Year);
                Console.WriteLine(validDateTime.Month);
                Console.WriteLine(validDateTime.Year);


                if (LLIdateTime.Year == validDateTime.Year && LLIdateTime.Month == validDateTime.Month)
                {

                    getLLIResponse.Output.Add(LLI);
                }
            }
        }
        getLLIResponse.HasError = getAllLLIResponse.HasError;
        getLLIResponse.ErrorMessage = getAllLLIResponse.ErrorMessage;

        timer.Stop();

        var successLogResponse = this.logging.CreateLog("Logs", userHash, "Info", "Persistent Data Store", "The user fetched all lli using the calendar");

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


        return getLLIResponse;

    }

    //Calls CreateLLI from lliservice to create a LLI
    public async Task<Response> CreateLLIWithCalendar(string userHash, LLI lli)
    {
        var timer = new Stopwatch();

        timer.Start();
        var createLLIResponse = await lliService.CreateLLI(userHash, lli);
        timer.Stop();

        var successLogResponse = this.logging.CreateLog("Logs", userHash, "Info", "Persistent Data Store", "The user created a LLI using the calendar");

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

        return createLLIResponse;
    }

    //Calls UpdateLLI from lliservice to edit a LLI
    public async Task<Response> EditLLIWithCalendar(string userHash, LLI lli)
    {
        var timer = new Stopwatch();

        timer.Start();
        var editLLIResponse = await lliService.UpdateLLI(userHash, lli);
        timer.Stop();

        var successLogResponse = this.logging.CreateLog("Logs", userHash, "Info", "Persistent Data Store", "User changed a LLI using the calendar");

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

        return editLLIResponse;
    }

    //Gets all the PN of the specified month
    public async Task<Response> GetMonthPN(string userHash, int month, int year)
    {
        var timer = new Stopwatch();

        timer.Start();

        var getPNResponse = new Response();

        getPNResponse.Output = new List<object>();

        DateTime validDateTime = new DateTime(year, month, 1);

        var getAllPNResponse = await personalNoteService.GetAllPersonalNotesFromUser(userHash);


        if (getAllPNResponse.Output is not null)
        {
            foreach (PN PN in getAllPNResponse.Output.Cast<PN>())
            {

                var pnDeadline = PN.NoteDate!.Substring(0, PN.NoteDate!.IndexOf(' '));


                DateTime PNdateTime;
                PNdateTime = DateTime.ParseExact(pnDeadline, "M/d/yyyy", null);

                if (PNdateTime.Year == validDateTime.Year && PNdateTime.Month == validDateTime.Month)
                {

                    getPNResponse.Output.Add(PN);
                }
            }
        }

        getPNResponse.HasError = getAllPNResponse.HasError;
        getPNResponse.ErrorMessage = getAllPNResponse.ErrorMessage;

        timer.Stop();

        var successLogResponse = this.logging.CreateLog("Logs", userHash, "Info", "Persistent Data Store", "User fetched all PN using the calendar");

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

        // return monthData

        return getPNResponse;

    }

    //Gets only one PN for specified date (input date into PN obj)
    public async Task<Response> GetOnePNWithCalendar(string userHash, PN pn)
    {
        var timer = new Stopwatch();

        timer.Start();
        var getPNResponse = await personalNoteService.ViewPersonalNote(userHash, pn);
        timer.Stop();

        var successLogResponse = this.logging.CreateLog("Logs", userHash, "Info", "Persistent Data Store", "User is viewing Personal Note though calendar");

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

        return getPNResponse;
    }

    // uses CreatePN from pnservice to create a PN
    public async Task<Response> CreatePNWithCalendar(string userHash, PN pn)
    {
        var timer = new Stopwatch();

        timer.Start();
        var createPNResponse = await personalNoteService.CreatePersonalNote(userHash, pn);
        timer.Stop();

        var successLogResponse = this.logging.CreateLog("Logs", userHash, "Info", "Persistent Data Store", "The note is successfully created though the calendar");

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

        return createPNResponse;
    }

    //uses updatePN from pnservice to update PN
    public async Task<Response> UpdatePNWithCalendar(string userHash, PN pn)
    {
        var timer = new Stopwatch();

        timer.Start();
        var editPNResponse = await personalNoteService.UpdatePersonalNote(userHash, pn);
        timer.Stop();

        var successLogResponse = this.logging.CreateLog("Logs", userHash, "Info", "Persistent Data Store", "The note is successfully edited though the calendar");

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

        return editPNResponse;
    }


}
