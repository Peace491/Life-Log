using MySql.Data.MySqlClient;

namespace Peace.Lifelog.DataAccess
{
    /// <summary>
    /// Provides data access methods for saving log entries to a MySQL database.
    /// </summary>
    public class DataAccess
    {
        private AppDbContext _appDbContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataAccess"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string for the MySQL database.</param>
        public DataAccess(string connectionString)
        {
            this._appDbContext = new AppDbContext(connectionString);
        }

        /// <summary>
        /// Saves log data to the database.
        /// </summary>
        /// <param name="logData">The log data to be saved.</param>
        public void SaveToLog(string logData)
        {
            using (var connection = this._appDbContext.GetConnection())
            {
                // Open the database connection
                connection.Open();

                using (var command = new MySqlCommand())
                {
                    // Set the connection for the command
                    command.Connection = connection;

                    // Define the SQL command to insert log data with a timestamp
                    command.CommandText = "INSERT INTO logs (log_data, log_timestamp) VALUES (@logData, NOW())";

                    // Add parameter for log data
                    command.Parameters.AddWithValue("@logData", logData);

                    // Execute the SQL command
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}


