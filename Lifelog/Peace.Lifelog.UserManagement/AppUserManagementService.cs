using DomainModels;
using Peace.Lifelog.DataAccess;

namespace Peace.Lifelog.UserManagement;

public class AppUserManagementService : ICreateAccount, IDeleteAccount
{
    public async Task<Response> CreateAccount(BaseUserAccount userAccount)
    {
        #region Input Validation
        if (String.IsNullOrEmpty(userAccount.ModelName))
        {
            throw new ArgumentNullException();
        }

        var properties = userAccount.GetType().GetProperties();
        foreach (var property in properties)
        {
            if (property.Name == "ModelName") { continue; }
            var tupleString = userAccount.GetType().GetProperty(property.Name).GetValue(userAccount, null).ToString();

            // Remove parentheses and split by comma
            var tupleValues = tupleString.Trim('(', ')').Split(',');

            // Trim spaces from each value
            for (int i = 0; i < tupleValues.Length; i++)
            {
                tupleValues[i] = tupleValues[i].Trim();
            }

            var parameter = tupleValues[0];
            var value = tupleValues[1];

            if (String.IsNullOrEmpty(parameter) || String.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException();
            }
        }

        #endregion

        var response = new Response();

        // Creating sql statement
        string sql = $"INSERT INTO {userAccount.ModelName} ";

        string parameters = "(";
        string values = "(";

        foreach (var property in properties)
        {
            if (property.Name == "ModelName") { continue; }

            var tupleString = userAccount.GetType().GetProperty(property.Name).GetValue(userAccount, null).ToString();

            // Remove parentheses and split by comma
            var tupleValues = tupleString.Trim('(', ')').Split(',');

            // Trim spaces from each value
            for (int i = 0; i < tupleValues.Length; i++)
            {
                tupleValues[i] = tupleValues[i].Trim();
            }

            var parameter = tupleValues[0];
            var value = tupleValues[1];

            parameters += $"{parameter}" + ",";
            values += $"\"{value}\"" + ",";
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

    public async Task<Response> DeleteAccount(BaseUserAccount userAccount)
    {
        #region Input Validation
        if (String.IsNullOrEmpty(userAccount.ModelName))
        {
            throw new ArgumentNullException();
        }

        if (String.IsNullOrEmpty(userAccount.UserId.Type) || String.IsNullOrEmpty(userAccount.UserId.Value))
        {
            throw new ArgumentNullException();
        }
        #endregion

        // Create Response
        var response = new Response();

        // Sql string
        var sql = $"DELETE FROM {userAccount.ModelName} WHERE {userAccount.UserId.Type}=\"{userAccount.UserId.Value}\"";

        var deleteOnlyDAO = new DeleteDataOnlyDAO();

        // Get Response
        response = await deleteOnlyDAO.DeleteData(sql);

        return response;
    }
}
