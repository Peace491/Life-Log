using DomainModels;
using Peace.Lifelog.DataAccess;

namespace Peace.Lifelog.Infrastructure;

public class UserFormRepo : IUserFormRepo
{
    private ICreateDataOnlyDAO createDataOnlyDAO;
    private IUpdateDataOnlyDAO updateDataOnlyDAO;

    public UserFormRepo(ICreateDataOnlyDAO createDataOnlyDAO, IUpdateDataOnlyDAO updateDataOnlyDAO) {
        this.createDataOnlyDAO = createDataOnlyDAO;
        this.updateDataOnlyDAO = updateDataOnlyDAO;
    }

    public async Task<Response> CreateUserFormInDB(
        string userHash,
        int mentalHealthRating, int physicalHealthRating, int outdoorRating,
        int sportRating, int artRating, int hobbyRating,
        int thrillRating, int travelRating, int volunteeringRating,
        int foodRating) 
    {
        var createUserFormResponse = new Response();

        string sql = "INSERT INTO UserForm " 
        + "(UserHash, MentalHealthRanking, PhysicaHealthRanking, OutdoorRanking, SportRanking, ArtRanking, HobbyRanking, ThrillRanking, TravelRanking, VolunteeringRanking, FoodRanking) "
        + "VALUES "
        + "("
        + $"\"{userHash}\", "
        + $"{mentalHealthRating}, "
        + $"{physicalHealthRating}, "
        + $"{outdoorRating}, "
        + $"{sportRating}, "
        + $"{artRating}, "
        + $"{hobbyRating}, "
        + $"{thrillRating}, "
        + $"{travelRating}, "
        + $"{volunteeringRating}, "
        + $"{foodRating}"
        + ");";

        try 
        {
            createUserFormResponse = await this.createDataOnlyDAO.CreateData(sql);  

            if (createUserFormResponse.HasError) {
                throw new Exception(createUserFormResponse.ErrorMessage);
            }

            var updateAuthenticationResponse = await this.UpdateUserIsCompletedFieldInAuthenticationTableInDB(userHash);

            if (updateAuthenticationResponse.HasError) {
                throw new Exception(updateAuthenticationResponse.ErrorMessage);
            }
        }
        catch (Exception error)
        {
            createUserFormResponse.HasError = true;
            createUserFormResponse.ErrorMessage = error.Message;
            createUserFormResponse.Output = null;
        }   

        return createUserFormResponse;    
    }

    public Task<Response> UpdateUserFormInDB()
    {
        throw new NotImplementedException();
    }

    private Task<Response> UpdateUserIsCompletedFieldInAuthenticationTableInDB(string userHash)
    {
        var sql = $"UPDATE LifelogAuthentication SET IsUserFormCompleted = 1 WHERE UserHash=\"{userHash}\"";
        
        var response = this.updateDataOnlyDAO.UpdateData(sql);

        return response;
    }
}
