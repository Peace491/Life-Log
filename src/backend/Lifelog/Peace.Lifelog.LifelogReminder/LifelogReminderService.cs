namespace Peace.Lifelog.LifelogReminder;

using DomainModels;
using MailKit.Net.Smtp;
using MimeKit;
using Peace.Lifelog.Infrastructure;
using Peace.Lifelog.Logging;
using Peace.Lifelog.Security;
public class LifelogReminderService : ILifelogReminderService
{
    LifelogConfig lifelogConfig = LifelogConfig.LoadConfiguration();
    private ILifelogReminderRepo lifelogReminderRepo;
    private ILifelogAuthService lifelogAuthService;
    private ILogging logging;

    public LifelogReminderService(ILifelogReminderRepo lifelogReminderRepo, ILifelogAuthService lifelogAuthService, ILogging logging)
    {
        this.lifelogReminderRepo = lifelogReminderRepo;
        this.lifelogAuthService = lifelogAuthService;
        this.logging = logging;
    }
    #region Update Reminder Config
    public async Task<Response> UpdateReminderConfiguration(ReminderFormData form)
    {
        Response response = new Response();
        response.HasError = false;
        string userHash = form.UserHash;

        response = IsUserHashValid(response, userHash);
        if (response.HasError)
        {
            return response;
        }

        string content = form.Content;
        string frequency = form.Frequency;
        response = await CheckIfUserHashInDB(response, userHash);
        if (response.HasError)
        {
            response.HasError = true;
            response.ErrorMessage = "Error in SQL or DB";
            response = Logging(response, userHash, "Error", "Persistant Database");
            return response;
        }
        Response addContentAndFrequencyToDB = new Response();
        try
        {
            addContentAndFrequencyToDB = await this.lifelogReminderRepo.UpdateContentAndFrequency(userHash, content, frequency);
        }
        catch (Exception error)
        {
            response.HasError = true;
            response.ErrorMessage = error.ToString();
            response = Logging(response, userHash, "Error", "persistant data store");
        }
        response = addContentAndFrequencyToDB;
        return response;
    }
    #endregion
    #region Send Email
    public async Task<Response> SendReminderEmail(ReminderFormData form)
    {
        Response response = new Response();
        response.HasError = false;
        string userHash = form.UserHash;
        form.Content = null!;
        form.Frequency = null!;
        // validates the user hash
        response = IsUserHashValid(response, userHash);
        if (response.HasError)
        {
            return response;
        }
        // checks if the user is already instantiated in the database
        response = await CheckIfUserHashInDB(response, userHash);
        if (response.HasError)
        {
            response.HasError = true;
            response.ErrorMessage = "Error in SQL or DB";
            response = Logging(response, userHash, "Error", "Persistant Database");
            return response;
        }
        // gets the content and frequency specification for the user
        Response getContentAndFrequency = new Response();
        try
        {
            getContentAndFrequency = await this.lifelogReminderRepo.GetContentAndFrequency(userHash);
        }
        catch (Exception error)
        {
            response.HasError = true;
            response.ErrorMessage = error.ToString();
            response = Logging(response, userHash, "Error", "persistant data store");
            return response;
        }
        // parses through output to assign content and frequency
        int days = 0;
        string frequency = "";
        string content = "";
        foreach (List<object> Object in getContentAndFrequency.Output!)
        {
            content = Object[0].ToString()!;
            frequency = Object[1].ToString()!;
        }
        if (frequency == "Weekly")
        {
            days = 7;
        }
        else
        {
            days = 30;
        }
        //checks if an email has already been sent out in this timeframe
        Response checkCurrentReminderResponse = new Response();
        try
        {
            checkCurrentReminderResponse = await this.lifelogReminderRepo.CheckCurrentReminder(userHash, days);
        }
        catch (Exception error)
        {
            response.HasError = true;
            response.ErrorMessage = error.ToString();
            response = Logging(response, userHash, "Error", "persistant data store");
            return response;
        }
        if (checkCurrentReminderResponse.Output?.Count != null)
        {
            response.ErrorMessage = "Email has already been sent";
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
            getUserIdResponse = await this.lifelogReminderRepo.GetUserID(userHash);
        }
        catch (Exception error)
        {
            response.HasError = true;
            response.ErrorMessage = error.ToString();
            response = Logging(response, userHash, "Error", "persistant data store");
            return response;
        }
        string userId = "";
        foreach (List<object> Object in getUserIdResponse.Output!)
        {
            userId = Object[0].ToString()!;
        }
        reminder.To.Add(new MailboxAddress("", userId));
        reminder.Subject = "Come on Back to Lifelog! Your LLI Item's are Waiting For You!";
        var body = new BodyBuilder();
        if (content == "Active")
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
        catch (Exception error)
        {
            response.HasError = true;
            response.ErrorMessage = error.ToString();
            response = Logging(response, userHash, "Error", "API");
            return response;
        }


        // update the date in the database
        Response updateDate;
        try
        {
            updateDate = await this.lifelogReminderRepo.UpdateCurrentDate(userHash);
        }
        catch (Exception error)
        {
            response.HasError = true;
            response.ErrorMessage = error.ToString();
            response = Logging(response, userHash, "Error", "persistant data store");
            return response;
        }
        string output = "Email Sent Successfully";
        response.Output = ConvertStringOutputToResponseOutput(output);
        response.ErrorMessage = "Email Sent Successfully";
        Logging(response, userHash, "info", "business");
        response.ErrorMessage = null;
        return response;
    }
    #endregion

    #region Helper Functions
    public async Task<Response> CheckIfUserHashInDB(Response response, string userHash)
    {
        Response addUserToDBResponse = new Response();
        Response checkIfUserInDBResponse = new Response();
        try
        {
            checkIfUserInDBResponse = await this.lifelogReminderRepo.CheckIfUserHashInDB(userHash);
        }
        catch (Exception error)
        {
            response.HasError = true;
            response.ErrorMessage = error.ToString();
        }
        if (checkIfUserInDBResponse.Output?.Count == null)
        {
            try
            {
                addUserToDBResponse = await this.lifelogReminderRepo.AddUserHashAndDate(userHash);
            }
            catch (Exception error)
            {
                response.HasError = true;
                response.ErrorMessage = error.ToString();
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

    private Response IsUserHashValid(Response response, string userHash)
    {
        if (userHash == null)
        {
            response.HasError = true;
            response.ErrorMessage = "User Hash is Invalid";
            response = Logging(response, userHash!, "info", "business");
            return response;
        }
        return response;
    }
    private Response Logging(Response response, string userHash, string logtype, string logfield)
    {
        var logResponse = this.logging.CreateLog("Logs", userHash, logtype, logfield, response.ErrorMessage);
        return response;
    }
    private List<object>? ConvertStringOutputToResponseOutput(string message)
    {
        List<object> output = new List<object>();
        output.Add(message);
        return output;
    }
    #endregion
}