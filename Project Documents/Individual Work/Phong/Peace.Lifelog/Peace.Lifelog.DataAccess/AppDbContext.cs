using MySql.Data.MySqlClient;

namespace Peace.Lifelog.DataAccess
{
    /// <summary>
    /// Represents a database context for managing database connections.
    /// </summary>
    public class AppDbContext
    {
        /// <summary>
        /// Gets or sets the connection string for the database.
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AppDbContext"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string for the database.</param>
        public AppDbContext(string connectionString)
        {
            this.ConnectionString = connectionString;
        }

        /// <summary>
        /// Creates and returns a new instance of MySqlConnection using the stored connection string.
        /// </summary>
        /// <returns>An instance of MySqlConnection.</returns>
        public MySqlConnection GetConnection()
        {
            return new MySqlConnection(this.ConnectionString);
        }
    }
}
