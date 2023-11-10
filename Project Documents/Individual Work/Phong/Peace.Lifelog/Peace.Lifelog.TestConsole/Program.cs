using Peace.Lifelog.Logging;

/// <summary>
/// Represents the entry point of the application.
/// </summary>
class Program
{
    /// <summary>
    /// The main method of the application.
    /// </summary>
    /// <param name="args">Command-line arguments.</param>
    static void Main(string[] args)
    {
        // Connection String to the database
        string connectionString = "Server = localhost; Database = LifelogDB; User ID = LifelogWriteUser; Password = password;";

        // Create a logger instance
        Logger logger = new Logger(connectionString);

        // Prompt user for log input
        Console.WriteLine("Enter log:");
        string log = Console.ReadLine();

        // Log the entered message
        logger.Log(log);

        // Indicate successful logging
        Console.WriteLine("Log Successful");
    }
}
