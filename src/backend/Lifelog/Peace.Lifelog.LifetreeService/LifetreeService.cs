namespace Peace.Lifelog.LifetreeService;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainModels;
using Peace.Lifelog.DataAccess;
using Peace.Lifelog.Infrastructure;
using Peace.Lifelog.LLI;
using Peace.Lifelog.Logging;
using Peace.Lifelog.PersonalNote;



public class LifetreeService
{
    #region contructor method
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
    public LifetreeService()
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


    public async Task<Response> getAllCompletedLLI(string userHash)
    {
        var timer = new Stopwatch();
        timer.Start();

        var getCompletedLLIResponse = new Response();
        getCompletedLLIResponse.Output = new List<object>();


        var getAllLLIResponse = await lliService.GetAllLLIFromUser(userHash);



        if (getAllLLIResponse.Output is not null)
        {
            
            foreach (LLI LLI in getAllLLIResponse.Output.Cast<LLI>())
            {
                if (LLI.Status == LLIStatus.Completed)
                {
                    getCompletedLLIResponse.Output.Add(LLI);
                }
                
            }
        }
        getCompletedLLIResponse.HasError = getAllLLIResponse.HasError;
        getCompletedLLIResponse.ErrorMessage = getAllLLIResponse.ErrorMessage;

        timer.Stop();

        // add logs here

        return getCompletedLLIResponse;
    }

    public async Task<Response> GetOnePNWithLifetree(string userHash, PN pn)
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
    public async Task<Response> CreatePNWithLifetree(string userHash, PN pn)
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
}

