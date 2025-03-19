using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using ProductManagement.Api.Options;

namespace ProductManagement.Api.Health;

public class SqlServerHealthCheck(IOptionsSnapshot<DatabaseSettings> options) : IHealthCheck
{
    private readonly string _connectionString = options.Value.DefaultDb.ConnectionString;

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);

            // If it's a *Liveness* check, we only test if the DB is up.
            if (context.Registration.Name == "SQL Liveness")
            {
                await using (var command = new SqlCommand("SELECT 1", connection))
                {
                    await command.ExecuteScalarAsync(cancellationToken);
                }

                return HealthCheckResult.Healthy("SQL Server is alive.");
            }

            // If it's a *Readiness* check, fetch metadata.
            await using (var command = new SqlCommand("SELECT DB_NAME() AS DatabaseName, SERVERPROPERTY('ProductVersion') AS SqlVersion", connection))
            {
                await using (var reader = await command.ExecuteReaderAsync(cancellationToken))
                {
                    if (!await reader.ReadAsync(cancellationToken))
                        return HealthCheckResult.Unhealthy("SQL Server health check failed.");
                        
                    var data = new Dictionary<string, object>
                    {
                        { "DatabaseName", reader["DatabaseName"] },
                        { "SqlVersion", reader["SqlVersion"] },
                        { "CheckedAt", DateTime.UtcNow.ToString("O") }
                    };

                    return HealthCheckResult.Healthy("SQL Server is available.", data);
                }
            }
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("SQL Server connection failed.", ex, new Dictionary<string, object>
            {
                { "Error", ex.Message }
            });
        }
    }
}