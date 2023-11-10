namespace Peace.Logging.ConsoleApp

{
    class Program
    {
        static void Main(string[] args)
        {
            // Import the logger and use its newlog method to log
            Logger logger = new Logger();
            logger.NewLog();
        }
    }
}