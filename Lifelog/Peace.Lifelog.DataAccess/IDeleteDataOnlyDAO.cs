namespace Peace.Lifelog.DataAccess;

using DomainModels;

public interface IDeleteDataOnlyDAO {
    Response DeleteData(string sql);
}