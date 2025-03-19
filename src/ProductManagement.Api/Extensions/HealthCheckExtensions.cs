using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using ProductManagement.Api.Health;

namespace ProductManagement.Api.Extensions;

public static class HealthCheckExtensions
{
    public static IServiceCollection AddCustomHealthChecks(this IServiceCollection services)
    {
        services.AddHealthChecks()
            .AddCheck("self", () => HealthCheckResult.Healthy(), tags: ["default"])
            .AddCheck<SqlServerHealthCheck>("SQL Liveness", tags: ["liveness"])
            .AddCheck<SqlServerHealthCheck>("SQL Readiness", tags: ["readiness", "database", "sql"])
            .AddCheck<AzureBlobStorageHealthCheck>("Azure Blob Storage", tags: ["default", "blob", "storage", "azure"]);

        return services;
    }

    public static void UseCustomHealthChecks(this WebApplication app)
    {
        app.MapHealthChecks("/health/live", new HealthCheckOptions
        {
            Predicate = check => check.Tags.Contains("default") || check.Tags.Contains("liveness"),
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });

        app.MapHealthChecks("/health/ready", new HealthCheckOptions
        {
            Predicate = check => check.Tags.Contains("default") || check.Tags.Contains("readiness"),
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });
    }
}