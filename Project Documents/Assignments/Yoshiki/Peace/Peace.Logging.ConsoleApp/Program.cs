namespace Peace.Logging.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Log a statement to the databse: ");

            string log = Console.ReadLine();

            LogData logData = new LogData();

            logData.Log(log);

            Console.WriteLine("Your statement has been logged.");

        }
    }
}
