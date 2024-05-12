using DomainModels;
using Peace.Lifelog.Security;

namespace Peace.Lifelog.LocationRecommendation;

public interface IUserValidation
{
    public Response ValidateUser(Response response, string userHash);
}