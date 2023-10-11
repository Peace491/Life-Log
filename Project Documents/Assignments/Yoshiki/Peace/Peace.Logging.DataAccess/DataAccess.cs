using System.Data.SqlClient;

namespace Peace.Logging.DataAccess
{
    public class DataAccess
    {
        private readonly string connectionString;

        public DataAccess(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public void SaveDataToTable(string data, DateTime timestamp)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string sql = $"INSERT INTO Demo (LoggedTests, TimeStamp) VALUES (@Data, @timestamp)";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Data", data);
                    command.Parameters.AddWithValue("@timestamp", timestamp);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}