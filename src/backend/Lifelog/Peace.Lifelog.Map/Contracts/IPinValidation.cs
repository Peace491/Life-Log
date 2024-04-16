using DomainModels;

namespace Peace.Lifelog.Map;

public interface IPinValidation
{
    public Response ValidatePinRequest(
        Response response, IPinRequest userFormRequest, string requestType
    );

}
