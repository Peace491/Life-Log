namespace Peace.Lifelog.SecurityTest;
using Peace.Lifelog.DataAccess;
public class AppAuthServiceShould
{
    private const int MAX_EXECUTION_TIME_IN_SECONDS = 3;
    private const string TABLE = "app_auth_service_test";



    // Setup for all test
    public AppAuthServiceShould()
    {
        var DDLTransactionDAO = new DDLTransactionDAO();
        var createMockTableSql = $"CREATE TABLE {TABLE} ("
        + "(user_id serial  NOT NULL," +
        "proof text  NOT NULL," +
        "CONSTRAINT app_auth_service_test_pk " +
        "PRIMARY KEY (user_id)" +
        ");";

        DDLTransactionDAO.ExecuteDDLCommand(createMockTableSql);
    }

    // Cleanup for all tests
    public async void Dispose()
    {
        var DDLTransactionDAO = new DDLTransactionDAO();

        var deleteMockTableSql = $"DROP TABLE {TABLE}";

        await DDLTransactionDAO.ExecuteDDLCommand(deleteMockTableSql);
    }


}
