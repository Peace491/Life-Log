using DomainModels;

namespace Peace.Lifelog.Security;

public interface ISalt
{
    Response getSalt();
}
