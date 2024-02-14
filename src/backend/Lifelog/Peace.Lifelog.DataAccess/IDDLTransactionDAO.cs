namespace Peace.Lifelog.DataAccess;

using DomainModels;

public interface IDDLTransactionDAO : ISqlDAO
{
    Task<Response> ExecuteDDLCommand(string sql);
}
