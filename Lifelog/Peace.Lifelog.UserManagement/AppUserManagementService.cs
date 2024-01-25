using System.Runtime.Serialization;
using DomainModels;
using Peace.Lifelog.DataAccess;
using Peace.Lifelog.Logging;

namespace Peace.Lifelog.UserManagement;

public class AppUserManagementService : ICreateAccount, IRecoverAccount, IModifyProfile, IDeleteAccount
{
    /// <summary>
    /// Create an Account in specified database model
    /// </summary>
    /// <param name="userAccountRequest"></param>
    /// <returns cref="Response"></returns>
    /// <exception cref="ArgumentNullException"></exception>
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

        // Log Account Creation
        var logTarget = new LogTarget(createDataOnlyDAO);
        var logging = new Logging.Logging(logTarget);

        if (response.HasError) {
            var errorMessage = response.ErrorMessage;
            logging.CreateLog("Logs", "TxT3KzlpTG0ExziT6GhXfJDStrAssjrEZjbe14UBfvU=", "ERROR", "Persistent Data Store", errorMessage);
        }
        else {
            logging.CreateLog("Logs", "TxT3KzlpTG0ExziT6GhXfJDStrAssjrEZjbe14UBfvU=", "Info", "Persistent Data Store", $"{userAccountRequest.UserId.Value} account creation successful");
        }

        return response;

    }

    /// <summary>
    /// Recovery an Account that use the MFA model
    /// </summary>
    /// <param name="userAccountRequest"></param>
    /// <returns cref="Response"></returns>
    /// <exception cref="ArgumentNullException"></exception>
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

        if (response.Output is null) 
        {
            response.HasError = true;
            response.ErrorMessage = "Account does not exist";
        }

        // Log Account recovery
        var createDataOnlyDAO = new CreateDataOnlyDAO();
        var logTarget = new LogTarget(createDataOnlyDAO);
        var logging = new Logging.Logging(logTarget);

        if (response.HasError) {
            var errorMessage = response.ErrorMessage;
            logging.CreateLog("Logs", "TxT3KzlpTG0ExziT6GhXfJDStrAssjrEZjbe14UBfvU=", "ERROR", "Persistent Data Store", errorMessage);
        }
        else {
            logging.CreateLog("Logs", "TxT3KzlpTG0ExziT6GhXfJDStrAssjrEZjbe14UBfvU=", "Info", "Persistent Data Store", $"{userAccountRequest.UserId.Value} account recovery successful");
        }

        return response;
    }

    /// <summary>
    /// Recover an Account that uses the status model
    /// </summary>
    /// <param name="userAccountRequest"></param>
    /// <returns cref="Response"></returns>
    /// <exception cref="ArgumentNullException"></exception>
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

        foreach (int rowsAffected in response.Output)
        {
            if (rowsAffected == 0)
            {
                response.HasError = true;
                response.ErrorMessage = "Account does not exist";
            }
        }

        // Log Account recovery
        var createDataOnlyDAO = new CreateDataOnlyDAO();
        var logTarget = new LogTarget(createDataOnlyDAO);
        var logging = new Logging.Logging(logTarget);

        if (response.HasError) {
            var errorMessage = response.ErrorMessage;
            logging.CreateLog("Logs", "TxT3KzlpTG0ExziT6GhXfJDStrAssjrEZjbe14UBfvU=", "ERROR", "Persistent Data Store", errorMessage);
        }
        else {
            logging.CreateLog("Logs", "TxT3KzlpTG0ExziT6GhXfJDStrAssjrEZjbe14UBfvU=", "Info", "Persistent Data Store", $"{userAccountRequest.UserId.Value} account recovery successful");
        }

        return response;
    }

    /// <summary>
    /// Modify a Profile in specified database model
    /// </summary>
    /// <param name="userAccountRequest"></param>
    /// <returns cref="Response"></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public async Task<Response> ModifyProfile(IUserProfileRequest userProfileRequest)
    {
        #region Input Validation
        if (String.IsNullOrEmpty(userProfileRequest.ModelName))
        {
            throw new ArgumentNullException();
        }

        if (String.IsNullOrEmpty(userProfileRequest.UserId.Type) || String.IsNullOrEmpty(userProfileRequest.UserId.Value))
        {
            throw new ArgumentNullException();
        }
        #endregion

        var response = new Response();

        // Creating sql statement
        string sql = $"UPDATE {userProfileRequest.ModelName} SET ";

        string parameters = "";

        var properties = userProfileRequest.GetType().GetProperties();
        foreach (var property in properties)
        {
            if (property.Name == "ModelName" || property.Name == "UserId") { continue; }

            var tupleString = userProfileRequest.GetType().GetProperty(property.Name).GetValue(userProfileRequest, null).ToString();

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
                // This property is not being modified
                continue;
            }

            parameters += $"{parameter} = \"{value}\"" + ",";
        }

        if (parameters.Length == 0) // Every argument is null
        {
            throw new ArgumentNullException();
        }

        parameters = parameters.Remove(parameters.Length - 1); // Remove extra comma at the end

        sql += parameters
            + $" WHERE {userProfileRequest.UserId.Type} = \"{userProfileRequest.UserId.Value}\"" +  ";";

        // Create user account in DB
        var updateDataOnlyDAO = new UpdateDataOnlyDAO();

        response = await updateDataOnlyDAO.UpdateData(sql);

        if (response.HasError == false) // Checking if there is any rows affected
        {
            foreach (int rowsAffected in response.Output)
            {
                if (rowsAffected == 0)
                {
                    response.HasError = true;
                    response.ErrorMessage = "Account does not exist";
                }
            }
        }

        // Log Profile modification
        var createDataOnlyDAO = new CreateDataOnlyDAO();
        var logTarget = new LogTarget(createDataOnlyDAO);
        var logging = new Logging.Logging(logTarget);

        if (response.HasError) {
            var errorMessage = response.ErrorMessage;
            logging.CreateLog("Logs", "TxT3KzlpTG0ExziT6GhXfJDStrAssjrEZjbe14UBfvU=", "ERROR", "Persistent Data Store", errorMessage);
        }
        else {
            logging.CreateLog("Logs", "Info", "TxT3KzlpTG0ExziT6GhXfJDStrAssjrEZjbe14UBfvU=", "Persistent Data Store", $"{userProfileRequest.UserId.Value} account creation successful");
        }

        return response;
        
    }

    /// <summary>
    /// Delete an Account in specified database model
    /// </summary>
    /// <param name="userAccountRequest"></param>
    /// <returns cref="Response"></returns>
    /// <exception cref="ArgumentNullException"></exception>
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

        if (response.HasError == false) // Checking if there is any rows affected
        {
            foreach (int rowsAffected in response.Output)
            {
                if (rowsAffected == 0)
                {
                    response.HasError = true;
                    response.ErrorMessage = "Account does not exist";
                }
            }
        }

        // Log Account deletion
        var createDataOnlyDAO = new CreateDataOnlyDAO();
        var logTarget = new LogTarget(createDataOnlyDAO);
        var logging = new Logging.Logging(logTarget);

        if (response.HasError) {
            var errorMessage = response.ErrorMessage;
            logging.CreateLog("Logs", "TxT3KzlpTG0ExziT6GhXfJDStrAssjrEZjbe14UBfvU=", "ERROR", "Persistent Data Store", errorMessage);
        }
        else {
            logging.CreateLog("Logs", "TxT3KzlpTG0ExziT6GhXfJDStrAssjrEZjbe14UBfvU=", "Info", "Persistent Data Store", $"{userAccountRequest.UserId.Value} account deletion successful");
        }

        return response;
    }
}
