namespace Peace.Lifelog.CalendarService;

using DomainModels;
using Peace.Lifelog.DataAccess;
using Peace.Lifelog.LLI;
using Peace.Lifelog.Logging;
using Peace.Lifelog.PersonalNote;
using System;



public class CalendarService : IGetMonthLLI, IEditLLIWithCalendar, ICreateLLIWithCalendar, IGetMonthPN, ICreatePNWithCalendar, IUpdatePNWithCalendar
{

    // contructor to create LLIService
    private CreateDataOnlyDAO createDataOnlyDAO;
    private ReadDataOnlyDAO readDataOnlyDAO;
    private UpdateDataOnlyDAO updateDataOnlyDAO;
    private DeleteDataOnlyDAO deleteDataOnlyDAO;
    private LogTarget logTarget;
    private Logging logging;
    private LLIService lliService;
    private PersonalNoteService personalNoteService;
    public CalendarService()
    {
        this.createDataOnlyDAO = new CreateDataOnlyDAO();
        this.readDataOnlyDAO = new ReadDataOnlyDAO();
        this.updateDataOnlyDAO = new UpdateDataOnlyDAO();
        this.deleteDataOnlyDAO = new DeleteDataOnlyDAO();
        this.logTarget = new LogTarget(this.createDataOnlyDAO);
        this.logging = new Logging(this.logTarget);
        this.lliService = new LLIService(this.createDataOnlyDAO, this.readDataOnlyDAO, this.updateDataOnlyDAO, this.deleteDataOnlyDAO, this.logging);
        this.personalNoteService = new PersonalNoteService(this.createDataOnlyDAO, this.readDataOnlyDAO, this.updateDataOnlyDAO, this.deleteDataOnlyDAO, this.logging);


    }
    

    public async Task<Response> GetMonthLLI(string userHash, int month, int year) 
    {
        var getLLIResponse = new Response();
        
        getLLIResponse.Output = new List<object>();

        DateTime validDateTime = new DateTime(year, month, 1);

        var getAllLLIResponse = await lliService.GetAllLLIFromUser(userHash);


        if (getAllLLIResponse.Output is not null)
        {
            foreach (LLI LLI in getAllLLIResponse.Output.Cast<LLI>())
            {

                DateTime LLIdateTime;
                DateTime.TryParseExact(LLI.Deadline, "M/d/yyyy h:mm:ss tt", null, System.Globalization.DateTimeStyles.None, out LLIdateTime);

                
                if (LLIdateTime.Year == validDateTime.Year && LLIdateTime.Month == validDateTime.Month)
                {
                    
                    getLLIResponse.Output.Add(LLI);
                }
            }
        }
        
        

        // return monthData

        return getLLIResponse;

    }

    public async Task<Response> CreateLLIWithCalendar(string userHash, LLI lli)
    {
        var createLLIResponse = await lliService.CreateLLI(userHash, lli);
        return createLLIResponse;
    }

    public async Task<Response> EditLLIWithCalendar(string userHash, LLI lli)
    {
        var editLLIResponse = await lliService.UpdateLLI(userHash, lli);
        return editLLIResponse;
    }


    public async Task<Response> GetMonthPN(string userHash, int month, int year)
    {
        var getPNResponse = new Response();

        getPNResponse.Output = new List<object>();

        DateTime validDateTime = new DateTime(year, month, 1);

        var getAllPNResponse = await personalNoteService.GetAllPersonalNotesFromUser(userHash);


        if (getAllPNResponse.Output is not null)
        {
            foreach (PN PN in getAllPNResponse.Output.Cast<PN>())
            {

                DateTime PNdateTime;
                //MAKE SURE PN.NOTEDATE IS THE  CORRECT FORMAT 
                
                DateTime.TryParseExact(PN.NoteDate, "M/d/yyyy h:mm:ss tt", null, System.Globalization.DateTimeStyles.None, out PNdateTime);

              
                if (PNdateTime.Year == validDateTime.Year && PNdateTime.Month == validDateTime.Month)
                {

                    getPNResponse.Output.Add(PN);
                }
            }
        }

        // return monthData

        return getPNResponse;

    }

    public async Task<Response> CreatePNWithCalendar(string userHash, PN pn)
    {
        var createPNResponse = await personalNoteService.CreatePersonalNote(userHash, pn);
        return createPNResponse;
    }

    public async Task<Response> UpdatePNWithCalendar(string userHash, PN pn)
    {
        var editPNResponse = await personalNoteService.UpdatePersonalNote(userHash, pn);
        return editPNResponse;
    }


}
