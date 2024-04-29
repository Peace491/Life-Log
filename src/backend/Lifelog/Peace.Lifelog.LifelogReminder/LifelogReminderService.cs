namespace Peace.Lifelog.LifelogReminderService;

using MimeKit;
using MailKit.Net.Smtp;
using DomainModels;
using Peace.Lifelog.Security;
using Peace.Lifelog.DataAccess;
using Peace.Lifelog.Logging;
public class LifelogReminderService : ILifelogReminderService
{
    LifelogConfig lifelogConfig = LifelogConfig.LoadConfiguration(); 
    private ILifelogReminderRepo lifelogReminderRepo;
    private ILifelogAuthService lifelogAuthService;
    private ILogging logging;

    public LocationRecommendationService(ILifelogReminderRepo lifelogReminderRepo, ILifelogAuthService lifelogAuthService, ILogging logging)
    {
        this.lifelogReminderRepo = lifelogReminderRepo;
        this.lifelogAuthService = lifelogAuthService;
        this.logging = logging;
    }
    #region Update Reminder Config
    public async Task<Response> UpdateReminderConfiguration(ReminderFormData form, string userHash)
    {
        Response response = new Response();
        response.HasError = false;
        
        response = IsUserHashValid(response, userHash);
        if(response.HasError)
        {
            return response;
        }
        
        string content = form.Content;
        string frequency = form.Frequency;
        response = CheckIfUserHashInDB(response, userHash);
        if(response.HasError)
        {
            response.HasError = true;
            response.errorMessage = "Error in SQL or DB";
            response = Logging(response, userHash, "Error", "Persistant Database");
            return response;
        }
        Response addContentAndFrequencyToDB = new Response()
        try
        {
            addContentAndFrequencyToDB = await this.lifelogReminderRepo.UpdateContentAndFrequency(userHash, content, frequency);
        }
        catch (exception error)
        {
            response.HasError = true;
            response.errorMessage = "SQL or input is invalid";
            response = Logging(response, userHash, "Error", "persistant data store");
        }
        response = addContentAndFrequencyToDB;
        return response;
    }
    #endregion
    #region Send Email
    public async Task<Response> SendReminderEmail(string userHash)
    {
        Response response = new Response();
        response.HasError = false;
        // validates the user hash
        response = IsUserHashValid(response, userHash);
        if(response.HasError)
        {
            return response;
        }
        // checks if the user is already instantiated in the database
        response = CheckIfUserHashInDB(response, userHash);
        if(response.HasError)
        {
            response.HasError = true;
            response.errorMessage = "Error in SQL or DB";
            response = Logging(response, userHash, "Error", "Persistant Database");
            return response;
        }
        // gets the content and frequency specification for the user
        Response getContentAndFrequency = new Response();
        try
        {
            getContentAndFrequency = await this.lifelogReminderRepo.GetContentAndFrequency(userHash);
        }
        catch(exception error)
        {
            response.HasError = true;
            response.errorMessage = "SQL or input is invalid";
            response = Logging(response, userHash, "Error", "persistant data store");
            return response;
        }
        // parses through output to assign content and frequency
        int days = 0;
        string frequency = "";
        string content = "";
        foreach(List<object> Object in getContentAndFrequency.Output)
        {
            string frequency = Object[0];
            days = int.Parse(frequency);
            content = Object[1];
        }
        //checks if an email has already been sent out in this timeframe
        Response checkCurrentReminderResponse = new Response();
        try
        {
            checkCurrentReminderResponse = await this.lifelogReminderRepo.CheckCurrentReminder(userHash, days);
        }
        catch(exception error)
        {
            response.HasError = true;
            response.errorMessage = "SQL or input is invalid";
            response = Logging(response, userHash, "Error", "persistant data store");
            return response;
        }
        if(checkCurrentReminderResponse.Output.Count > 0)
        {
            response.errorMessage = "Email has already been sent";
            response = Logging(response, userHash, "info", "business");
            return response;
        }
        /*
            implement Specilized email content here
        */
        //Formating email
        var reminder = new MimeMessage();
        reminder.From.Add(new MailboxAddress("Lifelog", lifelogConfig.LifelogSystemEmail)); // Get from config
        Response getUserIdResponse;
        try
        {
            getUserIdResponse = await this.lifelogReminderRepo.GetUserID(userHash)
        }
        catch (Exception error)
        {
            response.HasError = true;
            response.errorMessage = "SQL or input is invalid";
            response = Logging(response, userHash, "Error", "persistant data store");
            return response;
        }
        string userId = "";
        foreach(List<object> Object in getUserIdResponse.Output)
        {
            userId = Object[0].ToString();
        }
        reminder.To.Add(new MailboxAddress("", userId));
        reminder.Subject = "Come on Back to Lifelog! Your LLI Item's are Waiting For You!"
        var body = new BodyBuilder();
        if(content == "Active")
        {
            body.HtmlBody = "<h1>Hey there!</h1>" +
                 "<p>We noticed you haven't been active on Lifelog for more than a " + frequency + ". " +
                 "You have active LLI(s) that could be completed. " +
                 "Make sure to write notes on it and mark some pins!</p>" +
                 "<p>Thanks for using our application from Team Peace</p>";
        }
        else
        {
            body.HtmlBody = body.HtmlBody = "<h1>Hey there!</h1>" +
                 "<p>We noticed you haven't been active on Lifelog for more than a " + frequency + ". " +
                 "Let's help you continue your journey with Lifelog by using our recommendation's. " +
                 "Make sure to mark your calendar and your media to your completed LLI(s)!</p>" +
                 "<p>Thanks for using our application from Team Peace</p>";
        }
        reminder.Body = body.ToMessageBody();
        try
        {
            var emailResponse = SendEmail(reminder);
        }
        catch(Exception error)
        {
            response.HasError= true;
            response.errorMessage = "Error sending email";
            response = Logging(response, userHash, "Error", "API");
            return response;
        }

        // update the date in the database
        Response updateDate;
        try
        {
            updateDate = await this.lifelogReminderRepo.UpdateCurrentDate(userHash);
        }
        catch(exception error)
        {
            response.HasError= true;
            response.errorMessage = "SQL or input is invalid";
            response = Logging(response, userHash, "Error", "persistant data store");
            return response;
        }
        response.Output = "Email Sent Successfully";
        response.ErrorMessage = "Email Sent Successfully";
        Logging(response, userHash, "info", "business")
        response.Error = null;
        return response;
    }
#endregion

