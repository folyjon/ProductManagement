namespace ProductManagement.Api.Options;

public class StorageContainerSettings
{
    public const string SectionName = "StorageContainers";
    
    public Dictionary<string, StorageContainerConfig> StorageContainers { get; set; } = new();
}

public class StorageContainerConfig
{
    public string ConnectionString { get; set; } = string.Empty;
    public string ContainerName { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
}