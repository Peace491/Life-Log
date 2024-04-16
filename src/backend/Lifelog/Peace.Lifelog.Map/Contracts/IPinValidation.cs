using DomainModels;

namespace Peace.Lifelog.Map;

public interface IPinValidation
{
    public Response ValidatePinRequest(
        Response response, IPinRequest pinRequest, string requestType
    );

}
