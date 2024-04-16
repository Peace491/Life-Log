namespace Peace.Lifelog.Map;

using Peace.Lifelog.Security;

public class CreatePinRequest : IPinRequest
{
    public AppPrincipal? Principal { get; set; }
    public string PinId { get; set; } = 0;
    public string LLIId { get; set; } = 0;
    public string Address { get; set; } = string.Empty;
    public double Latitude { get; set; } = 0;
    public double Longitude { get; set; } = 0;
}

public class UpdatePinRequest : IPinRequest
{
    public AppPrincipal? Principal { get; set; }
    public string PinId { get; set; } = string.Empty;
    public string LLIId { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public double Latitude { get; set; } = 0;
    public double Longitude { get; set; } = 0;
}

public class DeletePinRequest : IPinRequest
{
    public AppPrincipal? Principal { get; set; }
    public string PinId { get; set; } = string.Empty;
    public string LLIId { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public double Latitude { get; set; } = 0;
    public double Longitude { get; set; } = 0;
}

public class ViewPinRequest : IPinRequest
{
    public AppPrincipal? Principal { get; set; }
    public string PinId { get; set; } = string.Empty;
    public string LLIId { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public double Latitude { get; set; } = 0;
    public double Longitude { get; set; } = 0;
}

public class EditPinLIIRequest : IPinRequest
{
    public AppPrincipal? Principal { get; set; }
    public string PinId { get; set; } = string.Empty;
    public string LLIId { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public double Latitude { get; set; } = 0;
    public double Longitude { get; set; } = 0;
}

public class UpdateLogRequest : IPinRequest
{
    public AppPrincipal? Principal { get; set; }
    public string PinId { get; set; } = string.Empty;
    public string LLIId { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public double Latitude { get; set; } = 0;
    public double Longitude { get; set; } = 0;
}
public static class PinRequestType
{
    public static string Create = "Create";
    public static string Update = "Update";
    public static string Delete = "Delete";
    public static string View = "View";
    public static string Edit = "Edit";
    public static string UpdateLog = "Update Log";
}
