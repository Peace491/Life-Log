using Peace.Lifelog.Security;

namespace Peace.Lifelog.MapsWebService
{

    public class GetAllPinFromUserRequest
    {
        public AppPrincipal? AppPrincipal { get; set; }
    }

    public class PostCreatePinRequest
    {
        public AppPrincipal? AppPrincipal { get; set; }
        public string PinId { get; set; } = string.Empty;
        public string LLIId { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public double Latitude { get; set; } = 0;
        public double Longitude { get; set; } = 0;
    }

    public class PutUpdatePinRequest
    {
        public AppPrincipal? AppPrincipal { get; set; }
        public string PinId { get; set; } = string.Empty;
        public string LLIId { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public double Latitude { get; set; } = 0;
        public double Longitude { get; set; } = 0;
    }

    public class GetPinStatusRequest
    {
        public AppPrincipal? AppPrincipal { get; set; }
        public string LLIId { get; set; } = string.Empty;
    }

    public class PutEditPinLIIRequest
    {
        public AppPrincipal? AppPrincipal { get; set; }
        public string PinId { get; set; } = string.Empty;
        public string LLIId { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public double Latitude { get; set; } = 0;
        public double Longitude { get; set; } = 0;
    }

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