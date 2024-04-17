namespace Peace.Lifelog.Map;

using Peace.Lifelog.Security;

public interface IPinRequest
{
    public AppPrincipal? Principal { get; set; }
    public string PinId { get; set; }
    public string LLIId { get; set; }
    public string Address { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}
