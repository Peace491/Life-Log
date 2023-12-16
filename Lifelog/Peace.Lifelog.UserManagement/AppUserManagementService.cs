using DomainModels;
using Peace.Lifelog.DataAccess;

namespace Peace.Lifelog.UserManagement;

public class AppUserManagementService : ICreateAccount, IRecoverAccount, IDeleteAccount
{
    public async Task<Response> CreateAccount(IUserAccountRequest userAccountRequest)
    {
        #region Input Validation
        if (String.IsNullOrEmpty(userAccountRequest.ModelName))
        {
            throw new ArgumentNullException();
        }

        var properties = userAccountRequest.GetType().GetProperties();
        foreach (var property in properties)
        {
            if (property.Name == "ModelName") { continue; }
            var tupleString = userAccountRequest.GetType().GetProperty(property.Name).GetValue(userAccountRequest, null).ToString();

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
        string sql = $"INSERT INTO {userAccountRequest.ModelName} ";

        string parameters = "(";
        string values = "(";

        foreach (var property in properties)
        {
            if (property.Name == "ModelName") { continue; }

            var tupleString = userAccountRequest.GetType().GetProperty(property.Name).GetValue(userAccountRequest, null).ToString();

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

    public async Task<Response> RecoverMfaAccount(IMultifactorAccountRequest userAccountRequest)
    {
        #region Input Validation
        if (String.IsNullOrEmpty(userAccountRequest.ModelName))
        {
            throw new ArgumentNullException();
        }

        if (String.IsNullOrEmpty(userAccountRequest.UserId.Type))
        {
            throw new ArgumentNullException();
        }

        if (String.IsNullOrEmpty(userAccountRequest.MfaId.Type) || String.IsNullOrEmpty(userAccountRequest.MfaId.Value))
        {
            throw new ArgumentNullException();
        }
        #endregion

        var response = new Response();

        string sql = $"SELECT {userAccountRequest.UserId.Type} FROM {userAccountRequest.ModelName} "
                    + $"WHERE {userAccountRequest.MfaId.Type} = \"{userAccountRequest.MfaId.Value}\"";

        var readDataOnlyDAO = new ReadDataOnlyDAO();

        response  = await readDataOnlyDAO.ReadData(sql);

        return response;
    }

    public async Task<Response> RecoverStatusAccount(IStatusAccountRequest userAccountRequest)
    {
        #region Input Validation
        if (String.IsNullOrEmpty(userAccountRequest.ModelName))
        {
            throw new ArgumentNullException();
        }

        if (String.IsNullOrEmpty(userAccountRequest.UserId.Type) || String.IsNullOrEmpty(userAccountRequest.UserId.Value))
        {
            throw new ArgumentNullException();
        }

        if (String.IsNullOrEmpty(userAccountRequest.AccountStatus.Type) || String.IsNullOrEmpty(userAccountRequest.AccountStatus.Value))
        {
            throw new ArgumentNullException();
        }
        #endregion

        var response = new Response();

        string sql = $"UPDATE {userAccountRequest.ModelName} " 
                    + $"SET {userAccountRequest.AccountStatus.Type} = \"{userAccountRequest.AccountStatus.Value}\" "
                    + $"WHERE {userAccountRequest.UserId.Type} = \"{userAccountRequest.UserId.Value}\"";

        var updateDataOnlyDAO = new UpdateDataOnlyDAO();

        response  = await updateDataOnlyDAO.UpdateData(sql);

        return response;
    }

    public async Task<Response> DeleteAccount(IUserAccountRequest userAccountRequest)
    {
        #region Input Validation
        if (String.IsNullOrEmpty(userAccountRequest.ModelName))
        {
            throw new ArgumentNullException();
        }

        if (String.IsNullOrEmpty(userAccountRequest.UserId.Type) || String.IsNullOrEmpty(userAccountRequest.UserId.Value))
        {
            throw new ArgumentNullException();
        }
        #endregion

        // Create Response
        var response = new Response();

        // Sql string
        var sql = $"DELETE FROM {userAccountRequest.ModelName} WHERE {userAccountRequest.UserId.Type}=\"{userAccountRequest.UserId.Value}\"";

        var deleteOnlyDAO = new DeleteDataOnlyDAO();

        // Get Response
        response = await deleteOnlyDAO.DeleteData(sql);

        return response;
    }
}
