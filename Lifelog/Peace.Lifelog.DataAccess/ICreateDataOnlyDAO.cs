namespace Peace.Lifelog.DataAccess;

using DomainModels;

public interface ICreateDataOnlyDAO {
    Response CreateData(string sql);
}
