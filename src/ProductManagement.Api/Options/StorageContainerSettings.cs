namespace ProductManagement.Api.Options;

public class StorageContainerSettings
{
    public const string SectionName = "StorageContainers";
    
    public StorageContainerConfig ProductFiles { get; set; }
    public StorageContainerConfig Reports { get; set; }
}

public class StorageContainerConfig
{
    public string ConnectionString { get; set; } = string.Empty;
    public string ContainerName { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
}