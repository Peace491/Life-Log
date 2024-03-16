namespace Peace.Lifelog.DataAccess;

using DomainModels;

public interface IDeleteDataOnlyDAO : ISqlDAO
{
    Task<Response> DeleteData(string sql);
}