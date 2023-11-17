namespace Peace.Lifelog.DataAccess;

using DomainModels;

public interface IReadDataOnlyDAO {
    Response ReadData(string sql, int count = 10, int currentPage = 0);
}
