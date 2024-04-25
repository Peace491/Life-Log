namespace Peace.Lifelog.LocationRecommendation;

using Peace.Lifelog.Security;

public class GetRecommendationRequest
{
    public AppPrincipal? Principal { get; set; }
    public string UserHash { get; set; } = string.Empty;
}

public class ViewRecommendationRequest
{
    public AppPrincipal? Principal { get; set; }
    public string UserHash { get; set; } = string.Empty;
}

/*public class ViewPinRequest
{
    public AppPrincipal? Principal { get; set; }
    public string PinId { get; set; } = string.Empty;
    public string LLIId { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public double Latitude { get; set; } = 0;
    public double Longitude { get; set; } = 0;
}

public class UpdateLogRequest
{
    public AppPrincipal? Principal { get; set; }
    public string PinId { get; set; } = string.Empty;
    public string LLIId { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public double Latitude { get; set; } = 0;
    public double Longitude { get; set; } = 0;
}*/