using Peace.Lifelog.Security;

namespace Peace.Lifelog.LocationRecommendationWebService
{

    public class PutUpdateLogRequest
    {
        public AppPrincipal? AppPrincipal { get; set; }
    }
    public class GetViewPinRequest
    {
        public AppPrincipal? AppPrincipal { get; set; }
        public string PinId { get; set; } = string.Empty;

    }
}