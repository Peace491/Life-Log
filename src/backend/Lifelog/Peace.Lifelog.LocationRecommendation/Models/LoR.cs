namespace Peace.Lifelog.LocationRecommendation;

using Peace.Lifelog.Security;

public class GetRecommendationRequest : ILoR
{
    public AppPrincipal? Principal { get; set; }
    public string UserHash { get; set; } = string.Empty;
}

public class ViewRecommendationRequest : ILoR
{
    public AppPrincipal? Principal { get; set; }
    public string UserHash { get; set; } = string.Empty;
}

public class ViewPinRequest : ILoR
{
    public AppPrincipal? Principal { get; set; }
    public string PinId { get; set; } = string.Empty;
    public string LLIId { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public double Latitude { get; set; } = 0;
    public double Longitude { get; set; } = 0;
}

public class UpdateLogRequest : ILoR
{
    public AppPrincipal? Principal { get; set; }
    public string PinId { get; set; } = string.Empty;
    public string LLIId { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public double Latitude { get; set; } = 0;
    public double Longitude { get; set; } = 0;
}