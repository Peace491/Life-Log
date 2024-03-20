namespace MyLibrary;

public interface ISqlDAO {
    
    public Response ExecuteSQL(string sql);
}

// Or (following interface segregation principle)

public interface IReadOnlyDAO
{
    // Connection string can be like this: string connString = "Server;Database;UID=DnUserReadOnly;Password";
    Response GetData(string sql);

    Response Query(string sql); // Can be an interface

    // Pagination, Can be an interface, might be leaky abstraction but allow for better performance and scalability
    Response Query(string sql, int count = 10, int currentPage = 0);
}

public interface IWriteOnlyDAO
{
    Response WriteData(string sql);
}

//...

// public class DataGateway: IReadOnlyDAO, IWriteOnlyDAO {

// }