using DomainModels;
using Peace.Lifelog.DataAccess;

namespace Peace.Lifelog.UserManagement;

public class AppUserManagementService : ICreateAccount
{
    public async Task<Response> CreateAccount(UserAccount userAccount)
    {
        #region Input Validation
        var properties = userAccount.GetType().GetProperties();
        foreach (var property in properties)
        {
            var value = userAccount.GetType().GetProperty(property.Name).GetValue(userAccount, null);
            if (value is null) throw new ArgumentNullException(nameof(userAccount.UserId)); 
        }

        #endregion

        var response = new Response();

        // Creating sql statement
        string sql = $"INSERT INTO {userAccount.GetType().Name} ";

        string parameters = "(";
        string values = "(";

        foreach (var property in properties)
        {
            var parameter = property.Name;
            var value = userAccount.GetType().GetProperty(property.Name).GetValue(userAccount, null);

            parameters += $"{parameter}" + ",";
            values += $"{value}" + ",";
        }

        parameters = parameters.Remove(parameters.Length - 1);
        values = values.Remove(values.Length - 1); // Remove extra comma at the end

        sql += parameters + ")" + " VALUES " + values + ");";

        // Create user account in DB
        var createDataOnlyDAO = new CreateDataOnlyDAO();

        var createResponse = await createDataOnlyDAO.CreateData(sql);

        // Populate Response
        response = createResponse;

        return response;

    }
}
