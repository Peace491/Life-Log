using System.Data;
using System.Reflection;

namespace Peace.Lifelog.Security;

public class AppSpecificConfig
{
    public void SetValue(string name, string value){}
    public string ConnectionString {get; set;}
    public int MaxRetryAttempts {get; set;}
    public int TimeoutLimitInSeconds {get; set;}
    public int MaxUploadCount {get; set;}
    public string AllowUploadFileTypes {get; set;}
}

public static class ConfigService
{
    public static AppSpecificConfig GetConfiguration()
    {
        var configs = new AppSpecificConfig();
        using (var fs = File.OpenText("./config.local.txt"))
        {
            while (!fs.EndOfStream)
            {
                var config = fs.ReadLine();

                var variableName = config?.Split("=")[0];
                var variableValue = config?.Split("=")[1];

                switch(variableName)
                {
                    case "ConnectionString":
                        configs.ConnectionString = config;
                        break;
                    default:
                        break;

                }

                // Reflection
                var configType = typeof(AppSpecificConfig);

                var bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance; // Take this out in actual code

                var allProperties = configType.GetProperties(bindingFlags);

                foreach(var p in allProperties)
                {
                    configs.SetValue(p.Name, variableValue);
                    
                }

                var userAccount = new UserAccount();

                // Should do for data access and authentication
                var sql = "INSERT INTO UserAccount (name, password) values (";
                var sqlParameters = new HashSet<SqlParameter>();

                foreach(var p in allProperties)
                {
                    sql += "@" + p.Name + ",";
                    sqlParameters.Add(new SqlParameter("@" + p.Name, value)
                    {
                        DbType = p.PropertyType
                    });
                    
                }




                

            }

        }

        return configs;
    }

}

// MFA - Losing email
// Account Disabled