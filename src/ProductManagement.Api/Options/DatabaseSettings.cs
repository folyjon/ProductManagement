namespace ProductManagement.Api.Options;

public class DatabaseSettings
{
    public const string SectionName = "Databases";
    
    public DatabaseConfig DefaultDb { get; set; }
    public DatabaseConfig ReportingDb { get; set; }
}

public class DatabaseConfig
{
    public string ConnectionString { get; set; } = string.Empty;
}