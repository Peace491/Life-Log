using DomainModels;

namespace Peace.Lifelog.DataAccess;

public interface ICreateOnlyDAO {
    Response CreateData(string sql);
}

public interface IReadOnlyDAO {
    Response ReadData(string sql, int count = 10, int currentPage = 0);
}

public interface IUpdateOnlyDAO {
    Response UpdateData(string sql);
}

public interface IDeleteOnlyDAO {
    Response DeleteData(string sql);
}