namespace Peace.Logging
{
    public class LogData
    {
        DateTime currentDateTime = DateTime.Now;
        Peace.Logging.DataAccess.DataAccess dataAccess;

        public LogData()
        {
            string connectionString = "Server=DESKTOP-0VURBLE;Database=TestDB;User Id=Zarif;Password=Zarif123;";
            this.dataAccess = new Peace.Logging.DataAccess.DataAccess(connectionString);
        }

        public void Log(string userInput)
        {
            this.dataAccess.SaveDataToTable(userInput, currentDateTime);
        }


    }
}