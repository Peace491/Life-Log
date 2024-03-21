using MyLibrary;

// Repository is a specific implementation of the DAO
public class UserRepository
{
    private readonly ISqlDAO _sqlDAO; //readonly: can only be set by constructor

    // Dependency Inversion, we are depending on ISqlDAO
    public UserRepository(ISqlDAO sqlDAO) 
    {
        _sqlDAO = sqlDAO;
    }

    public Response GetUsers()
    {
        var userSelect = "Select * from Users WHERE status = 'Active'";

        return _sqlDAO.ExecuteSQL(userSelect);
    }
}