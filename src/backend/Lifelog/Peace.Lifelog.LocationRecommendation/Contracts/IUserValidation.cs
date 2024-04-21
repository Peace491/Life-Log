using DomainModels;

namespace Peace.Lifelog.LocationRecommendation;

public interface IUserValidation
{
    public response ValidateUser(Response response, AppPrincipal? principal);
}