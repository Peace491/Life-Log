namespace Peace.Lifelog.Map;

using Peace.Lifelog.Security;

public interface IPinRequest
{
    public AppPrincipal? Principal { get; set; }
    public int PinId { get; set; }
    public int LLIId { get; set; }
    public string Address { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}
