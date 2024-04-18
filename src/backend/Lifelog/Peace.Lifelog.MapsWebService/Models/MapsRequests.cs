using Peace.Lifelog.Security;

namespace Peace.Lifelog.MapsWebService
{
    public class GetAllUserLLIRequest
    {
        public AppPrincipal ?AppPrincipal { get; set; }
        public string userHash { get; set; }
    }

    public class PostGetAllPinFromUserRequest
    {
        public AppPrincipal ?AppPrincipal { get; set; }
        public string userHash { get; set; }
    }

    public class PostCreatePinRequest
    {
        public AppPrincipal ?AppPrincipal { get; set; }
        public string PinId { get; set; } = string.Empty;
        public string LLIId { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public double Latitude { get; set; } = 0;
        public double Longitude { get; set; } = 0;
    }

    public class PostDeletePinRequest
    {
        public AppPrincipal ?AppPrincipal { get; set; }
        public string PinId { get; set; } = string.Empty;
        public string LLIId { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public double Latitude { get; set; } = 0;
        public double Longitude { get; set; } = 0;
    }

    public class PostUpdatePinRequest
    {
        public AppPrincipal ?AppPrincipal { get; set; }
        public string PinId { get; set; } = string.Empty;
        public string LLIId { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public double Latitude { get; set; } = 0;
        public double Longitude { get; set; } = 0;
    }

    public class PostReadPinRequest
    {
        public AppPrincipal ?AppPrincipal { get; set; }
        public string PinId { get; set; } = string.Empty;
        public string LLIId { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public double Latitude { get; set; } = 0;
        public double Longitude { get; set; } = 0;
    }

    public class PostEditPinLIIRequest
    {
        public AppPrincipal? Principal { get; set; }
        public string PinId { get; set; } = string.Empty;
        public string LLIId { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public double Latitude { get; set; } = 0;
        public double Longitude { get; set; } = 0;
    }

    public class PostUpdateLogRequest
    {
        public AppPrincipal? Principal { get; set; }
    }
}