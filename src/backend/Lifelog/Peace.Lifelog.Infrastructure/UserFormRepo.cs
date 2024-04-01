using DomainModels;
using Peace.Lifelog.DataAccess;

namespace Peace.Lifelog.Infrastructure;

public class UserFormRepo : IUserFormRepo
{
    private ICreateDataOnlyDAO createDataOnlyDAO;
    private IUpdateDataOnlyDAO updateDataOnlyDAO;

    public UserFormRepo(ICreateDataOnlyDAO createDataOnlyDAO, IUpdateDataOnlyDAO updateDataOnlyDAO)
    {
        this.createDataOnlyDAO = createDataOnlyDAO;
        this.updateDataOnlyDAO = updateDataOnlyDAO;
    }

    public async Task<Response> CreateUserFormInDB(
        string userHash,
        int mentalHealthRating, int physicalHealthRating, int outdoorRating,
        int sportRating, int artRating, int hobbyRating,
        int thrillRating, int travelRating, int volunteeringRating,
        int foodRating
    )
    {
        var createUserFormResponse = new Response();

        string sql = "INSERT INTO UserForm "
        + "(UserHash, MentalHealthRanking, PhysicalHealthRanking, OutdoorRanking, SportRanking, ArtRanking, HobbyRanking, ThrillRanking, TravelRanking, VolunteeringRanking, FoodRanking) "
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

            if (createUserFormResponse.HasError)
            {
                throw new Exception(createUserFormResponse.ErrorMessage);
            }

            var updateAuthenticationResponse = await this.UpdateUserIsCompletedFieldInAuthenticationTableInDB(userHash);

            if (updateAuthenticationResponse.HasError)
            {
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

    public async Task<Response> UpdateUserFormInDB(
        string userHash,
        int mentalHealthRating = 0, int physicalHealthRating = 0, int outdoorRating = 0,
        int sportRating = 0, int artRating = 0, int hobbyRating = 0,
        int thrillRating = 0, int travelRating = 0, int volunteeringRating = 0,
        int foodRating = 0
    )
    {
        var updateUserFormResponse = new Response();

        # region Creating the sql statement
        // Only add the parameters if it is filled out
        string sql = "UPDATE UserForm SET ";

        string parametersAndValues = "";

        if (mentalHealthRating != 0)
        {
            parametersAndValues += $"MentalHealthRanking = {mentalHealthRating}, ";
        }
        if (physicalHealthRating != 0)
        {
            parametersAndValues += $"PhysicalHealthRanking = {physicalHealthRating}, ";
        }
        if (outdoorRating != 0)
        {
            parametersAndValues += $"OutdoorRanking = {outdoorRating}, ";
        }
        if (sportRating != 0)
        {
            parametersAndValues += $"SportRanking = {sportRating}, ";
        }
        if (artRating != 0)
        {
            parametersAndValues += $"ArtRanking = {artRating}, ";
        }
        if (hobbyRating != 0)
        {
            parametersAndValues += $"HobbyRanking = {hobbyRating}, ";
        }
        if (thrillRating != 0)
        {
            parametersAndValues += $"ThrillRanking = {thrillRating}, ";
        }
        if (travelRating != 0)
        {
            parametersAndValues += $"TravelRanking = {travelRating}, ";
        }
        if (volunteeringRating != 0)
        {
            parametersAndValues += $"VolunteeringRanking = {volunteeringRating}, ";
        }
        if (foodRating != 0)
        {
            parametersAndValues += $"FoodRanking = {foodRating}, ";
        }

        // Removing the trailing comma and space
        parametersAndValues = parametersAndValues.TrimEnd(' ', ',');

        sql += parametersAndValues + $" WHERE UserHash=\"{userHash}\"";
        #endregion

        try
        {
            updateUserFormResponse = await updateDataOnlyDAO.UpdateData(sql);
        }
        catch (Exception error)
        {
            updateUserFormResponse.HasError = true;
            updateUserFormResponse.ErrorMessage = error.Message;
            updateUserFormResponse.Output = null;
        }

        return updateUserFormResponse;
    }

    private Task<Response> UpdateUserIsCompletedFieldInAuthenticationTableInDB(string userHash)
    {
        var sql = $"UPDATE LifelogAuthentication SET IsUserFormCompleted = 1 WHERE UserHash=\"{userHash}\"";

        var response = this.updateDataOnlyDAO.UpdateData(sql);

        return response;
    }
}