    #region Helper Functions
    public async Response CheckIfUserHashInDB(Response response, string userHash)
    {
        Response addUserToDBResponse = new Response();
        Response checkIfUserInDBResponse= new Response();
        try
        {
            checkIfUserInDBResponse = await this.lifelogReminderRepo.CheckIfUserHashInDB(userHash);
        }
        catch (Exception error)
        {
            response.HasError = true;
        }
        if(checkIfUserInDBResponse.Output.Count == 0)
        {
            try
            {
                addUserToDBResponse = await this.lifelogReminderRepo.AddUserHashAndDate(userHash);
            }
            catch(Exception error)
            {
                response.HasError = true;
            }
            response = addUserToDBResponse;
        }
        else
        {
            response = checkIfUserInDBResponse;
        }

        return response;
    }
    private Response SendEmail(MimeMessage email)
        
        {
        var response = new Response();
        using (var client = new SmtpClient())
        {
            try
            {
                client.Connect("smtp.gmail.com", 587, false); // Use SSL and correct port
                client.Authenticate(lifelogConfig.LifelogSystemEmail, lifelogConfig.LifelogSystemEmailAppPassword);
                client.Send(email);
                client.Disconnect(true);
                response.HasError = false;
            }
            catch (Exception ex)
            {
                response.HasError = true;
                response.ErrorMessage = ex.Message;
            }
        }
        return response;
    }
    
    private response IsUserHashValid(Response response, string userHash)
    {
        if(userHash == null)
        {
            response.HasError = true;
            response.errorMessage = "User Hash is Invalid";
            response = Logging(response, userHash, "info", "business");
            return response;
        }
        return response;
    }
    private Response Logging(Response response, string userHash, string logtype, string logfield)
    {
        var logResponse = this.logging.CreateLog("Logs", userHash, logtype, logfield, response.errorMessage);
        return response;
    }
    #endregion
}