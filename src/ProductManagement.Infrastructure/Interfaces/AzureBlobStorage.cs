namespace ProductManagement.Infrastructure.Interfaces;

public class AzureBlobStorageOptions
{
    public static string SectionName { get; } = "AzureBlobStorage";
    public string ConnectionString { get; set; } = string.Empty;
    public string ContainerName { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
}