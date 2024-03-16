namespace Peace.Lifelog.DataAccess;

using DomainModels;

public interface ICreateDataOnlyDAO : ISqlDAO
{

    Task<Response> CreateData(string sql);
}
