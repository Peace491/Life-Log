namespace Peace.Lifelog.UserForm;

using Peace.Lifelog.Security;

public class CreateUserFormRequest : IUserFormRequest
{
    public AppPrincipal? Principal { get; set; }
    public int MentalHealthRating { get; set; }
    public int PhysicalHealthRating { get; set; }
    public int OutdoorRating { get; set; }
    public int SportRating { get; set; }
    public int ArtRating { get; set; }
    public int HobbyRating { get; set; }
    public int ThrillRating { get; set; }
    public int TravelRating { get; set; }
    public int VolunteeringRating { get; set; }
    public int FoodRating { get; set; }
}

public class UpdateUserFormRequest : IUserFormRequest
{
    public AppPrincipal? Principal { get; set; }
    public int MentalHealthRating { get; set; } = 0;
    public int PhysicalHealthRating { get; set; } = 0;
    public int OutdoorRating { get; set; } = 0;
    public int SportRating { get; set; } = 0;
    public int ArtRating { get; set; } = 0;
    public int HobbyRating { get; set; } = 0;
    public int ThrillRating { get; set; } = 0;
    public int TravelRating { get; set; } = 0;
    public int VolunteeringRating { get; set; } = 0;
    public int FoodRating { get; set; } = 0;

}

public static class UserFormRequestType {
    public static string Create = "Create";
    public static string Update = "Update";
}
