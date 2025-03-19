using Azure.Storage.Blobs;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using ProductManagement.Api.Options;

namespace ProductManagement.Api.Health;

public class AzureBlobStorageHealthCheck(StorageContainerConfig config) : IHealthCheck
{
    private readonly string _connectionString = config.ConnectionString;
    private readonly string _containerName = config.ContainerName;

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var blobServiceClient = new BlobServiceClient(_connectionString);
            var blobContainerClient = blobServiceClient.GetBlobContainerClient(_containerName);

            if (await blobContainerClient.ExistsAsync(cancellationToken))
            {
                return HealthCheckResult.Healthy("Azure Blob Storage is available.", new Dictionary<string, object>
                {
                    { "containerName", _containerName },
                    { "checkedAt", DateTime.UtcNow.ToString("O") },
                    { "connectionStatus", "Available" },
                });
            }

            return HealthCheckResult.Unhealthy("Azure Blob Storage container does not exist.");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Azure Blob Storage health check failed.", ex, new Dictionary<string, object>
            {
                { "containerName", _containerName },
                { "error", ex.Message }
            });
        }
    }
}