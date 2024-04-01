namespace Peace.Lifelog.UserForm;

using Peace.Lifelog.Security;

public interface IUserFormRequest
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
