namespace Peace.Logging
{
    public class LogData
    {
        DateTime currentDateTime = DateTime.Now;
        Peace.Logging.DataAccess.DataAccess dataAccess;

        public LogData()
        {
            string connectionString = "Server=LAPTOP-JNHE03DV;Database=DemoDB;User Id=Yoshiki;Password=Yoshiki@0103;";
            this.dataAccess = new Peace.Logging.DataAccess.DataAccess(connectionString);
        }

        public void Log(string userInput)
        {
            this.dataAccess.SaveDataToTable(userInput, currentDateTime);
        }


    }
}