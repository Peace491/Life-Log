namespace Peace.Lifelog.Logging
{
    /// <summary>
    /// Provides logging functionality by saving log messages to a data store using DataAccess.
    /// </summary>
    public class Logger
    {
        private readonly DataAccess.DataAccess _dataAccess;

        /// <summary>
        /// Initializes a new instance of the <see cref="Logger"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string for the underlying data store.</param>
        public Logger(string connectionString)
        {
            _dataAccess = new DataAccess.DataAccess(connectionString);
        }

        /// <summary>
        /// Logs a message by saving it to the underlying data store.
        /// </summary>
        /// <param name="logMessage">The message to be logged.</param>
        public void Log(string logMessage)
        {
            _dataAccess.SaveToLog(logMessage);
        }
    }
}
