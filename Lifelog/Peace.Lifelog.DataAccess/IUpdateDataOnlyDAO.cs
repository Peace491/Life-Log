namespace Peace.Lifelog.DataAccess;

using DomainModels;

public interface IUpdateDataOnlyDAO {
    Response UpdateData(string sql);
}
