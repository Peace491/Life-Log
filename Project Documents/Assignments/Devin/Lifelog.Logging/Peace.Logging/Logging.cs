namespace Peace.Logging
{
    public class LogData
    {
        DateTime currentDateTime = DateTime.Now;
        Peace.Logging.DataAccess.DataAccess dataAccess;

        public LogData()
        {
            string connectionString = "Server=LAPTOP-IDD37K5P;Database=491;User Id=devin;Password=kothari;";
            this.dataAccess = new Peace.Logging.DataAccess.DataAccess(connectionString);
        }

        public void Log(string userInput)
        {
            this.dataAccess.SaveDataToTable(userInput, currentDateTime);
        }


    }
}