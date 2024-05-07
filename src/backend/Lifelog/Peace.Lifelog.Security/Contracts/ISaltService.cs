using DomainModels;

namespace Peace.Lifelog.Security;

public interface ISaltService
{
    Response getSalt();
}
