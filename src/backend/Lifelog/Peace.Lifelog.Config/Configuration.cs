using Microsoft.Extensions.Configuration;

public class LifelogConfig
{
    public string CreateOnlyConnectionString { get; set; } = "";
    public string ReadOnlyConnectionString { get; set; } = "";
    public string UpdateOnlyConnectionString { get; set; } = "";
    public string DeleteOnlyConnectionstring { get; set; } = "";
    public string LifelogSystemEmail { get; set; } = "";
    public string LifelogSystemEmailAppPassword { get; set; } = "";
    public string MaxExecutionTimeInMilliseconds { get; set; } = "";

    public static LifelogConfig LoadConfiguration()
    {
        var basePath = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "..", "Peace.Lifelog.Config");

        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("lifelog-config.Development.json")
            .Build();

        return configuration.GetSection("LifelogConfig").Get<LifelogConfig>()!;
    }
}