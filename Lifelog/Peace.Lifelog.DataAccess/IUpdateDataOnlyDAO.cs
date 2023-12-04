namespace Peace.Lifelog.DataAccess;

using DomainModels;

public interface IUpdateDataOnlyDAO : ISqlDAO
{
    Task<Response> UpdateData(string sql);
}
