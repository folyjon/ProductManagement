namespace ProductManagement.Api.Options;

public class DatabaseSettings
{
    public const string SectionName = "Databases";
    public Dictionary<string, DatabaseConfig> Databases { get; set; } = new();

    public DatabaseConfig GetDatabase(DatabaseType dbType)
    {
        var key = dbType.ToString();

        if (!Databases.TryGetValue(key, out var dbConfig))
            throw new KeyNotFoundException($"Database configuration for {key} not found.");

        return dbConfig;
    }
}

public class DatabaseConfig
{
    public string ConnectionString { get; set; } = string.Empty;
}

public enum DatabaseType
{
    DefaultDb,
    ReportingDb
}
